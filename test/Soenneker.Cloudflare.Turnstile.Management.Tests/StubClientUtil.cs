using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Cloudflare.OpenApiClient;
using Soenneker.Cloudflare.OpenApiClient.Models;
using Soenneker.Cloudflare.Utils.Client.Abstract;

namespace Soenneker.Cloudflare.Turnstile.Management.Tests;
    internal sealed class StubClientUtil(CloudflareOpenApiClient client, IRequestAdapter adapter) : ICloudflareClientUtil
    {
        public string? Token { get; private set; }
        public ValueTask<CloudflareOpenApiClient> Get(CancellationToken cancellationToken = default) => ValueTask.FromResult(client);
        public ValueTask<CloudflareOpenApiClient> Get(string apiKey, CancellationToken cancellationToken = default)
        {
            Token = apiKey;
            return ValueTask.FromResult(client);
        }
        public ValueTask<IRequestAdapter> GetRequestAdapter(CancellationToken cancellationToken = default) => ValueTask.FromResult(adapter);
        public ValueTask<IRequestAdapter> GetRequestAdapter(string apiKey, CancellationToken cancellationToken = default)
        {
            Token = apiKey;
            return ValueTask.FromResult(adapter);
        }
        public ValueTask<bool> Remove(CancellationToken cancellationToken = default) => ValueTask.FromResult(false);
        public ValueTask<bool> Remove(string apiKey, CancellationToken cancellationToken = default) => ValueTask.FromResult(false);
        public bool RemoveSync(CancellationToken cancellationToken = default) => false;
        public bool RemoveSync(string apiKey, CancellationToken cancellationToken = default) => false;
        public void Dispose() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

