namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class AzuriteConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="debugModeEnabled">A value indicating whether the Azurite debug mode is enabled or not</param>
    /// <param name="silentModeEnabled">A value indicating whether the Azurite silent mode is enabled or not.</param>
    /// <param name="looseModeEnabled">A value indicating whether the Azurite loose mode is enabled or not.</param>
    /// <param name="productStyleUrlDisabled">A value indicating whether the Azurite skip API version check is enabled or not.</param>
    /// <param name="skipApiVersionCheckEnabled">A value indicating whether the Azurite product style URL is enabled or not.</param>
    /// <param name="certificate">A value to certificate pem or PFX file to use by https endpoints.</param>
    /// <param name="password">A value with password for PFX file.</param>
    /// <param name="key">A value to key pem file.</param>
    /// <param name="oauthLevel">A value indicating whether the Azurite should support specified OAuth level.</param>
    public AzuriteConfiguration(
        bool? debugModeEnabled = null,
        bool? silentModeEnabled = null,
        bool? looseModeEnabled = null,
        bool? productStyleUrlDisabled = null,
        bool? skipApiVersionCheckEnabled = null,
        string certificate = null,
        string password = null,
        string key = null,
        AzuriteOauthLevels? oauthLevel = null)
    {
        DebugModeEnabled = debugModeEnabled;
        SilentModeEnabled = silentModeEnabled;
        LooseModeEnabled = looseModeEnabled;
        ProductStyleUrlDisabled = productStyleUrlDisabled;
        SkipApiVersionCheckEnabled = skipApiVersionCheckEnabled;
        Certificate = certificate;
        Password = password;
        Key = key;
        OauthLevel = oauthLevel;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzuriteConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzuriteConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzuriteConfiguration(AzuriteConfiguration resourceConfiguration)
        : this(new AzuriteConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public AzuriteConfiguration(AzuriteConfiguration oldValue, AzuriteConfiguration newValue)
        : base(oldValue, newValue)
    {
        DebugModeEnabled = BuildConfiguration.Combine(oldValue.DebugModeEnabled, newValue.DebugModeEnabled);
        SilentModeEnabled = BuildConfiguration.Combine(oldValue.SilentModeEnabled, newValue.SilentModeEnabled);
        LooseModeEnabled = BuildConfiguration.Combine(oldValue.LooseModeEnabled, newValue.LooseModeEnabled);
        ProductStyleUrlDisabled =
            BuildConfiguration.Combine(oldValue.ProductStyleUrlDisabled, newValue.ProductStyleUrlDisabled);
        SkipApiVersionCheckEnabled =
            BuildConfiguration.Combine(oldValue.SkipApiVersionCheckEnabled, newValue.SkipApiVersionCheckEnabled);
        Certificate = BuildConfiguration.Combine(oldValue.Certificate, newValue.Certificate);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
        Key = BuildConfiguration.Combine(oldValue.Key, newValue.Key);
        OauthLevel = BuildConfiguration.Combine(oldValue.OauthLevel, newValue.OauthLevel);
    }

    /// <summary>
    /// Gets a value indicating whether debug mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Writes logs to the workspace directory path.
    /// Default value is false.
    /// </remarks>
    public bool? DebugModeEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether silent mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public bool? SilentModeEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether loose mode is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public bool? LooseModeEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether skip API version check is enabled or not.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public bool? SkipApiVersionCheckEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether product style URL is enabled or not.
    /// </summary>
    /// <remarks>
    /// Parses storage account name from the URI path, instead of the URI host.
    /// Default value is false.
    /// </remarks>
    public bool? ProductStyleUrlDisabled { get; }

    /// <summary>
    /// Gets a PFX file path or certificate PEM file path used by https endpoints.
    /// </summary>
    public string Certificate { get; }

    /// <summary>
    /// Gets a password for a protected PFX file.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Gets a key PEM file path used by https endpoints.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets a value of OAuth level.
    /// </summary>
    public AzuriteOauthLevels? OauthLevel { get; }
}