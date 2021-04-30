using FluentAssertions;
using Pulumi.AzureNative.ContainerService;
using Pulumi.Testing;
using Stize.Infrastructure.Tests.Azure.KeyVault.Stacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Stize.Infrastructure.Azure.Tests.ContainerService
{
    public class ManagedClusterTests
    {
        [Fact]
        public async Task CreateBasicManagedCluster()
        {
            var resources = await Pulumi.Deployment.TestAsync<VaultBasicStack>(new VaultBasicMock(), new TestOptions { IsPreview = false });
            var mc = resources.OfType<ManagedCluster>().FirstOrDefault();
            mc.Should().NotBeNull("Managed cluster not found");
        }

        [Fact]
        public async Task LocationIsCorrect()
        {
            var resources = await Pulumi.Deployment.TestAsync<VaultBasicStack>(new VaultBasicMock(), new TestOptions { IsPreview = false });
            var mc = resources.OfType<ManagedCluster>().FirstOrDefault();
            mc.Location.Should().NotBeNull("uksouth");
        }
    }
}
