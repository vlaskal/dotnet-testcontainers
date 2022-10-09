namespace DotNet.Testcontainers.Tests.Fixtures.Builders
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public class MTlsEndpointAuthenticationProviderFixture : IAsyncLifetime
  {
    private const int DockerdContainerPort = 2376;
    private const string CertsDirectoryName = "certs";
    private static readonly string ContainerCertDirectoryPath = Path.Combine("/", CertsDirectoryName);
    private static readonly string HostCertDirectoryPath = Path.Combine(Path.GetTempPath(), CertsDirectoryName);

    private readonly ITestcontainersContainer mTlsContainer = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage("docker:20.10-dind")
      .WithPrivileged(true)
      .WithEnvironment("DOCKER_CERT_PATH", ContainerCertDirectoryPath)
      .WithEnvironment("DOCKER_TLS_CERTDIR", ContainerCertDirectoryPath)
      .WithEnvironment("DOCKER_TLS_VERIFY", "1")
      .WithBindMount(HostCertDirectoryPath, ContainerCertDirectoryPath, AccessMode.ReadWrite)
      .WithPortBinding(DockerdContainerPort, true)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(DockerdContainerPort))
      .Build();

    internal DockerSystemOperations DockerSystemOperations { get; private set; }

    public async Task InitializeAsync()
    {
      if (!Directory.Exists(HostCertDirectoryPath))
      {
        Directory.CreateDirectory(HostCertDirectoryPath);
      }

      await this.mTlsContainer.StartAsync();

      var propertiesFileConfiguration = new PropertiesFileConfiguration(
        $"docker.host=tcp://localhost:{this.mTlsContainer.GetMappedPublicPort(DockerdContainerPort)}",
        "docker.tls=true",
        $"docker.cert.path={Path.Combine(HostCertDirectoryPath, "client")}");
      var mTlsEndpointAuthenticationProvider = new MTlsEndpointAuthenticationProvider(propertiesFileConfiguration);
      this.DockerSystemOperations = new DockerSystemOperations(Guid.NewGuid(), mTlsEndpointAuthenticationProvider.GetAuthConfig(), NullLogger.Instance);
    }

    public Task DisposeAsync()
    {
      return this.mTlsContainer.DisposeAsync().AsTask();
    }
  }
}
