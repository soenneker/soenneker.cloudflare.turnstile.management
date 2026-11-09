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
    internal sealed class Fixture : IDisposable
    {
        public RecordingHandler Handler { get; } = new();
        private readonly System.Net.Http.HttpClient _http;
        private readonly HttpClientRequestAdapter _adapter;
        public StubClientUtil ClientUtil { get; }
        public CloudflareTurnstileManagementUtil Util { get; }
        public Fixture()
        {
            _http = new System.Net.Http.HttpClient(Handler);
            _adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: _http);
            ClientUtil = new StubClientUtil(new CloudflareOpenApiClient(_adapter), _adapter);
            Util = new CloudflareTurnstileManagementUtil(ClientUtil);
        }
        public void Dispose() { _adapter.Dispose(); _http.Dispose(); }
    }


