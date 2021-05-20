using System;
using System.Collections.Generic;
using System.Text;
using Pulumi;
using Pulumi.AzureNative.ContainerService;
using Pulumi.AzureNative.ContainerService.Inputs;
using Stize.Infrastructure.Strategies;

namespace Stize.Infrastructure.Azure.ContainerService
{
    public class AgentPoolBuilder
    {

        internal ManagedClusterAgentPoolProfileArgs Arguments = new ManagedClusterAgentPoolProfileArgs()
        {
            VmSize = "Standard_Ds2_v2",
            Name = "agentpool",
            Count = 3,
            Mode = "System",
            MaxPods = 110,
            OsDiskType = "Managed",
            OsDiskSizeGB = 128,
            OsType = "Linux",
            Type = "VirtualMachineScaleSets",
            
        };

        internal InputMap<string> NodeLabels = new InputMap<string>();

        internal InputList<string> NodeTaints = new InputList<string>();

        ManagedClusterBuilder ClusterBuilder;

        internal AgentPoolBuilder(ManagedClusterBuilder clusterBuilder)
        {
            ClusterBuilder = clusterBuilder;
        }

        public ManagedClusterBuilder Build()
        {
            Arguments.NodeLabels = NodeLabels;
            ClusterBuilder.Arguments.AgentPoolProfiles.Add(Arguments);
            return ClusterBuilder;
        }
    }
}

