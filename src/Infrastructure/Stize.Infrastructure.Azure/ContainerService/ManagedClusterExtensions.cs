using System;
using Pulumi;
using Pulumi.AzureAD;
using Pulumi.AzureNative.ContainerService.Inputs;
using Pulumi.AzureNative.ContainerService;
using Pulumi.Random;
using Pulumi.Tls;
using System.Collections.Generic;

namespace Stize.Infrastructure.Azure.ContainerService
{
    public static class ManagedClusterExtensions
    {
        /// <summary>
        /// Sets the name of the Managed Cluster resource.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="clusterName">Resource name.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder Name(this ManagedClusterBuilder builder, Input<string> clusterName)
        {
            builder.Arguments.ResourceName = clusterName;
            return builder;
        }

        /// <summary>
        /// Sets the location of the Managed Cluster resource.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="location">Location of the resource.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder Location(this ManagedClusterBuilder builder, Input<string> location)
        {
            builder.Arguments.Location = location;
            return builder;
        }

        /// <summary>
        /// The resource group where the Managed Cluster will reside.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="resourceGroupName">Resource Group name.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ResourceGroup(this ManagedClusterBuilder builder, Input<string> resourceGroupName)
        {
            builder.Arguments.ResourceGroupName = resourceGroupName;
            return builder;
        }

        /// <summary>
        /// Dns prefix for the Managed Cluster.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="dnsPrefix">Dns Prefix</param>
        /// <returns></returns>
        public static ManagedClusterBuilder DnsPrefix(this ManagedClusterBuilder builder, Input<string> dnsPrefix)
        {
            builder.Arguments.DnsPrefix = dnsPrefix;
            return builder;
        }

        /// <summary>
        /// Version of Kubernetes that the Managed Cluster enforce.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="version">Kubernetes version. Default is currently '1.19.9'</param>
        /// <returns></returns>
        public static ManagedClusterBuilder KubernetesVersion(this ManagedClusterBuilder builder, Input<string> version = null)
        {
            builder.Arguments.KubernetesVersion = version ?? "1.19.9";
            return builder;
        }

        /// <summary>
        /// Decide whether to enable or disable RBAC Role-Based Access Control.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="enableRbac">Set to 'true' to enable RBAC; set to 'false' to disable RBAC.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableRBAC(this ManagedClusterBuilder builder, Input<bool> enableRbac)
        {
            builder.Arguments.EnableRBAC = enableRbac;
            return builder;
        }

        /// <summary>
        /// The Sku details for the Managed Cluster: Sku name and Sku tier.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="skuName">Sku name for Managed Cluster. Valid values: 'Basic'</param>
        /// <param name="skuTier">Sku tier for Managed Cluster. Valid values: 'Free', 'Paid'</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ClusterSku(this ManagedClusterBuilder builder, InputUnion<string, ManagedClusterSKUName> skuName, InputUnion<string, ManagedClusterSKUTier> skuTier)
        {
            builder.Arguments.Sku = new ManagedClusterSKUArgs { Name = skuName, Tier = skuTier };
            return builder;
        }

        /// <summary>
        /// Adds Agent node pools to the Managed Cluster. Use the <see cref="AgentPoolBuilder"/> to construct an Agent pool and pass it into this method.
        /// Multiple Agent pools can be passed into this method by using a comma-separated list of Agent pools.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="agentPools">Add Agent pools to the Managed Cluster. Use the <see cref="AgentPoolBuilder"/> to construct Agent pools and pass them into this method. 
        /// Multiple Agent pools can be passed into this method by using a comma-separated list of Agent pools.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder AddAgentPool(this ManagedClusterBuilder builder, params Input<ManagedClusterAgentPoolProfileArgs>[] agentPools)
        {
            builder.Arguments.AgentPoolProfiles = agentPools;
            return builder;
        }

        /// <summary>
        /// Set the service principal profile of the Managed Cluster with an existing service principal, using its client ID and the secret.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="clientID">ID of the service principal.</param>
        /// <param name="secret">Secret password associated with the service principal.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithExistingServicePrincipal(this ManagedClusterBuilder builder, Input<string> clientID, Input<string> secret)
        {
            builder.Arguments.ServicePrincipalProfile = new ManagedClusterServicePrincipalProfileArgs { ClientId = clientID, Secret = secret};
            return builder;
        }

        /// <summary>
        /// Adds a primary Agent Pool, with Microsoft Azure default settings, to the Managed Cluster.
        /// Default settings:
        /// Name: "agentpool",
        /// VmSize: "Standard_DS2_v2"
        /// Count: 3
        /// Mode: "System",
        /// MaxPods: 110,
        /// OsDiskType: "Managed",
        /// OsDiskSizeGB: 128,
        /// OsType: "Linux",
        /// Type: "VirtualMachineScaleSets
        /// AvailabilityZones: "None"   
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder AddDefaultAgentPool(this ManagedClusterBuilder builder)
        {
            builder.Arguments.AgentPoolProfiles.Add(new ManagedClusterAgentPoolProfileArgs { 
                Name = "agentpool",
                VmSize = "Standard_DS2_v2",
                Count = 3,
                Mode = "System",
            });
            return builder;
        }

