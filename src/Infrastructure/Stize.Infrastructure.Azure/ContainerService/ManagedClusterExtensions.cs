using System;
using Pulumi;
using Pulumi.AzureAD;
using Pulumi.AzureNative.ContainerService.Inputs;
using Pulumi.AzureNative.ContainerService;
using Pulumi.Random;
using Pulumi.Tls;

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
        /// Set the service principle profile of the Managed Cluster with an existing service principle, using its client ID and the secret.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="clientID">ID of the service principle.</param>
        /// <param name="secret">Secret password associated with the service principle.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithExistingServicePrinciple(this ManagedClusterBuilder builder, Input<string> clientID, Input<string> secret)
        {
            builder.Arguments.ServicePrincipalProfile = new ManagedClusterServicePrincipalProfileArgs { ClientId = clientID, Secret = secret};
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithDefaultAgentPool(this ManagedClusterBuilder builder)
        {
            builder.Arguments.AgentPoolProfiles = new InputList<ManagedClusterAgentPoolProfileArgs> 
            { 
                new ManagedClusterAgentPoolProfileArgs { 
                    Name = "agentpool",
                    VmSize = "Standard_DS2_v2",
                    Count = 3,
                    Mode = "System",
                }
            };
            return builder;
        }

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