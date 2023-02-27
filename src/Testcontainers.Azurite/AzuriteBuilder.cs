namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AzuriteBuilder : ContainerBuilder<AzuriteBuilder, AzuriteContainer, AzuriteConfiguration>
{
    /// <summary>
    /// Default Azurite docker image.
    /// </summary>
    public const string DefaultImage = "mcr.microsoft.com/azure-storage/azurite:3.18.0";

    /// <summary>
    /// Default Azurite Blob service port.
    /// </summary>
    public const ushort DefaultBlobPort = 10000;

    /// <summary>
    /// Default Azurite Queue service port.
    /// </summary>
    public const ushort DefaultQueuePort = 10001;

    /// <summary>
    /// Default Azurite Table service port.
    /// </summary>
    public const ushort DefaultTablePort = 10002;

    /// <summary>
    /// Default Azurite workspace directory path '/data/'.
    /// </summary>
    public const string DefaultWorkspaceDirectoryPath = "/data/";

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    public AzuriteBuilder()
        : this(new AzuriteConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AzuriteBuilder(AzuriteConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    internal AzuriteConfiguration Configuration
    {
        get { return new AzuriteConfiguration(DockerResourceConfiguration); }
    }

    /// <inheritdoc />
    protected override AzuriteConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override AzuriteContainer Build()
    {
        var commandArgs = new List<string>();
        commandArgs.AddRange(GetExposedServices());
        commandArgs.AddRange(GetWorkspaceLocationDirectoryPath());
        commandArgs.AddRange(GetDebugModeEnabled());
        commandArgs.Add(GetSilentModeEnabled());
        commandArgs.Add(GetLooseModeEnabled());
        commandArgs.Add(GetSkipApiVersionCheckEnabled());
        commandArgs.Add(GetProductStyleUrlDisabled());
        commandArgs.AddRange(GetHttps());
        commandArgs.AddRange(GetOauthLevel());

        var builder = WithEntrypoint(GetExecutable())
            .WithCommand(commandArgs.Where(x => !string.IsNullOrEmpty(x)).ToArray());

        builder.Validate();

        return new AzuriteContainer(builder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <summary>
    /// Sets a value indicating whether Azurite debug mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Writes logs to the workspace directory path.
    /// Default value is false.
    /// </remarks>
    public AzuriteBuilder WithDebugModeEnabled(bool debugModeEnabled)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(debugModeEnabled));
    }

    /// <summary>
    /// Sets a value indicating whether Azurite silent mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public AzuriteBuilder WithSilentModeEnabled(bool silentModeEnabled)
    {
        return Merge(DockerResourceConfiguration,
            new AzuriteConfiguration(silentModeEnabled: silentModeEnabled));
    }

    /// <summary>
    /// Sets a value indicating whether Azurite loose mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public AzuriteBuilder WithLooseModeEnabled(bool looseModeEnabled)
    {
        return Merge(DockerResourceConfiguration,
            new AzuriteConfiguration(looseModeEnabled: looseModeEnabled));
    }

    /// <summary>
    /// Sets a value indicating whether Azurite skip API version check is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public AzuriteBuilder WithSkipApiVersionCheckEnabled(bool skipApiVersionCheckEnabled)
    {
        return Merge(DockerResourceConfiguration,
            new AzuriteConfiguration(skipApiVersionCheckEnabled: skipApiVersionCheckEnabled));
    }

    /// <summary>
    /// Sets a value indicating whether Azurite product style URL is enabled or not.
    /// </summary>
    /// <remarks>
    /// Parses storage account name from the URI path, instead of the URI host.
    /// Default value is false.
    /// </remarks>
    public AzuriteBuilder WithProductStyleUrlDisabled(bool productStyleUrlDisabled)
    {
        return Merge(DockerResourceConfiguration,
            new AzuriteConfiguration(productStyleUrlDisabled: productStyleUrlDisabled));
    }

    /// <summary>
    /// Sets a pfx file path and its password to enable https endpoints.
    /// </summary>
    public AzuriteBuilder WithHttpsDefinedByPfxFile(string pfxFilePath, string pfxFilePassword)
    {
        return Merge(DockerResourceConfiguration,
            new AzuriteConfiguration(certificate: pfxFilePath, password: pfxFilePassword));
    }

    /// <summary>
    /// Sets a certificate and key pem file paths to enable https endpoints.
    /// </summary>
    public AzuriteBuilder WithHttpsDefinedByPemFiles(string certificateFilePath, string keyFilePath)
    {
        return Merge(DockerResourceConfiguration,
            new AzuriteConfiguration(certificate: certificateFilePath, key: keyFilePath));
    }

    /// <summary>
    /// Sets an OAuth level.
    /// </summary>
    public AzuriteBuilder WithOauth(AzuriteOauthLevels level = AzuriteOauthLevels.Basic)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(oauthLevel: level));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Init()
    {
        return base
            .Init()
            .WithImage(DefaultImage)
            .WithPortBinding(DefaultBlobPort, true)
            .WithPortBinding(DefaultQueuePort, true)
            .WithPortBinding(DefaultTablePort, true)
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilPortIsAvailable(DefaultBlobPort)
                .UntilPortIsAvailable(DefaultQueuePort)
                .UntilPortIsAvailable(DefaultTablePort));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Merge(AzuriteConfiguration oldValue, AzuriteConfiguration newValue)
    {
        return new AzuriteBuilder(new AzuriteConfiguration(oldValue, newValue));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        if (!string.IsNullOrEmpty(DockerResourceConfiguration.Certificate))
        {
            if (string.IsNullOrEmpty(DockerResourceConfiguration.Key) &&
                string.IsNullOrEmpty(DockerResourceConfiguration.Password))
            {
                const string certificateMessage =
                    "Cannot setup HTTPS configuration certificate without provided key or password. Set key pem file or set password for PFX file.\nhttps://github.com/Azure/Azurite#certificate-configuration-https";
                throw new ArgumentException(certificateMessage, nameof(AzuriteConfiguration.Certificate));
            }
        }

        if (DockerResourceConfiguration.OauthLevel.HasValue &&
            string.IsNullOrEmpty(DockerResourceConfiguration.Certificate))
        {
            const string oauthMessage =
                "Cannot setup OAuth configuration without configured HTTPS. Set HTTPS configuration for Azurite or do not use OAuth configuration. More info:\nhttps://github.com/Azure/Azurite#oauth-configuration";
            throw new ArgumentException(oauthMessage, nameof(AzuriteConfiguration.OauthLevel));
        }
    }

    private string GetExecutable()
    {
        return "azurite";
    }

    private string[] GetExposedServices()
    {
        const string defaultRemoteEndpoint = "0.0.0.0";

        var args = new List<string>
        {
            "--blobHost",
            defaultRemoteEndpoint,
            "--queueHost",
            defaultRemoteEndpoint,
            "--tableHost",
            defaultRemoteEndpoint,
        };

        return args.ToArray();
    }

    private string[] GetWorkspaceLocationDirectoryPath()
    {
        return new[] { "--location", DefaultWorkspaceDirectoryPath };
    }

    private string[] GetDebugModeEnabled()
    {
        var debugLogFilePath = Path.Combine(DefaultWorkspaceDirectoryPath, "debug.log");
        return DockerResourceConfiguration.DebugModeEnabled ?? false
            ? new[] { "--debug", debugLogFilePath }
            : Array.Empty<string>();
    }

    private string GetSilentModeEnabled()
    {
        return DockerResourceConfiguration.SilentModeEnabled ?? false ? "--silent" : null;
    }

    private string GetLooseModeEnabled()
    {
        return DockerResourceConfiguration.LooseModeEnabled ?? false ? "--loose" : null;
    }

    private string GetSkipApiVersionCheckEnabled()
    {
        return DockerResourceConfiguration.SkipApiVersionCheckEnabled ?? false ? "--skipApiVersionCheck" : null;
    }

    private string GetProductStyleUrlDisabled()
    {
        return DockerResourceConfiguration.ProductStyleUrlDisabled ?? false ? "--disableProductStyleUrl" : null;
    }

    private string[] GetHttps()
    {
        if (string.IsNullOrEmpty(DockerResourceConfiguration.Certificate))
        {
            return Array.Empty<string>();
        }

        var args = new List<string> { "--cert", DockerResourceConfiguration.Certificate };

        if (!string.IsNullOrEmpty(DockerResourceConfiguration.Key))
        {
            args.Add("--key");
            args.Add(DockerResourceConfiguration.Key);
        }

        if (!string.IsNullOrEmpty(DockerResourceConfiguration.Password))
        {
            args.Add("--pwd");
            args.Add(DockerResourceConfiguration.Password);
        }

        return args.ToArray();
    }

    private string[] GetOauthLevel()
    {
        if (!DockerResourceConfiguration.OauthLevel.HasValue)
        {
            return Array.Empty<string>();
        }

        return new[] { "--oauth", DockerResourceConfiguration.OauthLevel.ToString().ToLowerInvariant() };
    }
}