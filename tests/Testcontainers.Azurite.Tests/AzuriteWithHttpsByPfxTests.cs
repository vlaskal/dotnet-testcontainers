namespace Testcontainers.Azurite;

public sealed class AzuriteWithHttpsByPfxTests : IClassFixture<AzuriteWithHttpsByPfxFixture>
{
    private readonly AzuriteWithHttpsByPfxFixture _azuriteFixture;

    public AzuriteWithHttpsByPfxTests(AzuriteWithHttpsByPfxFixture azuriteFixture)
    {
        _azuriteFixture = azuriteFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
        // Given
        var blobServiceClient = new BlobServiceClient(_azuriteFixture.Container.ConnectionString,
            new BlobClientOptions { Transport = AzuriteWithHttpsByPfxFixture.HttpClientTransport });

        var queueServiceClient = new QueueServiceClient(_azuriteFixture.Container.ConnectionString,
            new QueueClientOptions { Transport = AzuriteWithHttpsByPfxFixture.HttpClientTransport });

        var tableServiceClient = new TableServiceClient(_azuriteFixture.Container.ConnectionString,
            new TableClientOptions { Transport = AzuriteWithHttpsByPfxFixture.HttpClientTransport });

        // When
        var blobProperties = await blobServiceClient.GetPropertiesAsync()
            .ConfigureAwait(false);

        var queueProperties = await queueServiceClient.GetPropertiesAsync()
            .ConfigureAwait(false);

        var tableProperties = await tableServiceClient.GetPropertiesAsync()
            .ConfigureAwait(false);

        var execResult = await _azuriteFixture.Container
            .ExecAsync(new List<string> { "ls", AzuriteBuilder.DefaultWorkspaceDirectoryPath })
            .ConfigureAwait(false);

        // Then
        Assert.False(blobProperties.IsError());
        Assert.False(queueProperties.IsError());
        Assert.False(tableProperties.IsError());
        Assert.Equal(AzuriteBuilder.DefaultBlobPort, _azuriteFixture.Container.BlobContainerPort);
        Assert.Equal(AzuriteBuilder.DefaultQueuePort, _azuriteFixture.Container.QueueContainerPort);
        Assert.Equal(AzuriteBuilder.DefaultTablePort, _azuriteFixture.Container.TableContainerPort);
        Assert.Contains(AzuriteDataFileNames.BlobServiceDataFileName, execResult.Stdout);
        Assert.Contains(AzuriteDataFileNames.QueueServiceDataFileName, execResult.Stdout);
        Assert.Contains(AzuriteDataFileNames.TableServiceDataFileName, execResult.Stdout);
    }
}