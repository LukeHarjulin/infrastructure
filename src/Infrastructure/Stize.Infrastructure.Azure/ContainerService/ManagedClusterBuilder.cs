using System;
using Pulumi;
using Pulumi.AzureNative.ContainerService;
using Pulumi.AzureNative.ContainerService.Inputs;
using Pulumi.Random;
using Stize.Infrastructure.Strategies;

namespace Stize.Infrastructure.Azure.ContainerService
{
    public class ManagedClusterBuilder : BaseBuilder<ManagedCluster>
    {
        /// <summary>
        /// 
        /// </summary>
        public ManagedClusterArgs Arguments { get; private set; } = new ManagedClusterArgs()
        {
            AgentPoolProfiles = new InputList<ManagedClusterAgentPoolProfileArgs>(),
            KubernetesVersion = "1.19.9",
            EnableRBAC = true
        };

        /// <summary>
        /// 
        /// </summary>
        public ManagedClusterAgentPoolProfileArgs PrimaryAgentPool = new ManagedClusterAgentPoolProfileArgs();

        /// <summary>
        /// 
        /// </summary>
        public ContainerServiceNetworkProfileArgs NetworkProfile = new ContainerServiceNetworkProfileArgs() 
        {
            NetworkPlugin = NetworkPlugin.Kubenet,
            
        };

        public ManagedClusterLoadBalancerProfileArgs LoadBalancerProfile = new ManagedClusterLoadBalancerProfileArgs();

        public ManagedClusterPropertiesAutoScalerProfileArgs AutoScalerProfile = new ManagedClusterPropertiesAutoScalerProfileArgs();

        public ManagedClusterAPIServerAccessProfileArgs SecurityProfileArgs = new ManagedClusterAPIServerAccessProfileArgs();

        public InputMap<ManagedClusterAddonProfileArgs> AddonProfiles = new InputMap<ManagedClusterAddonProfileArgs>();

        public ManagedClusterAADProfileArgs AadProfile = new ManagedClusterAADProfileArgs();

        /// <summary>
        /// Creates a new instance of <see="ManagedClusterBuilder" />
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ManagedClusterBuilder(string name) : base(name)
        {
        }
        /// <summary>
        /// Creates a new instance of <see="ManagedClusterBuilder" />
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ManagedClusterBuilder(string name, ResourceContext context) : base(name, context)
        {
        }

        /// <summary>
        /// Creates a new instance of <see="ManagedClusterBuilder" />
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public ManagedClusterBuilder(string name, ManagedClusterArgs arguments) : this(name)
        {
            Arguments = arguments;
        }
        /// <summary>
        /// Creates a new instance of <see="ManagedClusterBuilder" />
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public ManagedClusterBuilder(string name, ManagedClusterArgs arguments, ResourceContext context) : this(name, context)
        {
            Arguments = arguments;
        }

        /// <summary>
        /// Builds and returns the underlying Pulumi object
        /// </summary>
        /// <param name="cro"></param>
        /// <returns></returns>
        public override ManagedCluster Build(CustomResourceOptions cro)
        {
            Arguments.ResourceName = ResourceStrategy.Naming.GenerateName(Arguments.ResourceName);
            ResourceStrategy.Tagging.AddTags(Arguments.Tags);
            NetworkProfile.LoadBalancerProfile = LoadBalancerProfile;
            Arguments.NetworkProfile = NetworkProfile;
            Arguments.AadProfile = AadProfile;
            Arguments.ApiServerAccessProfile = SecurityProfileArgs;
            Arguments.AddonProfiles = AddonProfiles;
            Arguments.AutoScalerProfile = AutoScalerProfile;
            var cluster = new ManagedCluster(Name, Arguments, cro);
            return cluster;
        }
    }
}