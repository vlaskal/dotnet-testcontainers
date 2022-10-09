namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures.Builders;
  using Xunit;

  public class MTlsEndpointAuthenticationProviderTest : IClassFixture<MTlsEndpointAuthenticationProviderFixture>
  {
    private readonly MTlsEndpointAuthenticationProviderFixture fixture;

    public MTlsEndpointAuthenticationProviderTest(MTlsEndpointAuthenticationProviderFixture fixture)
    {
      this.fixture = fixture;
    }

    [Fact]
    public async Task MTlsEndpointAuthenticationProviderIsAbleProvideDockerEndpointConnectWithMTlsConfiguration()
    {
      var versionResponse = await this.fixture.DockerSystemOperations.GetVersion();

      Assert.NotNull(versionResponse);
      Assert.Contains(versionResponse.Components, x => x.Name.ToLowerInvariant() == "engine");
    }
  }
}
