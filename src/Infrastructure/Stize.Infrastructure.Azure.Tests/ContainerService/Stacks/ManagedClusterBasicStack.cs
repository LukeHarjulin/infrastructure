using Pulumi;
using Pulumi.AzureNative.ContainerService;
using Stize.Infrastructure.Azure.ContainerService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stize.Infrastructure.Azure.Tests.ContainerService.Stacks
{
    public class ManagedClusterBasicStack : Stack
    {
        public ManagedClusterBasicStack()
        {

            var rg = new ResourceGroupBuilder("rg1")
                    .Name("rg1")
                    .Location("uksouth")
                    .Build();
            var mc1 = new ManagedClusterBuilder("mc1")
                .Name("mc1")
                .Location("uksouth")
                .ResourceGroup(rg.Name)
                .ClusterSku(ManagedClusterSKUName.Basic, ManagedClusterSKUTier.Free)
                .DnsPrefix("mc1-dns")
                .NetworkPluginType(NetworkPlugin.Kubenet)
                .WithNewAppAndServicePrincipal()
                .EnableRBAC(true)
                .Build();
        }
    }
}
