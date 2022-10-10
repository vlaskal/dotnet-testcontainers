namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class DockerTlsFixture : IAsyncLifetime
  {
    private const ushort TlsPort = 2376;

    private const string CertsDirectoryName = "certs";

    private readonly ITestcontainersContainer container;

    public DockerTlsFixture()
    {
      this.container = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("docker:20.10.18-dind")
        .WithPrivileged(true)
        .WithEnvironment("DOCKER_CERT_PATH", this.ContainerCertDirectoryPath)
        .WithEnvironment("DOCKER_TLS_CERTDIR", this.ContainerCertDirectoryPath)
        .WithEnvironment("DOCKER_TLS_VERIFY", "1")
        .WithBindMount(this.HostCertDirectoryPath, this.ContainerCertDirectoryPath, AccessMode.ReadWrite)
        .WithPortBinding(TlsPort, true)
        .WithWaitStrategy(Wait.ForUnixContainer()
          .UntilPortIsAvailable(TlsPort))
        .Build();
    }

    public string ContainerCertDirectoryPath { get; }
      = Path.Combine("/", CertsDirectoryName);

    public string HostCertDirectoryPath { get; }
      = Path.Combine(Path.GetTempPath(), CertsDirectoryName);

    public Uri TcpEndpoint
    {
      get
      {
        return new UriBuilder("tcp", this.container.Hostname, this.container.GetMappedPublicPort(TlsPort)).Uri;
      }
    }

    public Task InitializeAsync()
    {
      _ = Directory.CreateDirectory(this.HostCertDirectoryPath);
      return this.container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.container.DisposeAsync().AsTask();
    }
  }
}
