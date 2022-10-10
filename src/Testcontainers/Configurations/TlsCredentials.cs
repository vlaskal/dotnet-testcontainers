namespace DotNet.Testcontainers.Configurations
{
  using System.Net;
  using System.Net.Http;
  using System.Net.Security;
  using Docker.DotNet;
  using Microsoft.Net.Http.Client;

  internal sealed class TlsCredentials : Credentials
  {
    private readonly RemoteCertificateValidationCallback serverCertificateValidationCallback;

    public TlsCredentials(RemoteCertificateValidationCallback serverCertificateValidationCallback)
    {
      this.serverCertificateValidationCallback = serverCertificateValidationCallback ?? ServicePointManager.ServerCertificateValidationCallback;
    }

    public override bool IsTlsCredentials()
    {
      return true;
    }

    public override HttpMessageHandler GetHandler(HttpMessageHandler innerHandler)
    {
      var handler = (ManagedHandler)innerHandler;
      handler.ServerCertificateValidationCallback = this.serverCertificateValidationCallback;
      return handler;
    }
  }
}
