using Pulumi;
using Pulumi.AzureNative.ContainerService;

namespace Stize.Infrastructure.Azure.ContainerService
{
    public static class AgentPoolExtensions
    {
        /// <summary>
        /// A list of Availability Zones where the Nodes in this Node Pool should be created in. Changing this forces a new resource to be created.
        /// Valid values: '1', '2', '3'
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="availabilityZone">Availability Zone where the Nodes in this Node Pool should be created in. Valid values: '1', '2', '3'</param>
        /// <returns></returns>
        public static AgentPoolBuilder AvailabilityZones(this AgentPoolBuilder builder, params Input<int>[] availabilityZone)
        {
            InputList<string> zones = new InputList<string>();
            foreach (var zone in availabilityZone)
            {
                zones.Add(zone.Apply(e => e.ToString()));
            }
            builder.Arguments.AvailabilityZones = zones;
            return builder;
        }

        /// <summary>
        /// The initial number of nodes which should exist within this Node Pool. 
        /// Valid values are between 0 and 1000 and must be a value in the range between the MinCount - MaxCount, if specified.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="count">The initial number of nodes which should exist within this Node Pool. 
        /// Valid values are between 0 and 1000 and must be a value in the range between the MinCount - MaxCount, if auto-scaling is enabled.</param>
        /// <returns></returns>
        public static AgentPoolBuilder NodeCount(this AgentPoolBuilder builder, Input<int> count)
        {
            builder.Arguments.Count = count;
            return builder;
        }

        /// <summary>
        /// Whether to enable <see cref="https://docs.microsoft.com/en-us/azure/aks/cluster-autoscaler">auto-scaler</see>. Defaults to false.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AgentPoolBuilder EnableAutoScaling(this AgentPoolBuilder builder)
        {
            builder.Arguments.EnableAutoScaling = true;
            return builder;
        }

        /// <summary>
        /// The minimum number of nodes which should exist within this Node Pool. 
        /// Valid values are between 0 and 1000 and must be less than or equal to max_count.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="minCount">The minimum number of nodes which should exist within this Node Pool. 
        /// Valid values are between 0 and 1000 and must be less than or equal to max_count.</param>
        /// <returns></returns>
        public static AgentPoolBuilder MinCount(this AgentPoolBuilder builder, Input<int> minCount)
        {
            builder.Arguments.MinCount = minCount;
            return builder;
        }

        /// <summary>
        /// The maximum number of nodes which should exist within this Node Pool. 
        /// Valid values are between 0 and 1000 and must be greater than or equal to min_count.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="maxCount">The maximum number of nodes which should exist within this Node Pool. 
        /// Valid values are between 0 and 1000 and must be greater than or equal to min_count.</param>
        /// <returns></returns>
        public static AgentPoolBuilder MaxCount(this AgentPoolBuilder builder, Input<int> maxCount)
        {
            builder.Arguments.MaxCount = maxCount;
            return builder;
        }

        /// <summary>
        /// Should each node have a Public IP Address? Defaults to false.
        /// Specify the public prefix IP resource ID.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="publicIPPrefixID">Public IP Prefix ID. VM nodes use IPs assigned from this Public IP Prefix.</param>
        /// <returns></returns>
        public static AgentPoolBuilder EnableNodePublicIP(this AgentPoolBuilder builder, Input<string> publicIPPrefixID = null)
        {
            builder.Arguments.EnableNodePublicIP = true;
            builder.Arguments.NodePublicIPPrefixID = publicIPPrefixID;
            return builder;
        }

        /// <summary>
        /// The maximum number of pods that can run on each agent.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="maxPods">The maximum number of pods that can run on each agent.</param>
        /// <returns></returns>
        public static AgentPoolBuilder MaxPods(this AgentPoolBuilder builder, Input<int> maxPods)
        {
            builder.Arguments.MaxPods = maxPods;
            return builder;
        }

