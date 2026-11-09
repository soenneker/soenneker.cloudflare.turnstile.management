using System.Threading.Tasks;
using Soenneker.Cloudflare.Turnstile.Management.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Cloudflare.Turnstile.Management.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class CloudflareTurnstileManagementUtilTests : HostedUnitTest
{
    private readonly ICloudflareTurnstileManagementUtil _util;

    public CloudflareTurnstileManagementUtilTests(Host host) : base(host)
    {
        _util = Resolve<ICloudflareTurnstileManagementUtil>(true);
    }

    [Test]
    public async Task Registrar_resolves_util()
    {
        await Assert.That(_util).IsNotNull();
    }
}

