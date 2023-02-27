namespace Testcontainers.Azurite;

public sealed class AzuriteCustomWorkspaceBinding : IClassFixture<AzuriteWithWorkspaceBindingFixture>
{
    private readonly IEnumerable<string> _dataFiles;

    public AzuriteCustomWorkspaceBinding(AzuriteWithWorkspaceBindingFixture azurite)
    {
        var workspaceLocation = azurite.Container.WorkspaceLocationBinding;
        _dataFiles = Directory.Exists(workspaceLocation)
            ? Directory
                .EnumerateFiles(workspaceLocation, "*", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
            : Array.Empty<string>();
    }

    [Fact]
    public void ShouldGetDataFiles()
    {
        Assert.Contains(AzuriteDataFileNames.BlobServiceDataFileName, _dataFiles);
        Assert.Contains(AzuriteDataFileNames.QueueServiceDataFileName, _dataFiles);
        Assert.Contains(AzuriteDataFileNames.TableServiceDataFileName, _dataFiles);
    }
}