        /// <summary>
        /// Should this Node Pool be used for System or User resources? 
        /// Valid values are 'System' and 'User'. Defaults to 'User'.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="mode">Should this Node Pool be used for System or User resources? Valid values are 'System' and 'User'. Defaults to 'User'.</param>
        /// <returns></returns>
        public static AgentPoolBuilder Mode(this AgentPoolBuilder builder, InputUnion<string, AgentPoolMode> mode)
        {
            builder.Arguments.Mode = mode;
            return builder;
        }

        /// <summary>
        /// Name of the AgentPool
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name">Name of the AgentPool</param>
        /// <returns></returns>
        public static AgentPoolBuilder Name(this AgentPoolBuilder builder, Input<string> name)
        {
            builder.Arguments.Name = name;
            return builder;
        }
        /// <summary>
        /// The Agent Operating System disk size in GB. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="size">The Agent Operating System disk size in GB.</param>
        /// <returns></returns>
        public static AgentPoolBuilder OsDiskSize(this AgentPoolBuilder builder, Input<int> size)
        {
            builder.Arguments.OsDiskSizeGB = size;
            return builder;
        }

        /// <summary>
        /// The type of disk which should be used for the Operating System. Valid values are 'Ephemeral' and 'Managed'. Defaults to 'Managed'. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="type">The type of disk which should be used for the Operating System. Valid values are 'Ephemeral' and 'Managed'. Defaults to 'Managed'. </param>
        /// <returns></returns>
        public static AgentPoolBuilder OsDiskType(this AgentPoolBuilder builder, InputUnion<string, OSDiskType> type)
        {
            builder.Arguments.OsDiskType = type;
            return builder;
        }

        /// <summary>
        /// Sets Linux as the Operating System which should be used for this Node Pool. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="osType">Sets Linux as the Operating System which should be used for this Node Pool.</param>
        /// <returns></returns>
        public static AgentPoolBuilder WithLinux(this AgentPoolBuilder builder, InputUnion<string, OSSKU> osSKU)
        {
            builder.Arguments.OsType = OSType.Linux;
            builder.Arguments.OsSKU = osSKU;
            return builder;
        }

        /// <summary>
        /// Sets Windows as the Operating System which should be used for this Node Pool. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="osType">Sets Windows as the Operating System which should be used for this Node Pool.</param>
        /// <returns></returns>
        public static AgentPoolBuilder WithWindows(this AgentPoolBuilder builder)
        {
            builder.Arguments.OsType = OSType.Windows;
            return builder;
        }

        /// <summary>
        /// Agent Pool Type represents types of an agent pool.
        /// Valid values are 'VirtualMachineScaleSets' and 'AvailabilitySet'.
        /// Default value is 'VirtualMachineScaleSets'.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="type">AgentPoolType represents types of an agent pool.
        /// Valid values are 'VirtualMachineScaleSets' and 'AvailabilitySet'.
        /// Default value is 'VirtualMachineScaleSets'.</param>
        /// <returns></returns>
        public static AgentPoolBuilder Type(this AgentPoolBuilder builder, InputUnion<string, AgentPoolType> type)
        {
            builder.Arguments.Type = type;
            return builder;
        }

        /// <summary>
        /// The SKU which should be used for the Virtual Machines used in this Node Pool.
        /// For example, 'DS2_v2'.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="size">The SKU which should be used for the Virtual Machines used in this Node Pool.
        /// For example, 'DS2_v2'.</param>
        /// <returns></returns>
        public static AgentPoolBuilder VmSize(this AgentPoolBuilder builder, Input<string> size)
        {
            builder.Arguments.VmSize = size;
            return builder;
        }

        /// <summary>
        /// Enables Host Encryption for this node pool. It is enabled by default.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AgentPoolBuilder EnableEncryptionAtHost(this AgentPoolBuilder builder)
        {
            builder.Arguments.EnableEncryptionAtHost = true;
            return builder;
        }

        /// <summary>
        /// Disables Host Encryption for this node pool. It is disabled by default.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AgentPoolBuilder DisableEncryptionAtHost(this AgentPoolBuilder builder)
        {
            builder.Arguments.EnableEncryptionAtHost = false;
            return builder;
        }

