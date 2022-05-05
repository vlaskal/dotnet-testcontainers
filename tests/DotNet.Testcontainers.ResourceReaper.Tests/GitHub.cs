namespace DotNet.Testcontainers.ResourceReaper.Tests
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class GitHub : IAsyncLifetime
  {
    private readonly IDockerContainer container = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage("alpine")
      .Build();

    [Fact]
    public void Issue452()
    {
      Assert.NotEmpty(this.container.Name);
    }

    public Task InitializeAsync()
    {
      return this.container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.container.DisposeAsync().AsTask();
    }
  }
}
