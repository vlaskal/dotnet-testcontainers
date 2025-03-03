﻿using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Hosting;

namespace WeatherForecast;

public sealed class DatabaseContainer : IHostedService
{
#pragma warning disable 618
  private readonly TestcontainerDatabase _container = new TestcontainersBuilder<MsSqlTestcontainer>()
#pragma warning restore 618
    .WithDatabase(new DatabaseContainerConfiguration())
    .Build();

  public Task StartAsync(CancellationToken cancellationToken)
  {
    return _container.StartAsync(cancellationToken);
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return _container.StopAsync(cancellationToken);
  }

  public string GetConnectionString()
  {
    return _container.ConnectionString;
  }
}
