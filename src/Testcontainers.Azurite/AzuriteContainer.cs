namespace Testcontainers.Azurite;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class AzuriteContainer : DockerContainer
{
    private readonly AzuriteConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public AzuriteContainer(AzuriteConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the blob container port.
    /// </summary>
    public int BlobContainerPort
    {
        get { return AzuriteBuilder.DefaultBlobPort; }
    }

    /// <summary>
    /// Gets the queue container port.
    /// </summary>
    public int QueueContainerPort
    {
        get { return AzuriteBuilder.DefaultQueuePort; }
    }

    /// <summary>
    /// Gets the table container port.
    /// </summary>
    public int TableContainerPort
    {
        get { return AzuriteBuilder.DefaultTablePort; }
    }

    /// <summary>
    /// Gets the host blob port.
    /// </summary>
    public int BlobPort
    {
        get { return GetMappedPublicPort(BlobContainerPort); }
    }

    /// <summary>
    /// Gets the host queue port.
    /// </summary>
    public int QueuePort
    {
        get { return GetMappedPublicPort(QueueContainerPort); }
    }

    /// <summary>
    /// Gets the host table port.
    /// </summary>
    public int TablePort
    {
        get { return GetMappedPublicPort(TableContainerPort); }
    }

    /// <summary>
    /// Gets the Azurite workspace directory path.
    /// </summary>
    /// <remarks>
    /// Corresponds to the default workspace directory path.
    /// </remarks>
    public string WorkspaceContainerLocation
    {
        get { return AzuriteBuilder.DefaultWorkspaceDirectoryPath; }
    }

    /// <summary>
    /// Gets the directory path where to bind the Azurite workspace directory.
    /// </summary>
    [CanBeNull]
    public string WorkspaceLocationBinding
    {
        get
        {
            return _configuration.Mounts
                .SingleOrDefault(x => x.Target == AzuriteBuilder.DefaultWorkspaceDirectoryPath)?.Source;
        }
    }

    /// <summary>
    /// Gets a value indicating whether debug mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Writes logs to the workspace directory path.
    /// Default value is false.
    /// </remarks>
    public bool DebugModeEnabled
    {
        get { return _configuration.DebugModeEnabled ?? false; }
    }

    /// <summary>
    /// Gets a value indicating whether silent mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public bool SilentModeEnabled
    {
        get { return _configuration.SilentModeEnabled ?? false; }
    }

    /// <summary>
    /// Gets a value indicating whether loose mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public bool LooseModeEnabled
    {
        get { return _configuration.LooseModeEnabled ?? false; }
    }

    /// <summary>
    /// Gets a value indicating whether skip API version check is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public bool SkipApiVersionCheckEnabled
    {
        get { return _configuration.SkipApiVersionCheckEnabled ?? false; }
    }

    /// <summary>
    /// Gets a value indicating whether product style URL is enabled or not.
    /// </summary>
    /// <remarks>
    /// Parses storage account name from the URI path, instead of the URI host.
    /// Default value is false.
    /// </remarks>
    public bool ProductStyleUrlDisabled
    {
        get { return _configuration.ProductStyleUrlDisabled ?? false; }
    }

    /// <summary>
    /// Gets a PFX file path or certificate PEM file path used by https endpoints.
    /// </summary>
    public string Certificate
    {
        get { return _configuration.Certificate; }
    }

    /// <summary>
    /// Gets a password for a protected PFX file.
    /// </summary>
    public string Password
    {
        get { return _configuration.Password; }
    }

    /// <summary>
    /// Gets a key PEM file path used by https endpoints.
    /// </summary>
    public string Key
    {
        get { return _configuration.Key; }
    }

    /// <summary>
    /// Gets the storage connection string.
    /// </summary>
    public string ConnectionString
    {
        get { return GetConnectionString(Hostname, BlobPort, QueuePort, TablePort); }
    }

    /// <summary>
    /// Gets the container storage connection string.
    /// </summary>
    public string ContainerConnectionString
    {
        get
        {
            var alias = _configuration.NetworkAliases?.FirstOrDefault() ?? IpAddress;
            return GetConnectionString(alias, BlobContainerPort, QueueContainerPort,
                TableContainerPort);
        }
    }

    private static string GetEndpoint(UriBuilder uriBuilder, int port)
    {
        uriBuilder.Port = port;
        return uriBuilder.ToString();
    }


    private string GetConnectionString(string host, int blobPort, int queuePort, int tablePort)
    {
        // https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio#well-known-storage-account-and-key.
        const string accountName = "devstoreaccount1";
        const string accountKey =
            "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

        var scheme = string.IsNullOrEmpty(Certificate) ? "http" : "https";

        var endpointBuilder = new UriBuilder(scheme, host, -1, accountName);

        var connectionString = new Dictionary<string, string>
        {
            { "DefaultEndpointsProtocol", endpointBuilder.Scheme },
            { "AccountName", accountName },
            { "AccountKey", accountKey },
            { "BlobEndpoint", GetEndpoint(endpointBuilder, blobPort) },
            { "QueueEndpoint", GetEndpoint(endpointBuilder, queuePort) },
            { "TableEndpoint", GetEndpoint(endpointBuilder, tablePort) },
        };

        return string.Join(";", connectionString.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
}