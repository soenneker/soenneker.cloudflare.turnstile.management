using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cloudflare.OpenApiClient.Models;
using Soenneker.Cloudflare.Turnstile.Management.Abstract;
using Soenneker.Cloudflare.Utils.Client.Abstract;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;

namespace Soenneker.Cloudflare.Turnstile.Management;

public sealed class CloudflareTurnstileManagementUtil(ICloudflareClientUtil clientUtil) : ICloudflareTurnstileManagementUtil
{
    public async ValueTask<AccountsTurnstileWidgetsList200Response?> List(string accountId, string apiKey, int? page = null, int? perPage = null, string? filter = null, AccountsAccountIdChallengesWidgetsOrder? order = null, AccountsAccountIdChallengesWidgetsDirection? direction = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        if (page is < 1) throw new ArgumentOutOfRangeException(nameof(page));
        if (perPage is < 1) throw new ArgumentOutOfRangeException(nameof(perPage));
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).NoSync();
        return await client.Accounts[accountId].Challenges.Widgets.GetAsync(config =>
        {
            config.QueryParameters.Page = page;
            config.QueryParameters.PerPage = perPage;
            config.QueryParameters.Filter = filter;
            config.QueryParameters.Order = order;
            config.QueryParameters.Direction = direction;
        }, cancellationToken).NoSync();
    }

    public async ValueTask<AccountsTurnstileWidgetCreate200Response?> Create(string accountId, string apiKey, AccountsTurnstileWidgetCreateRequest body, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        ArgumentNullException.ThrowIfNull(body);
        ValidateSettings(body.Name, body.Domains);
        if (body.Mode is null) throw new ArgumentException("A widget mode is required.", nameof(body));
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).NoSync();
        return await client.Accounts[accountId].Challenges.Widgets.PostAsync(body, cancellationToken: cancellationToken).NoSync();
    }

    public async ValueTask<AccountsTurnstileWidgetGet200Response?> Get(string accountId, string apiKey, string sitekey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(sitekey);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).NoSync();
        return await client.Accounts[accountId].Challenges.Widgets[sitekey].GetAsync(cancellationToken: cancellationToken).NoSync();
    }

    public async ValueTask<AccountsTurnstileWidgetUpdate200Response?> Update(string accountId, string apiKey, string sitekey, AccountsTurnstileWidgetUpdateRequest body, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(sitekey);
        ArgumentNullException.ThrowIfNull(body);
        ValidateSettings(body.Name, body.Domains);
        if (body.Mode is null) throw new ArgumentException("A widget mode is required.", nameof(body));
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).NoSync();
        return await client.Accounts[accountId].Challenges.Widgets[sitekey].PutAsync(body, cancellationToken: cancellationToken).NoSync();
    }

    public async ValueTask<AccountsTurnstileWidgetDelete200Response?> Delete(string accountId, string apiKey, string sitekey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(sitekey);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).NoSync();
        return await client.Accounts[accountId].Challenges.Widgets[sitekey].DeleteAsync(cancellationToken: cancellationToken).NoSync();
    }

    public async ValueTask<AccountsTurnstileWidgetRotateSecret200Response?> RotateSecret(string accountId, string apiKey, string sitekey, bool invalidateImmediately = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(sitekey);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).NoSync();
        return await client.Accounts[accountId].Challenges.Widgets[sitekey].Rotate_secret.PostAsync(new AccountsTurnstileWidgetRotateSecretRequest { InvalidateImmediately = invalidateImmediately }, cancellationToken: cancellationToken).NoSync();
    }

    private static void ValidateSettings(string? name, List<string>? domains)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(domains);
        foreach (string domain in domains)
            ArgumentException.ThrowIfNullOrWhiteSpace(domain);
    }
}

