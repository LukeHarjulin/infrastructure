using System;
using System.Collections.Generic;
using System.Text;
using Pulumi.AzureNative.ContainerService.Inputs;
namespace Stize.Infrastructure.Azure.ContainerService
{
    public class AgentPoolBuilder
    {

        public ManagedClusterAgentPoolProfileArgs Arguments = new ManagedClusterAgentPoolProfileArgs()
        {
            VmSize = "Standard_Ds2_v2",
            Name = "agentpool",
            Count =  3,
            Mode = "System",
            MaxPods = 110,
            OsDiskType = "Managed",
            OsDiskSizeGB = 128,
            OsType = "Linux",
            Type = "VirtualMachineScaleSets",
            AvailabilityZones = "None"   
        };
        public AgentPoolBuilder()
        {

        }

        public ManagedClusterAgentPoolProfileArgs Build()
        {
            return Arguments;
        }
    }
}
