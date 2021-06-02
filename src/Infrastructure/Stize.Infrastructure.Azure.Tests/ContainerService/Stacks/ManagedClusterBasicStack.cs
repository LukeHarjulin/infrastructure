using Pulumi;
using Pulumi.AzureNative.ContainerService;
using Stize.Infrastructure.Azure;
using Stize.Infrastructure.Azure.ContainerService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stize.Infrastructure.Tests.Azure.ContainerService.Stacks
{
    public class ManagedClusterBasicStack : Stack
    {
        public ManagedClusterBasicStack()
        {

            var rg = new ResourceGroupBuilder("rg1")
                    .Name("rg1")
                    .Location("uksouth")
                    .Build();

            // Declare Agent Pools before MC
        /* 
            var ap1 = new AgentPool()
                .Name("agentpool1")
                ...()
                .Build()
            var ap2 = new AgentPool()
                .Name("agentpool2")
                ...()
                .Build()
        */

            var mc1 = new ManagedClusterBuilder("mctest12345pulumi")
            .Name("mctest12345pulmui")
            .Location(rg.Location)
            .ResourceGroup(rg.Name)
            .KubernetesVersion("1.19.9")
            .DnsPrefix("mctest12345-dns")
            .ClusterSku(ManagedClusterSKUName.Basic, ManagedClusterSKUTier.Free)
            .AddAgentPool()
                .Name("ap1")
                .AvailabilityZones(1,2,3)
                .AddNodeLabel("test", "label")
                .Build()
            //.WithNewAppAndServicePrincipal()
            //.AllowDeletionOfNodesWithKubeSystem()
            //.AllowDeletionOfNodesWithLocalStorage()
            //.BalanceSimilarNodeGroups(true)
            //.EnableAzurePolicy()
            //.EnableHttpAppRouting()
            //.EnableIngressAppGateway()
            //.EnableKubeDashboard()
            .EnableRBAC()
            //.EnableAKSManagedAzureAD()
            //.EnableAzureADForAuth()
            .EnableSystemAssignedManagedIdentity()
            //.ExpanderType(Expander.Least_waste)
            //.LoadBalancerAllocatedPorts(4000)
            //.LoadBalancerIdleTimeout(10)
            .LoadBalancerManagedOutboundIpCount(3)
            .LoadBalancerSku(LoadBalancerSku.Standard)
            //.MaxBulkDeleteOfNodes(3)
            //.MaxNumberOfUnreadyNodes(3)
            //.MaxWaitForProvisioningNode(10)
            .NetworkPluginType(NetworkPlugin.Kubenet)
            .NetworkPolicy(NetworkPolicy.Calico)
            //.NewPodScaleUpDelay(15)
            .OutboundType(OutboundType.LoadBalancer)
            //.ScaleDownDelayAfterAdd(3)
            //.ScaleDownDelayAfterDelete(60)
            //.ScaleDownDelayAfterFailure(5)
            //.ScaleDownUnneededNodes(3)
            //.ScaleDownUnreadyNodes(3)
            //.ScaleDownUtilizationThreshold("0.5")
            //.ScanInterval(30)
            .WithKubenet()
            .Build();


            // Declare AgentPool after MC but would require at least one AgentPool declare in the MC when building the MC.
            /*
                var ap = new AgentPoolBuilder("ap1")
                    .Name("ap1")
                    ...()
                    .Build();
            */
        }
    }
}