        /// <summary>
        /// Enables the Federal Information Processing Standard (FIPS) for the Operating System. It is enabled by default.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AgentPoolBuilder EnableFIPS(this AgentPoolBuilder builder)
        {
            builder.Arguments.EnableFIPS = true;
            return builder;
        }

        /// <summary>
        /// Disables the Federal Information Processing Standard (FIPS) for the Operating System. It is enabled by default.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AgentPoolBuilder DisableFIPS(this AgentPoolBuilder builder)
        {
            builder.Arguments.EnableFIPS = false;
            return builder;
        }

        /// <summary>
        /// GPUInstanceProfile to be used to specify GPU MIG instance profile for supported GPU VM SKU. 
        /// Valid values are MIG1g, MIG2g, MIG3g, MIG4g and MIG7g.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="gpuProfile">GPUInstanceProfile to be used to specify GPU MIG instance profile for supported GPU VM SKU. 
        /// Valid values are MIG1g, MIG2g, MIG3g, MIG4g and MIG7g.</param>
        /// <returns></returns>
        public static AgentPoolBuilder GPUInstanceProfile(this AgentPoolBuilder builder, InputUnion<string, GPUInstanceProfile> gpuProfile)
        {
            builder.Arguments.GpuInstanceProfile = gpuProfile;
            return builder;
        }

        
        /// <summary>
        /// KubeletDiskType determines the placement of emptyDir volumes, container runtime data root, and Kubelet ephemeral storage. 
        /// Currently allows one value, OS, resulting in Kubelet using the OS disk for data.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="kubeletDiskType">KubeletDiskType determines the placement of emptyDir volumes, container runtime data root, and Kubelet ephemeral storage. 
        /// Currently allows one value, OS, resulting in Kubelet using the OS disk for data.</param>
        /// <returns></returns>
        public static AgentPoolBuilder KubeletDiskType(this AgentPoolBuilder builder, InputUnion<string, KubeletDiskType> kubeletDiskType) 
        {
            builder.Arguments.KubeletDiskType = kubeletDiskType;
            return builder;
        }

        

        /// <summary>
        /// Kubernetes labels which should be applied to nodes in this Node Pool. 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AgentPoolBuilder AddNodeLabel(this AgentPoolBuilder builder, string key, Input<string> value)
        {
            builder.NodeLabels.Add(key, value); 
            return builder;
        }

        /// <summary>
        /// Specify Taints added to nodes during node pool creation/scaling.
        /// Node Taints is a property of Nodes that allows them to repel a set of pods.
        /// The taint information is visible in Kubernetes for handling scheduling rules for nodes. 
        /// The Kubernetes scheduler can use taints to restrict what workloads can run on nodes.
        /// Format: 'key=value:taint_effect'. 
        /// Valid Taint effects are: 'NoSchedule', 'PreferNoSchedule', and 'NoExecute'
        /// For example, 'sku=gpu:NoSchedule'
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="nodeTaint">Specify Taints added to nodes during node pool creation/scaling. Format: 'key=value:taint_effect'. 
        /// Valid Taint effects are: 'NoSchedule', 'PreferNoSchedule', and 'NoExecute'. For example, 'sku=gpu:NoSchedule'</param>
        /// <returns></returns>
        public static AgentPoolBuilder AddNodeTaint(this AgentPoolBuilder builder, Input<string> nodeTaint)
        {
            builder.NodeTaints.Add(nodeTaint);
            return builder;
        }

        /// <summary>
        /// Version of Kubernetes used for the Agents. If not specified, the latest recommended version will be used at provisioning time (but won't auto-upgrade)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="orchestratorVersion">Version of Kubernetes used for the Agents. If not specified, the latest recommended version will be used at provisioning time (but won't auto-upgrade)</param>
        /// <returns></returns>
        public static AgentPoolBuilder OrchestratorVersion(this AgentPoolBuilder builder, Input<string> orchestratorVersion)
        {
            builder.Arguments.OrchestratorVersion = orchestratorVersion;
            return builder;
        }

