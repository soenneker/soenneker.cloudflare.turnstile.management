using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cloudflare.OpenApiClient.Models;

namespace Soenneker.Cloudflare.Turnstile.Management.Tests;

public sealed class WidgetRequestTests
{
    [Test]
    [Arguments("create", "POST", "")]
    [Arguments("update", "PUT", "/sitekey")]
    [Arguments("get", "GET", "/sitekey")]
    [Arguments("delete", "DELETE", "/sitekey")]
    [Arguments("rotate", "POST", "/sitekey/rotate_secret")]
    public async Task Operations_use_expected_requests(string operation, string method, string suffix, CancellationToken cancellationToken)
    {
        using var fixture = new Fixture();
        switch (operation)
        {
            case "create":
                var created = await fixture.Util.Create("account", "token",
                    new AccountsTurnstileWidgetCreateRequest { Name = "Login", Domains = ["example.com"], Mode = AccountsTurnstileWidgetCreateRequestMode.Managed },
                    cancellationToken);
                await Assert.That(created!.Success).IsTrue();
                break;
            case "update":
                await fixture.Util.Update("account", "token", "sitekey",
                    new AccountsTurnstileWidgetUpdateRequest { Name = "Login", Domains = ["example.com"], Mode = AccountsTurnstileWidgetUpdateRequestMode.Managed },
                    cancellationToken);
                break;
            case "get":
                await fixture.Util.Get("account", "token", "sitekey", cancellationToken);
                break;
            case "delete":
                await fixture.Util.Delete("account", "token", "sitekey", cancellationToken);
                break;
            case "rotate":
                await fixture.Util.RotateSecret("account", "token", "sitekey", cancellationToken: cancellationToken);
                break;
        }

        await Assert.That(fixture.Handler.Method).IsEqualTo(method);
        await Assert.That(fixture.Handler.Uri).IsEqualTo("https://api.cloudflare.com/client/v4/accounts/account/challenges/widgets" + suffix);
        await Assert.That(fixture.ClientUtil.Token).IsEqualTo("token");
        if (operation is "create" or "update")
        {
            using var json = JsonDocument.Parse(fixture.Handler.Body!);
            await Assert.That(json.RootElement.GetProperty("name").GetString()).IsEqualTo("Login");
            await Assert.That(json.RootElement.GetProperty("domains")[0].GetString()).IsEqualTo("example.com");
            await Assert.That(json.RootElement.GetProperty("mode").GetString()).IsEqualTo("managed");
        }
        if (operation == "rotate")
        {
            using var json = JsonDocument.Parse(fixture.Handler.Body!);
            await Assert.That(json.RootElement.GetProperty("invalidate_immediately").GetBoolean()).IsFalse();
        }
    }

    [Test]
    public async Task List_preserves_pagination_and_filter(CancellationToken cancellationToken)
    {
        using var fixture = new Fixture();
        fixture.Handler.Response = """{"success":true,"result":[],"result_info":{"page":2,"per_page":10,"total_count":15}}""";
        var response = await fixture.Util.List("account", "token", 2, 10, "name:Login form", cancellationToken: cancellationToken);
        await Assert.That(fixture.Handler.Uri!.Contains("page=2")).IsTrue();
        await Assert.That(fixture.Handler.Uri.Contains("per_page=10")).IsTrue();
        await Assert.That(Uri.UnescapeDataString(fixture.Handler.Uri).Contains("filter=name:Login form")).IsTrue();
        await Assert.That(response!.ResultInfo!.TotalCount).IsEqualTo(15);
    }

    [Test]
    public async Task Rotation_can_invalidate_immediately(CancellationToken cancellationToken)
    {
        using var fixture = new Fixture();
        await fixture.Util.RotateSecret("account", "token", "sitekey", true, cancellationToken);
        using var json = JsonDocument.Parse(fixture.Handler.Body!);
        await Assert.That(json.RootElement.GetProperty("invalidate_immediately").GetBoolean()).IsTrue();
    }

    [Test]
    public async Task Cancellation_stops_before_acquiring_client()
    {
        using var fixture = new Fixture();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        try
        {
            await fixture.Util.Delete("account", "token", "sitekey", cancellation.Token);
            throw new InvalidOperationException("Expected cancellation.");
        }
        catch (OperationCanceledException) { }
        await Assert.That(fixture.ClientUtil.Token).IsNull();
    }

    [Test]
    [Arguments("account")]
    [Arguments("token")]
    [Arguments("sitekey")]
    [Arguments("page")]
    [Arguments("body")]
    public async Task Invalid_arguments_stop_before_acquiring_client(string argument)
    {
        using var fixture = new Fixture();
        try
        {
            switch (argument)
            {
                case "account": await fixture.Util.Get("", "token", "sitekey"); break;
                case "token": await fixture.Util.Get("account", "", "sitekey"); break;
                case "sitekey": await fixture.Util.Get("account", "token", " "); break;
                case "page": await fixture.Util.List("account", "token", page: 0); break;
                case "body": await fixture.Util.Create("account", "token", new AccountsTurnstileWidgetCreateRequest()); break;
            }
            throw new InvalidOperationException("Expected validation failure.");
        }
        catch (ArgumentException) { }
        await Assert.That(fixture.ClientUtil.Token).IsNull();
    }

    [Test]
    public async Task Api_errors_propagate(CancellationToken cancellationToken)
    {
        using var fixture = new Fixture();
        fixture.Handler.Status = HttpStatusCode.Forbidden;
        fixture.Handler.Response = """{"success":false,"errors":[{"code":10000,"message":"Authentication error"}]}""";
        try
        {
            await fixture.Util.Get("account", "token", "sitekey", cancellationToken);
            throw new InvalidOperationException("Expected API failure.");
        }
        catch (Microsoft.Kiota.Abstractions.ApiException error)
        {
            await Assert.That(error.ResponseStatusCode).IsEqualTo(403);
        }
    }
}

