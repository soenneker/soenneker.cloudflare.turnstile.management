using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cloudflare.OpenApiClient.Models;

namespace Soenneker.Cloudflare.Turnstile.Management.Abstract;

/// <summary>Manages account-scoped Cloudflare Turnstile widgets using a Cloudflare API token.</summary>
/// <remarks>
/// Returns generated API envelopes with success and error details. Transport errors propagate from the shared client.
/// Responses may contain widget secrets. The apiKey argument is the Cloudflare API token.
/// </remarks>
public interface ICloudflareTurnstileManagementUtil
{
    /// <summary>Lists one page of widgets with pagination metadata. Filters use field:value (name or sitekey).</summary>
    ValueTask<AccountsTurnstileWidgetsList200Response?> List(string accountId, string apiKey, int? page = null, int? perPage = null, string? filter = null, AccountsAccountIdChallengesWidgetsOrder? order = null, AccountsAccountIdChallengesWidgetsDirection? direction = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a widget.</summary>
    ValueTask<AccountsTurnstileWidgetCreate200Response?> Create(string accountId, string apiKey, AccountsTurnstileWidgetCreateRequest body, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a widget by sitekey.</summary>
    ValueTask<AccountsTurnstileWidgetGet200Response?> Get(string accountId, string apiKey, string sitekey, CancellationToken cancellationToken = default);

    /// <summary>Updates a widget using its required name, domains, mode, and optional settings.</summary>
    ValueTask<AccountsTurnstileWidgetUpdate200Response?> Update(string accountId, string apiKey, string sitekey, AccountsTurnstileWidgetUpdateRequest body, CancellationToken cancellationToken = default);

    /// <summary>Deletes a widget by sitekey.</summary>
    ValueTask<AccountsTurnstileWidgetDelete200Response?> Delete(string accountId, string apiKey, string sitekey, CancellationToken cancellationToken = default);

    /// <summary>Rotates the secret. Unless invalidateImmediately is true, the previous secret remains valid for two hours.</summary>
    ValueTask<AccountsTurnstileWidgetRotateSecret200Response?> RotateSecret(string accountId, string apiKey, string sitekey, bool invalidateImmediately = false, CancellationToken cancellationToken = default);

}

