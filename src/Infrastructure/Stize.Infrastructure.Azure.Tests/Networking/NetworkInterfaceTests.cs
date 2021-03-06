﻿using System.Linq;
using FluentAssertions;
using Pulumi.AzureNative.Network;
using Stize.Infrastructure.Tests.Azure.Networking.Stacks;
using Xunit;
using Stize.Infrastructure.Test;
using System.Threading.Tasks;
using Stize.Infrastructure.Azure.Tests.Networking.Stacks;
using Pulumi.Testing;
using Pulumi;
using System.Collections.Generic;

namespace Stize.Infrastructure.Tests.Azure.Networking
{
    public class NetworkInterfaceTests
    {
        /// <summary>
        /// Checks that the resource is created correctly
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateBasicNIC()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().FirstOrDefault();
            
            nic.Should().NotBeNull("Network Interface not found");
            nic.Name.OutputShould().Be("nic1");
        }
        /// <summary>
        /// Checks the location for the resource is assigned correctly
        /// </summary>
        /// <returns></returns>
        [Fact]        
        public async Task LocationIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            (await nic.Location.GetValueAsync()).Should().Be("westeurope");
        }

        /// <summary>
        /// Checks the Dns Settings for the NIC are correct by checking if the Dns settings are null.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DnsSettingsIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            var t = await nic.DnsSettings.GetValueAsync();
            t?.InternalDnsNameLabel.Should().Be("dnsLabel");            
        }
        /// <summary>
        /// Checks if the Accelerated Networking for the NIC is correct by checking if it is enabled or disabled.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AcceleratedNetworkingIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            (await nic.EnableAcceleratedNetworking.GetValueAsync()).Should().Be(true);
        }
        /// <summary>
        /// Checks if the IP Forwarding for the NIC is correct by checking if it is enabled or disabled.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task IPForwardingIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            (await nic.EnableIPForwarding.GetValueAsync()).Should().Be(true);
        }
        /// <summary>
        /// Checks the extended location of the NIC by checking if the output is null and checking the name of the Extended Location
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExtendedLocationIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            (await nic.ExtendedLocation.GetValueAsync())?.Name.Should().Be("edgelocation");
        }
        /// <summary>
        /// Checks the Subnet of the NIC 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SubnetIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            var subnet = resources.OfType<Subnet>().LastOrDefault();
            var id = await subnet.Id.GetValueAsync();
            (await nic.IpConfigurations.GetValueAsync())[0]?.Subnet?.Id.Should().Be(id);
        }

        [Fact]
        public async Task IPAddressVersionIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            (await nic.IpConfigurations.GetValueAsync())[0].PrivateIPAddressVersion.Should().Be("IPv4");
        }

        [Fact]
        public async Task IPConfigNameIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            (await nic.IpConfigurations.GetValueAsync())[0].Name.Should().Be("ipconfig1");
        }

        [Fact]
        public async Task StaticIPAddressIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            (await nic.IpConfigurations.GetValueAsync())[1].PrivateIPAddress.Should().Be("172.16.0.69");
        }

        [Fact]
        public async Task PrimaryIPIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            (await nic.IpConfigurations.GetValueAsync())[1].Primary.Should().Be(true);
        }
        /// <summary>
        /// Checks the NSG associated with the NIC by checking if the Id of NSG property in the NIC to see if it matches the Id of the NSG resource created
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NetworkSecurityGroupIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            var nsg = resources.OfType<NetworkSecurityGroup>().LastOrDefault();
            var t = await nic.NetworkSecurityGroup.GetValueAsync();
            var id = await nsg.Id.GetValueAsync();
            t?.Id.Should().Be(id);
        }
        /// <summary>
        /// Checks the NSG associated with the NIC by checking if the Id of NSG property in the NIC to see if it matches the Id of the NSG resource created
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TagsIsCorrect()
        {
            var resources = await Deployment.TestAsync<NetworkInterfaceBasicStack>(new NetworkInterfaceBasicMock(), new TestOptions { IsPreview = false });
            var nic = resources.OfType<NetworkInterface>().LastOrDefault();
            var tags = await nic.Tags.GetValueAsync();
            var testTags = new Dictionary<string, string>() { { "env", "dev" } };
            tags?.Should().NotBeNull();
            foreach (var tag in testTags)
            {
                tags?.ContainsKey(tag.Key);
                tags?.ContainsValue(tag.Value);
            }            
        }
    }
}
