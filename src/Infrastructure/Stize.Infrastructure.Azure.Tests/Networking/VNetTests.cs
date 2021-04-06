
using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.Testing;
using Stize.Infrastructure.Azure.Tests.Networking.Stacks;
using Stize.Infrastructure.Tests.Azure.Networking.Stacks;
using Xunit;

namespace Stize.Infrastructure.Tests.Azure.Networking
{
    public class VNetTests
    {
        [Fact]
        public async Task CreateBasicVnet()
        {
            var resources = await Deployment.TestAsync<VNetBasicStack>(new VNetBasicMock(), new TestOptions { IsPreview = false });
            var vnet = resources.OfType<VirtualNetwork>().FirstOrDefault();

            vnet.Should().NotBeNull("Virtual Network not found");
            vnet.Name.Apply(x => x.Should().Be("vnet1"));
            vnet.Location.Apply(x => x.Should().Be("westeurope"));
        }

        [Fact]
        public async Task AddressSpacesAreCorrect()
        {
            var resources = await Deployment.TestAsync<VNetBasicStack>(new VNetBasicMock(), new TestOptions { IsPreview = false });
            var vnet = resources.OfType<VirtualNetwork>().FirstOrDefault();

            (await vnet.AddressSpace.GetValueAsync())?.AddressPrefixes.Should().Contain("172.16.0.0/24");
        }
    }
}
