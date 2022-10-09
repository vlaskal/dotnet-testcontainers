namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Docker.DotNet.X509;
  using DotNet.Testcontainers.Configurations;
  using Org.BouncyCastle.Asn1;
  using Org.BouncyCastle.Asn1.Pkcs;
  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.Crypto.Parameters;
  using Org.BouncyCastle.Pkcs;
  using Org.BouncyCastle.Security;
  using Org.BouncyCastle.X509;
  using MSX509 = System.Security.Cryptography.X509Certificates;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class MTlsEndpointAuthenticationProvider : TlsEndpointAuthenticationProvider
  {
    private const string DefaultClientCertFileName = "cert.pem";
    private const string DefaultClientKeyFileName = "key.pem";
    private static readonly Regex PemData = new Regex("-----BEGIN (.*)-----(.*)-----END (.*)-----", RegexOptions.Multiline);

    private readonly Lazy<MSX509.X509Certificate2> clientCertificate;
    private readonly string dockerClientCertFile;
    private readonly string dockerClientKeyFile;
    private readonly bool dockerTlsVerifyEnabled;

    /// <summary>
    ///   Initializes a new instance of the <see cref="MTlsEndpointAuthenticationProvider" /> class.
    /// </summary>
    public MTlsEndpointAuthenticationProvider()
      : this(PropertiesFileConfiguration.Instance, EnvironmentConfiguration.Instance)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="MTlsEndpointAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="customConfigurations">A list of custom configurations.</param>
    public MTlsEndpointAuthenticationProvider(params ICustomConfiguration[] customConfigurations)
      : base(customConfigurations)
    {
      this.dockerTlsVerifyEnabled = customConfigurations
        .Select(customConfiguration => customConfiguration.GetDockerTlsVerify())
        .Aggregate(false, (x, y) => x || y);
      this.dockerClientCertFile = Path.Combine(this.DockerCertPath, DefaultClientCertFileName);
      this.dockerClientKeyFile = Path.Combine(this.DockerCertPath, DefaultClientKeyFileName);
      this.clientCertificate = new Lazy<MSX509.X509Certificate2>(this.GetClientCertificate);
    }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return this.dockerTlsVerifyEnabled;
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      var certificateCredentials = new CertificateCredentials(this.clientCertificate.Value);
      certificateCredentials.ServerCertificateValidationCallback = this.ServerCertificateValidationCallback;
      return new DockerEndpointAuthenticationConfiguration(this.DockerEngine, certificateCredentials);
    }

    private static MSX509.X509Certificate2 GetCertificateWithKey(X509Certificate certificate, AsymmetricKeyParameter privateKey)
    {
      var store = new Pkcs12StoreBuilder()
        .SetUseDerEncoding(true)
        .Build();

      var certEntry = new X509CertificateEntry(certificate);
      store.SetKeyEntry(certificate.SubjectDN.ToString(), new AsymmetricKeyEntry(privateKey), new[] { certEntry });

      byte[] pfxBytes;
      var password = Guid.NewGuid().ToString("N");

      using (var stream = new MemoryStream())
      {
        store.Save(stream, password.ToCharArray(), new SecureRandom());
        pfxBytes = stream.ToArray();
      }

      var result = Pkcs12Utilities.ConvertToDefiniteLength(pfxBytes);

      return new MSX509.X509Certificate2(result, password, MSX509.X509KeyStorageFlags.Exportable);
    }

    private static X509Certificate ReadPemCert(string pathToPemFile)
    {
      var x509CertificateParser = new X509CertificateParser();
      var cert = x509CertificateParser.ReadCertificate(File.ReadAllBytes(pathToPemFile));

      return cert;
    }

    private static AsymmetricKeyParameter ReadPemRsaPrivateKey(string pathToPemFile)
    {
      var keyData = File.ReadAllText(pathToPemFile)
        .Replace("\n", string.Empty);

      var keyMatch = PemData.Match(keyData);
      if (!keyMatch.Success)
      {
        throw new NotSupportedException("Not supported key content");
      }

      if (keyMatch.Groups[1].Value != "RSA PRIVATE KEY")
      {
        throw new NotSupportedException("Not supported key type. Only RSA PRIVATE KEY is supported.");
      }

      var keyContent = keyMatch.Groups[2].Value;
      var keyRawData = Convert.FromBase64String(keyContent);
      var seq = Asn1Sequence.GetInstance(keyRawData);

      if (seq.Count != 9)
      {
        throw new NotSupportedException("Invalid RSA Private Key ASN1 sequence.");
      }

      var keyStructure = RsaPrivateKeyStructure.GetInstance(seq);
      var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(new RsaPrivateCrtKeyParameters(keyStructure));
      var key = PrivateKeyFactory.CreateKey(privateKeyInfo);

      return key;
    }

    private MSX509.X509Certificate2 GetClientCertificate()
    {
      var certificate = ReadPemCert(this.dockerClientCertFile);
      var key = ReadPemRsaPrivateKey(this.dockerClientKeyFile);

      return GetCertificateWithKey(certificate, key);
    }
  }
}
