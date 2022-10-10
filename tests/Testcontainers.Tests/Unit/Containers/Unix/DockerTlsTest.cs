namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class DockerTlsTest : IClassFixture<DockerTlsFixture>
  {
    private readonly ICustomConfiguration customConfiguration;

    public DockerTlsTest(DockerTlsFixture dockerTlsFixture)
    {
      IList<string> properties = new List<string>();
      properties.Add($"docker.host={dockerTlsFixture.TcpEndpoint}");
      properties.Add($"docker.cert.path={Path.Combine(dockerTlsFixture.HostCertDirectoryPath, "client")}");
      properties.Add("docker.tls=true");
      this.customConfiguration = new PropertiesFileConfiguration(properties.ToArray());
    }

    [Fact]
    public async Task GetVersionReturnsVersion()
    {
      // Given
      var authConfig = new MTlsEndpointAuthenticationProvider(this.customConfiguration).GetAuthConfig();

      // When
      IDockerSystemOperations systemOperations = new DockerSystemOperations(Guid.Empty, authConfig, NullLogger.Instance);

      var version = await systemOperations.GetVersion()
        .ConfigureAwait(false);

      // Then
      Assert.Equal("20.10.18", version.Version);
    }
  }
}
