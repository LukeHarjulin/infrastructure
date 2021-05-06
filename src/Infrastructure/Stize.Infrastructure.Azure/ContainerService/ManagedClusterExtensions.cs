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
            builder.Arguments.AgentPoolProfiles = agentPools; // NEED TO LOOK INTO THIS MORE. SEE "default_node_pool" on https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/kubernetes_cluster
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
            builder.Arguments.ServicePrincipalProfile = new ManagedClusterServicePrincipalProfileArgs { ClientId = clientID, Secret = secret };
            return builder;
        }

        /// <summary>
        /// Adds a primary Agent Pool, with Microsoft Azure default settings, to the Managed Cluster.
        /// Default settings:
        /// Name: "agentpool",
        /// VmSize: "Standard_DS2_v2",
        /// Count: 3,
        /// Mode: "System",
        /// MaxPods: 110,
        /// OsDiskType: "Managed",
        /// OsDiskSizeGB: 128,
        /// OsType: "Linux",
        /// Type: "VirtualMachineScaleSets",
        /// AvailabilityZones: "None"   
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder AddDefaultAgentPool(this ManagedClusterBuilder builder)
        {
            builder.Arguments.AgentPoolProfiles.Add(new ManagedClusterAgentPoolProfileArgs
            {
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
        /// Disables Kubernetes role-based access control.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder DisableRBAC(this ManagedClusterBuilder builder)
        {
            builder.Arguments.EnableRBAC = false;
            return builder;
        }

        /// <summary>
        /// Enables Kubernetes role-based access control which provides fine-grained control over cluster resources.
        /// Enables AKS-managed Azure AD.
        /// This can be used to control access to specific namespaces inside your Kubernetes cluster based on a user's membership in specified Azure Active Directory groups.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tenantId">The Tenant ID used for Azure Active Directory Application.</param>
        /// <param name="adminGroupObjectIDs">A comma-seperated list of Object IDs of Azure Active Directory Groups which should have Admin Role on the Cluster.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableAKSManagedAzureAD(this ManagedClusterBuilder builder, Input<string> tenantId, params Input<string>[] adminGroupObjectIDs)
        {
            builder.Arguments.EnableRBAC = true;
            builder.AadProfile.Managed = true;
            builder.AadProfile.AdminGroupObjectIDs = adminGroupObjectIDs;
            builder.AadProfile.TenantID = tenantId;
            return builder;
        }

        /// <summary>
        /// Enable AKS to use Azure AD for user authentication.
        /// Azure Kubernetes Service Managed Azure AD must be enabled for this cluster to use AKS for Azure AD authentication.
        /// Azure AD based RBAC is in Public Preview - more information and details on how to opt into the Preview can be found in <see href="https://docs.microsoft.com/en-us/azure/aks/manage-azure-rbac">this article.</see>
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableAzureADForAuth(this ManagedClusterBuilder builder)
        {
            builder.AadProfile.EnableAzureRBAC = true;
            return builder;
        }

        /// <summary>
        /// Enables Kubernetes role-based access control which provides fine-grained control over cluster resources.
        /// Specify the resource ID for client application used for user login via kubectl, and the server application, the managed cluster's API server application.
        /// The secret for the API server must also be specified.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="clientAppId">The ID of an Azure Active Directory client application of type "Native". This application is for user login via kubectl.</param>
        /// <param name="serverAppId">The ID of an Azure Active Directory server application of type "Web app/API". This application represents the managed cluster's apiserver (Server application).</param>
        /// <param name="serverAppSecret">The secret of an Azure Active Directory server application.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithAzureADClientServerApp(this ManagedClusterBuilder builder, Input<string> clientAppId, Input<string> serverAppId,
            Input<string> serverAppSecret)
        {
            builder.Arguments.EnableRBAC = true;
            builder.AadProfile.Managed = false;
            builder.AadProfile.ClientAppID = clientAppId;
            builder.AadProfile.ServerAppID = serverAppId;
            builder.AadProfile.ServerAppSecret = serverAppSecret;
            return builder;
        }

        /// <summary>
        /// With kubenet, nodes get an IP address from a virtual network subnet. 
        /// Network address translation (NAT) is then configured on the nodes, and pods receive an IP address "hidden" behind the node IP. 
        /// This approach reduces the number of IP addresses that you need to reserve in your network space for pods to use.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="podIpAddressRange">The CIDR to use for pod IP addresses.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithKubenet(this ManagedClusterBuilder builder, Input<string> podIpAddressRange = null)
        {
            builder.NetworkProfile.NetworkPlugin = NetworkPlugin.Kubenet;
            builder.NetworkProfile.PodCidr = podIpAddressRange;
            return builder;
        }

        /// <summary>
        /// The Azure CNI networking plug-in allows clusters to use a new or existing VNet with customizable addresses. 
        /// Application pods are connected directly to the VNet, which allows for native integration with VNet features.
        /// The Azure CNI plugin requires an IP address from the subnet below for each pod on a node, 
        /// which can more quickly exhaust available IP addresses if a high value is set for pods per node.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="ipAddressRange">A CIDR notation IP range from which to assign service cluster IPs. It must not overlap with any Subnet IP ranges. For example: 10.0.0.0/16.</param>
        /// <param name="dnsIpAddress">An IP address assigned to the Kubernetes DNS service. It must be within the Kubernetes service address range. For example: 10.0.0.10.</param>
        /// <param name="dockerBridgeAddress">An IP address and netmask assigned to Docker Bridge. It must not be in any Subnet IP ranges, or the Kubernetes service address range. For example: 172.17.0.1/16.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithAzureCNI(this ManagedClusterBuilder builder, Input<string> ipAddressRange, Input<string> dnsIpAddress,
            Input<string> dockerBridgeAddress)
        {
            builder.NetworkProfile.NetworkPlugin = NetworkPlugin.Azure;
            builder.NetworkProfile.ServiceCidr = ipAddressRange;
            builder.NetworkProfile.DnsServiceIP = dnsIpAddress;
            builder.NetworkProfile.DockerBridgeCidr = dockerBridgeAddress;
            return builder;
        }

        /// <summary>
        /// Network policies allow you to define rules for ingress and egress traffic between pods in a cluster, 
        /// improving your cluster security by restricting access to certain pods. 
        /// You can choose between Calico Network Policies and Azure Network Policies for your cluster. 
        /// Network policies will only apply to Linux node pools.
        /// Must use Azure CNI to be able to use the Azure network policy.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="networkPolicy"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder NetworkPolicy(this ManagedClusterBuilder builder, InputUnion<string, NetworkPolicy> networkPolicy)
        {
            builder.NetworkProfile.NetworkPolicy = Pulumi.AzureNative.ContainerService.NetworkPolicy.;
            return builder;
        }

        public static ManagedClusterBuilder NetworkProfile(this ManagedClusterBuilder builder)
        {
            builder.Arguments.NetworkProfile = new ContainerServiceNetworkProfileArgs
            {
                LoadBalancerSku = LoadBalancerSku.Standard, //  Specifies the SKU of the Load Balancer used for this Kubernetes Cluster. Possible values are Basic and Standard. Defaults to Standard.
                LoadBalancerProfile = new ManagedClusterLoadBalancerProfileArgs // A load_balancer_profile block. This can only be specified when load_balancer_sku is set to Standard
                {
                    AllocatedOutboundPorts = 0, // Number of desired SNAT port for each VM in the clusters load balancer. Must be between 0 and 64000 inclusive. Defaults to 0.
                    EffectiveOutboundIPs = new InputList<ResourceReferenceArgs>
                    {
                        new ResourceReferenceArgs { Id = "" }
                    },
                    IdleTimeoutInMinutes = 0, // Desired outbound flow idle timeout in minutes for the cluster load balancer. Must be between 4 and 120 inclusive. Defaults to 30.
                    ManagedOutboundIPs = new ManagedClusterLoadBalancerProfileManagedOutboundIPsArgs
                    {
                        Count = 0 // Count of desired managed outbound IPs for the cluster load balancer. Must be between 1 and 100 inclusive.
                    },
                    OutboundIPPrefixes = new ManagedClusterLoadBalancerProfileOutboundIPPrefixesArgs
                    {
                        PublicIPPrefixes = new InputList<ResourceReferenceArgs>
                        {
                            new ResourceReferenceArgs { Id = "" } // The ID of the outbound Public IP Address Prefixes which should be used for the cluster load balancer.
                        }
                    },
                    OutboundIPs = new ManagedClusterLoadBalancerProfileOutboundIPsArgs
                    {
                        PublicIPs = new InputList<ResourceReferenceArgs>
                        {
                            new ResourceReferenceArgs { Id = "" } // he ID of the Public IP Addresses which should be used for outbound communication for the cluster load balancer.
                        }
                    },
                },
                NetworkMode = NetworkMode.Transparent, // Might be useless. Can only be set to 'bridge' for existing clusters and cannot be used to provision new clusters. Can only be set when NetworkPlugin is set to 'azure'.
                OutboundType = OutboundType.LoadBalancer, // The outbound (egress) routing method which should be used for this Kubernetes Cluster. Defaults to 'loadbalancer'
            };
            return builder;
        }

        /// <summary>
        /// Virtual nodes enable network communication between pods that run in Azure Container Instances (ACI) and the AKS cluster.
        /// Enabling virtual nodes allows you to deploy or burst out containers to nodes backed by serverless Azure Container Instances. 
        /// This can provide fast burst scaling options beyond your defined cluster size.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="subnetName">The subnet name for the virtual nodes to run.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableVirtualNodes(this ManagedClusterBuilder builder, Input<string> subnetName)
        {
            builder.AddonProfiles.Add("", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
                Config = new InputMap<string>
                {
                    { "subnet_name", subnetName }
                }
            });
            return builder;
        }

        /// <summary>
        /// The Application Gateway Ingress Controller (AGIC) is a Kubernetes application, which makes it possible for 
        /// Azure Kubernetes Service (AKS) customers to leverage Azure's native Application Gateway L7 load-balancer to expose cloud software to the Internet.
        /// See <see href="https://docs.microsoft.com/en-us/azure/application-gateway/ingress-controller-overview">this link</see> and 
        /// <see href="https://docs.microsoft.com/en-us/azure/application-gateway/tutorial-ingress-controller-add-on-existing">this link</see> for further details.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="gatewayId">The ID of the Application Gateway to integrate with the ingress controller of this Kubernetes Cluster. 
        /// See <see href="https://docs.microsoft.com/en-us/azure/application-gateway/tutorial-ingress-controller-add-on-existing">this link</see> for further details.</param>
        /// <param name="subnetAddressRange">The subnet CIDR to be used to create an Application Gateway, which in turn will be integrated with the ingress controller of this Kubernetes Cluster. 
        /// See <see href="https://docs.microsoft.com/en-us/azure/application-gateway/tutorial-ingress-controller-add-on-existing">this link</see> for further details.</param>
        /// <param name="subnetID">The ID of the subnet on which to create an Application Gateway, which in turn will be integrated with the ingress controller of this Kubernetes Cluster.
        /// See <see href="https://docs.microsoft.com/en-us/azure/application-gateway/tutorial-ingress-controller-add-on-existing">this link</see> for further details.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableIngressAppGateway(this ManagedClusterBuilder builder, Input<string> gatewayId = null, Input<string> subnetAddressRange = null, Input<string> subnetID = null)
        {
            builder.AddonProfiles.Add("ingress_application_gateway", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
                Config = new InputMap<string>
                {
                    { "gateway_id", gatewayId },
                    { "subnet_cidr", subnetAddressRange},
                    { "subnet_id", subnetID },
                }
            });
            return builder;
        }

        /// <summary>
        /// Container insights is a feature designed to monitor the performance of container workloads.
        /// Container insights gives you performance visibility by collecting memory and processor metrics from 
        /// controllers, nodes, and containers that are available in Kubernetes through the Metrics API. 
        /// Metrics are written to the metrics store and log data is written to the logs store associated with your Log Analytics workspace.
        /// See <see href="https://docs.microsoft.com/en-us/azure/azure-monitor/containers/container-insights-overview">this link</see> for further details.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logAnalyticsWorkspaceId">Resource ID for the Log Analytics workspace to store monitoring data.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableContainerInsights(this ManagedClusterBuilder builder, Input<string> logAnalyticsWorkspaceId)
        {
            builder.AddonProfiles.Add("oms_agent", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
                Config = new InputMap<string>
                {
                    { "log_analytics_workspace_id", logAnalyticsWorkspaceId },
                }
            });
            return builder;
        }

        /// <summary>
        /// Kubernetes includes a web dashboard that can be used for basic management operations. 
        /// This dashboard lets you view basic health status and metrics for your applications, create and deploy services, and edit existing applications.
        /// WARNING: The Kubernetes dashboard is enabled by default for clusters running a Kubernetes version less than 1.18.
        /// Though, the Kubernetes dashboard add-on will be disabled by default for all new clusters created on Kubernetes 1.18 or greater. 
        /// Starting with Kubernetes 1.19 in preview, AKS will no longer support installation of the managed kube-dashboard add-on.
        /// See <see href="https://docs.microsoft.com/en-us/azure/aks/kubernetes-dashboard">this link</see> for further details.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableKubeDashboard(this ManagedClusterBuilder builder)
        {
            builder.AddonProfiles.Add("kube_dashboard", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
            });
            return builder;
        }

        /// <summary>
        /// The HTTP application routing solution makes it easy to access applications that are deployed to your Azure Kubernetes Service (AKS) cluster. 
        /// When the solution is enabled, it configures an Ingress controller in your AKS cluster.
        /// As applications are deployed, the solution also creates publicly accessible DNS names for application endpoints.
        /// Also, when enabled, a DNS Zone is created in your subscription.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableHttpAppRouting(this ManagedClusterBuilder builder)
        {
            builder.AddonProfiles.Add("http_application_routing", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
            });
            return builder;
        }

        /// <summary>
        /// Azure Policy makes it possible to manage and report on the compliance state of your Kubernetes clusters from one place. 
        /// Azure Policy for Kubernetes only supports Linux node pools and built-in policy definitions. Built-in policy definitions are in the Kubernetes category. 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableAzurePolicy(this ManagedClusterBuilder builder)
        {
            builder.AddonProfiles.Add("azure_policy", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
            });
            return builder;
        }

        /// <summary>
        /// A private cluster uses an internal IP address to ensure that network traffic between the API server and node pools remains on a private network only.
        /// See <see href="https://docs.microsoft.com/en-us/azure/aks/private-clusters#configure-private-dns-zone">this link</see> for information on configuring Private DNS Zone.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="privateDnsZone">There are three valid values: resource ID of Private DNS Zone, 'System', and 'None'. </param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnablePrivateCluster(this ManagedClusterBuilder builder, Input<string> privateDnsZone = null)
        {
            builder.SecurityProfileArgs.EnablePrivateCluster = true;
            builder.SecurityProfileArgs.PrivateDNSZone = privateDnsZone;
            return builder;
        }

        /// <summary>
        /// Secure access to the API server using authorized IP address ranges.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="ipRanges">Authorized IP ranges to Kubernetes API server. Add more IP ranges using a comma seperated list of arguments.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder SetAuthorizedIpRanges(this ManagedClusterBuilder builder, params Input<string>[] ipRanges)
        {
            builder.SecurityProfileArgs.AuthorizedIPRanges = ipRanges;
            return builder;
        }

        /// <summary>
        /// Profile for Windows VMs in the managed cluster.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="username">Specifies the name of the administrator account.</param>
        /// <param name="password">Specifies the password of the administrator account.</param>
        /// <param name="licenseType">The license type to use for Windows VMs. Valid values: 'Windows_Server' and 'None'. Windows server is used to enable Azure Hybrid User Benefits for WIndows VMs.</param>
        /// <param name="enableCSIProxy">Container Storage Interface (CSI) is the standard for exposing block and file storage to containerized worloads on Kubernetes. 
        /// Setting this to true enables node plugins to perform privileged storage operations on nodes. 
        /// See <see href="https://kubernetes.io/blog/2020/04/03/kubernetes-1-18-feature-windows-csi-support-alpha/">this link</see> for more information.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithWindows(this ManagedClusterBuilder builder, Input<string> username, Input<string> password,
            InputUnion<string, LicenseType> licenseType = null, Input<bool> enableCSIProxy = null)
        {
            builder.Arguments.WindowsProfile = new ManagedClusterWindowsProfileArgs
            {
                AdminUsername = username,
                AdminPassword = password,
                LicenseType = licenseType,
                EnableCSIProxy = enableCSIProxy
            };
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="username"></param>
        /// <param name="elipCurve"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder WithLinuxNewECKey(this ManagedClusterBuilder builder, Input<string> username = null, Input<string> elipCurve = null)
        {
            var sshKey = new PrivateKey($"{builder.Arguments.ResourceName}-ssh-key", new PrivateKeyArgs
            {
                Algorithm = "ECDSA",
                EcdsaCurve = elipCurve ?? "P224"
            });

            builder.Arguments.LinuxProfile = new ContainerServiceLinuxProfileArgs
            {
                AdminUsername = username ?? "stize",
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
        public static ManagedClusterBuilder WithLinuxNewRsaKey(this ManagedClusterBuilder builder, Input<string> username = null, int rsaBits = 4096)
        {
            var sshKey = new PrivateKey($"{builder.Arguments.ResourceName}-ssh-key", new PrivateKeyArgs
            {
                Algorithm = "RSA",
                RsaBits = rsaBits
            });

            builder.Arguments.LinuxProfile = new ContainerServiceLinuxProfileArgs
            {
                AdminUsername = username ?? "stize",
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