        /// <summary>
        /// The Network Plugin used for building a Kubernetes network.
        /// Valid values are: 'Azure' and 'Kubenet'.
        /// With Kubenet, nodes get an IP address from the Azure VNet subnet. Pods receive an IP address from a logically different address space to the Azure VNet subnet of the nodes.
        /// Selecting Azure ensures that the Azure CNI is used. With Azure CNI, every pod gets an IP address from the subnet and can be accessed directly.
        /// For more information <see href="https://docs.microsoft.com/en-us/azure/aks/configure-kubenet">click here.</see>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="networkPlugin"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder NetworkPluginType(this ManagedClusterBuilder builder, InputUnion<string, NetworkPlugin> networkPlugin)
        {
            builder.NetworkProfile.NetworkPlugin = networkPlugin;
            return builder;
        }

        /// <summary>
        /// Sets the type of identity management for the <see cref="ManagedCluster"/> to system-assigned managed identity.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableSystemAssignedManagedIdentity(this ManagedClusterBuilder builder)
        {
            builder.Arguments.Identity = new ManagedClusterIdentityArgs { Type = ResourceIdentityType.SystemAssigned };
            return builder;
        }

        /// <summary>
        /// Sets the type of identity management for the <see cref="ManagedCluster"/> to user-assigned managed identity.
        /// The resource ID of the user-assigned managed identity must be provided, along with the name of the resource.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="userId">Resource ID of the User-Assigned Managed Identity.</param>
        /// <param name="userName">Name of the User-Assigned Managed Identity.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableUserAssignedManagedIdentity(this ManagedClusterBuilder builder, string userId, string userName)
        {
            builder.Arguments.Identity = new ManagedClusterIdentityArgs 
            { 
                Type = ResourceIdentityType.UserAssigned, 
                UserAssignedIdentities = new InputMap<object> { { userId, userName } }
            };
            return builder;
        }

        /// <summary>
        /// Configure Kubernetes role-based access control (RBAC) using Azure Active Directory group membership. 
        /// This can be used to control access to specific namespaces inside your Kubernetes cluster based on a user's membership in specified Azure Active Directory groups.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="adminGroupObjectIDs"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableAKSManagedAAD(this ManagedClusterBuilder builder, Input<string> tenantId, params Input<string>[] adminGroupObjectIDs)
        {
            builder.Arguments.EnableRBAC = true; // THIS WHOLE AKS AAD STUFF NEEDS RE-DOING AND TESTING - CURRENTLY UNUSABLE
            builder.Arguments.AadProfile = new ManagedClusterAADProfileArgs 
            { 
                TenantID = tenantId, 
                AdminGroupObjectIDs = adminGroupObjectIDs,  
            };
            return builder;
        }

        /// <summary>
        /// Ensures that a new Azure AD Application, Service Principal and a random password is created for a new Service Principal Profile for this Managed Cluster.
        /// The format for the names of the resources are:
        /// Application: "{resourceName}-app"
        /// ServicePrincipal: "{clusterName}-sp"
        /// RandomPassword: "{clusterName}-password"
        /// ServicePrincipalPassword: "{clusterName}-sp-password"
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithNewAppAndServicePrincipal(this ManagedClusterBuilder builder)
        {
            var adApp = new Application($"{builder.Arguments.ResourceName}-app", new ApplicationArgs { DisplayName = $"{builder.Arguments.ResourceName}-app" });

            var adSp = new ServicePrincipal($"{builder.Arguments.ResourceName}-sp", new ServicePrincipalArgs
            {
                ApplicationId = adApp.ApplicationId
            });

            var password = new RandomPassword($"{builder.Arguments.ResourceName}-password", new RandomPasswordArgs
            {
                Length = 10,
                Special = true
            });

            var adSpPassword = new ServicePrincipalPassword($"{builder.Arguments.ResourceName}-sp-password", new ServicePrincipalPasswordArgs
            {
                ServicePrincipalId = adSp.Id,
                Value = password.Result,
                EndDateRelative = "8760h" // 1 year from the creation of this password
            });

            builder.Arguments.ServicePrincipalProfile = new ManagedClusterServicePrincipalProfileArgs
            {
                ClientId = adApp.ApplicationId,
                Secret = adSpPassword.Value
            };            
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="rsaBits"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithNewRsaKey(this ManagedClusterBuilder builder, int rsaBits = 4096)
        {
            var sshKey = new PrivateKey($"{builder.Arguments.ResourceName}-ssh-key", new PrivateKeyArgs
            {
                Algorithm = "RSA",
                RsaBits = rsaBits
            });

            builder.Arguments.LinuxProfile = new ContainerServiceLinuxProfileArgs
            {
                AdminUsername = "stize",
                Ssh = new ContainerServiceSshConfigurationArgs
                {
                    PublicKeys = 
                    {
                        new ContainerServiceSshPublicKeyArgs
                        {
                            KeyData = sshKey.PublicKeyOpenssh,
                        }                        
                    }
                }
            };

            return builder;
        }
    }
}