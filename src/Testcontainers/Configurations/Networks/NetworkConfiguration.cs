﻿namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="INetworkConfiguration" />
  [PublicAPI]
  internal sealed class NetworkConfiguration : ResourceConfiguration<NetworksCreateParameters>, INetworkConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkConfiguration" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="driver">The driver.</param>
    /// <param name="options">A dictionary of network options.</param>
    public NetworkConfiguration(
      string name = null,
      NetworkDriver driver = default,
      IReadOnlyDictionary<string, string> options = null)
    {
      this.Name = name;
      this.Driver = driver;
      this.Options = options;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public NetworkConfiguration(IResourceConfiguration<NetworksCreateParameters> resourceConfiguration)
      : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public NetworkConfiguration(INetworkConfiguration resourceConfiguration)
      : this(new NetworkConfiguration(), resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public NetworkConfiguration(INetworkConfiguration oldValue, INetworkConfiguration newValue)
      : base(oldValue, newValue)
    {
      this.Name = BuildConfiguration.Combine(oldValue.Name, newValue.Name);
      this.Driver = BuildConfiguration.Combine(oldValue.Driver, newValue.Driver);
      this.Options = BuildConfiguration.Combine(oldValue.Options, newValue.Options);
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public NetworkDriver Driver { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Options { get; }
  }
}
