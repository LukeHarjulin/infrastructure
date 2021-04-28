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

        public static ManagedClusterBuilder Name(this ManagedClusterBuilder builder, Input<string> clusterName)
        {
            builder.Arguments.ResourceName = clusterName;
            return builder;
        }

        public static ManagedClusterBuilder Location(this ManagedClusterBuilder builder, Input<string> location)
        {
            builder.Arguments.Location = location;
            return builder;
        }

        public static ManagedClusterBuilder ResourceGroup(this ManagedClusterBuilder builder, Input<string> resourceGroupName)
        {
            builder.Arguments.ResourceGroupName = resourceGroupName;
            return builder;
        }

        public static ManagedClusterBuilder DnsPrefix(this ManagedClusterBuilder builder, Input<string> dnsPrefix)
        {
            builder.Arguments.DnsPrefix = dnsPrefix;
            return builder;
        }
        public static ManagedClusterBuilder KubernetesVersion(this ManagedClusterBuilder builder, Input<string> version)
        {
            builder.Arguments.KubernetesVersion = version;
            return builder;
        }

        public static ManagedClusterBuilder ClusterSku(this ManagedClusterBuilder builder, InputUnion<string, ManagedClusterSKUName> skuName, InputUnion<string, ManagedClusterSKUTier> skuTier)
        {
            builder.Arguments.Sku = new ManagedClusterSKUArgs { Name = skuName, Tier = skuTier };
            return builder;
        }

        public static ManagedClusterBuilder WithExistingServicePrinciple(this ManagedClusterBuilder builder, Input<string> clientID, Input<string> secret)
        {
            builder.Arguments.ServicePrincipalProfile = new ManagedClusterServicePrincipalProfileArgs { ClientId = clientID, Secret = secret};
            return builder;
        }

        public static ManagedClusterBuilder WithDefaultAgentPool(this ManagedClusterBuilder builder, Input<string> clientID, Input<string> secret)
        {
            builder.Arguments.AgentPoolProfiles = new InputList<ManagedClusterAgentPoolProfileArgs> 
            { 
                new ManagedClusterAgentPoolProfileArgs { 
                    Name = "agentpool",
                }
            };
            return builder;
        }

        public static ManagedClusterBuilder WithNewAppAndServicePrincipal(this ManagedClusterBuilder builder)
        {
            var adApp = new Application($"{builder.Arguments.ResourceName}-app");

            var adSp = new ServicePrincipal($"{builder.Arguments.ResourceName}-sp", new ServicePrincipalArgs
            {
                ApplicationId = adApp.Id
            });

            var password = new RandomPassword($"{builder.Arguments.ResourceName}-password", new RandomPasswordArgs
            {
                Length = 20,
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