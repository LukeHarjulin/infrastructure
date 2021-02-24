﻿using Pulumi;
using Pulumi.AzureNextGen.Network.Latest;
using Pulumi.AzureNextGen.Network.Latest.Inputs;

namespace Stize.Infrastructure.Azure.Networking
{
    /// <summary>
    /// Network Interface class builder
    /// </summary>
    public class NetworkInterfaceBuilder : BaseBuilder<NetworkInterface>
    {
        /// <summary>
        /// NI arguments
        /// </summary>
        public NetworkInterfaceArgs Arguments { get; private set; } = new NetworkInterfaceArgs();

        public NetworkInterfaceIPConfigurationArgs IpConfigArgs = new NetworkInterfaceIPConfigurationArgs();
        /// <summary>
        /// Creates a new instance of <see cref="NetworkInterfaceBuilder"/>
        /// </summary>
        /// <param name="name"></param>
        public NetworkInterfaceBuilder(string name) : base(name)
        {

        }

        /// <summary>
        /// Builds the Network interface
        /// </summary>
        /// <param name="cro">Custom Resource Object</param>
        /// <returns></returns>
        public override NetworkInterface Build(CustomResourceOptions cro)
        {
            Arguments.IpConfigurations = IpConfigArgs;
            var nic = new NetworkInterface(Name, Arguments, cro);
            return nic;
        }
    }
}