        /// <summary>
        /// Sets the Subnet for the pods of the node pool.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="podSubnetID"></param>
        /// <returns></returns>
        public static AgentPoolBuilder PodSubnetID(this AgentPoolBuilder builder, Input<string> podSubnetID)
        {
            builder.Arguments.PodSubnetID = podSubnetID; /// I don't know what the difference is between this and <see cref="SubnetId(AgentPoolBuilder, Input{string})"/>
            return builder;
        }

        /// <summary>
        /// The ID of the Proximity Placement Group where the Virtual Machine Scale Set that powers this Node Pool will be placed. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="groupId">The ID of the Proximity Placement Group where the Virtual Machine Scale Set that powers this Node Pool will be placed. </param>
        /// <returns></returns>
        public static AgentPoolBuilder ProximityPlacementGroupId(this AgentPoolBuilder builder, Input<string> groupId)
        {
            builder.Arguments.ProximityPlacementGroupID = groupId;
            return builder;
        }

        /// <summary>
        /// The Eviction Policy which should be used for Virtual Machines within the Virtual Machine Scale Set powering this Node Pool. 
        /// Valid values are 'Deallocate' and 'Delete'.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="evictionPolicy">The Eviction Policy which should be used for Virtual Machines within the Virtual Machine Scale Set powering this Node Pool. 
        /// Valid values are 'Deallocate' and 'Delete'.</param>
        /// <returns></returns>
        public static AgentPoolBuilder ScaleSetEvictionPolicy(this AgentPoolBuilder builder, InputUnion<string, ScaleSetEvictionPolicy> evictionPolicy)
        {
            builder.Arguments.ScaleSetEvictionPolicy = evictionPolicy;
            return builder;
        }

        /// <summary>
        /// The Priority for Virtual Machines within the Virtual Machine Scale Set that powers this Node Pool. 
        /// Valid values are 'Regular' and 'Spot'. Defaults to 'Regular'. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scaleSetPriority">The Priority for Virtual Machines within the Virtual Machine Scale Set that powers this Node Pool. 
        /// Valid values are Regular and Spot. Defaults to Regular. </param>
        /// <returns></returns>
        public static AgentPoolBuilder ScaleSetPriority(this AgentPoolBuilder builder, InputUnion<string, ScaleSetPriority> scaleSetPriority)
        {
            builder.Arguments.ScaleSetPriority = scaleSetPriority;
            return builder;
        }

        /// <summary>
        /// The maximum price you're willing to pay in USD per Virtual Machine. 
        /// Valid values are -1 (the current on-demand price for a Virtual Machine) or a positive value with up to five decimal places. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="price">The maximum price you're willing to pay in USD per Virtual Machine.
        /// Valid values are -1 (the current on-demand price for a Virtual Machine) or a positive value with up to five decimal places. </param>
        /// <returns></returns>
        public static AgentPoolBuilder SpotMaxPrice(this AgentPoolBuilder builder, Input<double> price)
        {
            builder.Arguments.SpotMaxPrice = price;
            return builder;
        }

        /// <summary>
        /// Count or percentage of additional nodes to be added during upgrade. If empty uses AKS default.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="maxSurge">Count or percentage of additional nodes to be added during upgrade. If empty uses AKS default.</param>
        /// <returns></returns>
        public static AgentPoolBuilder NodesAddedOnUpgrade(this AgentPoolBuilder builder, Input<string> maxSurge)
        {
            builder.Arguments.UpgradeSettings = new Pulumi.AzureNative.ContainerService.Inputs.AgentPoolUpgradeSettingsArgs
            {
                MaxSurge = maxSurge
            };
            return builder;
        }

        /// <summary>
        /// The resource ID of the Subnet where this Node Pool should exist.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="subnetId">The resource ID of the Subnet where this Node Pool should exist.</param>
        /// <returns></returns>
        public static AgentPoolBuilder SubnetId(this AgentPoolBuilder builder, Input<string> subnetId)
        {
            builder.Arguments.VnetSubnetID = subnetId;
            return builder;
        }


        /// TBD:
        /// DO I EVEN BOTHER WIH THESE LARGE CONFIGURATION PROPERTIES
        /// KubeletConfig - args
        /// LinuxOSConfig - args
    }
}