namespace Testcontainers.Azurite;

[UsedImplicitly]
public class AzuriteDefaultFixture : IAsyncLifetime
{
    public AzuriteDefaultFixture()
        : this(builder => builder)
    {
    }

    protected AzuriteDefaultFixture(Func<AzuriteBuilder, AzuriteBuilder> modifier)
    {
        var builder = modifier(new AzuriteBuilder());
        Container = builder.Build();
    }

    public AzuriteContainer Container { get; }

    public Task InitializeAsync()
    {
        return Container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return Container.DisposeAsync().AsTask();
    }
}