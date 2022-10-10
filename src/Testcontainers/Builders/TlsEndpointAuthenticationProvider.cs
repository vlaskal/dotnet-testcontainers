namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Net.Security;
  using System.Security.Cryptography.X509Certificates;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal class TlsEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider
  {
    private const string DefaultUserDockerFolderName = ".docker";
    private const string DefaultCaCertFileName = "ca.pem";
    private static readonly Uri DefaultTlsDockerEndpoint = new Uri("tcp://localhost:2376");
    private static readonly string DefaultCertPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), DefaultUserDockerFolderName);

    private readonly Lazy<X509Certificate2> caCertificate;
    private readonly bool dockerTlsEnabled;

    /// <summary>
    ///   Initializes a new instance of the <see cref="TlsEndpointAuthenticationProvider" /> class.
    /// </summary>
    public TlsEndpointAuthenticationProvider()
      : this(PropertiesFileConfiguration.Instance, EnvironmentConfiguration.Instance)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="TlsEndpointAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="customConfigurations">A list of custom configurations.</param>
    public TlsEndpointAuthenticationProvider(params ICustomConfiguration[] customConfigurations)
    {
      this.DockerCertPath = customConfigurations
        .Select(customConfiguration => customConfiguration.GetDockerCertPath())
        .FirstOrDefault(value => value != null) ?? DefaultCertPath;
      this.DockerEngine = customConfigurations
        .Select(customConfiguration => customConfiguration.GetDockerHost())
        .FirstOrDefault(value => value != null) ?? DefaultTlsDockerEndpoint;
      this.dockerTlsEnabled = customConfigurations
        .Select(customConfiguration => customConfiguration.GetDockerTls())
        .Aggregate(false, (x, y) => x || y);
      this.caCertificate = new Lazy<X509Certificate2>(() => new X509Certificate2(Path.Combine(this.DockerCertPath, DefaultCaCertFileName)));
    }

    /// <summary>
    ///   Gets path to the docker certificate folder.
    /// </summary>
    protected string DockerCertPath { get; }

    /// <summary>
    /// Gets URI to the docker engine.
    /// </summary>
    protected Uri DockerEngine { get; }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return this.dockerTlsEnabled;
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(this.DockerEngine, new TlsCredentials(this.ServerCertificateValidationCallback));
    }

    protected bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
      switch (sslPolicyErrors)
      {
        case SslPolicyErrors.None:
          return true;
        case SslPolicyErrors.RemoteCertificateNotAvailable:
        case SslPolicyErrors.RemoteCertificateNameMismatch:
          return false;
        case SslPolicyErrors.RemoteCertificateChainErrors:
        default:
          var validationChain = new X509Chain();
          validationChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
          validationChain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
          validationChain.ChainPolicy.ExtraStore.Add(this.caCertificate.Value);
          validationChain.ChainPolicy.ExtraStore.AddRange(chain.ChainElements.OfType<X509ChainElement>().Select(element => element.Certificate).ToArray());
          var isVerified = validationChain.Build(certificate as X509Certificate2 ?? new X509Certificate2(certificate));
          var isSignedByExpectedRoot = validationChain.ChainElements[validationChain.ChainElements.Count - 1].Certificate.RawData.SequenceEqual(this.caCertificate.Value.RawData);
          var isSuccess = isVerified && isSignedByExpectedRoot;
          return isSuccess;
      }
    }
  }
}
