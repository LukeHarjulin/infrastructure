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
        /// Dns name prefix for the Managed Cluster.
        /// For example, "mc1-dns".
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
        /// Enables Kubernetes Role-Based Access Control.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableRBAC(this ManagedClusterBuilder builder)
        {
            builder.Arguments.EnableRBAC = true;
            return builder;
        }

        /// <summary>
        /// Getting static credential will be disabled for this cluster. Expected to only be used for AAD clusters.
        /// Requires preview mode.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder DisableLocalAccounts(this ManagedClusterBuilder builder)
        {
            builder.Arguments.DisableLocalAccounts = true;
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
        /// Construct an agent node pool to the Managed Cluster. After calling this method, a new
        /// <see cref="AgentPoolBuilder"/> is returned, providing access to all of the <see
        /// cref="AgentPoolExtensions"/>. Once the agent pool is finished, call the .Build() method
        /// and the current <see cref="ManagedClusterBuilder"/> will be returned. For example, new
        /// ManagedClusterBuilder("mc1").Name("mc1") ... .AddAgentPool().Name("ap1").Build() ... .Build();
        /// </summary>
        /// <param name="clusterBuilder"></param>
        /// <returns></returns>
        public static AgentPoolBuilder AddAgentPool(this ManagedClusterBuilder clusterBuilder)
        {
            var builder = new AgentPoolBuilder(clusterBuilder); 
            /// Might be a better method. Require user to pass in AgentPoolBuilder
            /// for example:
            /// AddAgentPool(new AgentPoolBuilder()
            ///     .Name("ap1")
            ///     .NodeCount(4)
            ///     )
            return builder;
        }

        /// <summary>
        /// Set the service principal profile of the Managed Cluster with an existing service
        /// principal, using its client ID and the secret.
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
        /// %%% NEEDS FIX %%%
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
                UserAssignedIdentities = new InputMap<object> { { userId, userName} }
                /// This stupid InputMap input type doesn't work. 
                /// When Id is provided it throws an error, when you set it to null (like it asks) it throws another error.
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
        public static ManagedClusterBuilder EnableAKSManagedAzureAD(this ManagedClusterBuilder builder, Input<string> tenantId = null, params Input<string>[] adminGroupObjectIDs)
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
        /// Using Kubernetes role-based access control provides fine-grained control over cluster resources.
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
        /// The outbound (egress) routing method which should be used for this Kubernetes Cluster. Defaults to 'loadbalancer'.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="outboundType">The outbound (egress) routing method which should be used for this Kubernetes Cluster. Defaults to 'loadbalancer'.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder OutboundType(this ManagedClusterBuilder builder, InputUnion<string, OutboundType> outboundType)
        {
            builder.NetworkProfile.OutboundType = outboundType;
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
        /// <param name="networkPolicy">The Network Policy that defines the rules for ingress and egress traffic between pods in a cluster. Valid values: 'Calico' and 'Azure'.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder NetworkPolicy(this ManagedClusterBuilder builder, InputUnion<string, NetworkPolicy> networkPolicy)
        {
            builder.NetworkProfile.NetworkPolicy = networkPolicy;
            return builder;
        }

        /// <summary>
        /// The Load Balancer Sku for the Managed Cluster.
        /// An Azure Load Balancer routes and balances traffic to your Kubernetes cluster. 
        /// The 'Standard' SKU, which has expanded capabilities compared to the 'Basic' SKU, is the default SKU.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sku"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder LoadBalancerSku(this ManagedClusterBuilder builder, InputUnion<string, LoadBalancerSku> sku)
        {
            builder.NetworkProfile.LoadBalancerSku = sku;
            return builder;
        }

        /// <summary>
        /// Set the number of managed outbound IPs for the cluster Load Balancer. Value must be between 1 and 100 inclusive.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="count">Set the number of managed outbound IPs for the cluster Load Balancer. Value must be between 1 and 100 inclusive.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder LoadBalancerManagedOutboundIpCount(this ManagedClusterBuilder builder, Input<int> count)
        {
            builder.LoadBalancerProfile.ManagedOutboundIPs = new ManagedClusterLoadBalancerProfileManagedOutboundIPsArgs
            {
                Count = count
            };
            return builder;
        }

        /// <summary>
        /// The ID of the Public IP Addresses which should be used for outbound communication for the cluster load balancer.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="outboundIpIDs">The ID of the Public IP Addresses which should be used for outbound communication for the cluster load balancer.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder LoadBalancerOutboundIPs(this ManagedClusterBuilder builder, params Input<string>[] outboundIpIDs)
        {
            var publicIPs = new InputList<ResourceReferenceArgs>();
            foreach (var id in outboundIpIDs)
            {
                publicIPs.Add(new ResourceReferenceArgs { Id = id });
            }
            builder.LoadBalancerProfile.OutboundIPs = new ManagedClusterLoadBalancerProfileOutboundIPsArgs { PublicIPs = publicIPs };

            // NEED TO FIND OUT THE DIFFERENCE BETWEEN 'OutboundIPs' and 'EffectiveOutboundIPs'!!!
            // https://www.pulumi.com/docs/reference/pkg/azure-native/containerservice/managedcluster/#managedclusterloadbalancerprofile
            return builder;
        }

        /// <summary>
        /// The ID of the outbound Public IP Address Prefixes which should be used for the cluster load balancer.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="outboundPrefixIds"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder LoadBalancerOutboundPrefixes(this ManagedClusterBuilder builder, params Input<string>[] outboundPrefixIds)
        {
            var publicPrefixes = new InputList<ResourceReferenceArgs>();
            foreach (var id in outboundPrefixIds)
            {
                publicPrefixes.Add(new ResourceReferenceArgs { Id = id });
            }
            builder.LoadBalancerProfile.OutboundIPPrefixes = new ManagedClusterLoadBalancerProfileOutboundIPPrefixesArgs { PublicIPPrefixes = publicPrefixes };
            return builder;
        }

        /// <summary>
        /// Number of desired SNAT port for each VM in the clusters load balancer. Must be between 0 and 64000 inclusive. Defaults to 0.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="ports">Number of desired SNAT port for each VM in the clusters load balancer. Must be between 0 and 64000 inclusive. Defaults to 0.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder LoadBalancerAllocatedPorts(this ManagedClusterBuilder builder, Input<int> ports)
        {
            builder.LoadBalancerProfile.AllocatedOutboundPorts = ports;
            return builder;
        }

        /// <summary>
        /// Desired outbound flow idle timeout in minutes for the cluster load balancer. Must be between 4 and 120 inclusive. Defaults to 30.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="minutes">Desired outbound flow idle timeout in minutes for the cluster load balancer. Must be between 4 and 120 inclusive. Defaults to 30.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder LoadBalancerIdleTimeout(this ManagedClusterBuilder builder, Input<int> minutes)
        {
            builder.LoadBalancerProfile.IdleTimeoutInMinutes = minutes;
            return builder;
        }


        /// <summary>
        /// %%% NEEDS FIX %%%
        /// Virtual nodes enable network communication between pods that run in Azure Container Instances (ACI) and the AKS cluster.
        /// Enabling virtual nodes allows you to deploy or burst out containers to nodes backed by serverless Azure Container Instances. 
        /// This can provide fast burst scaling options beyond your defined cluster size.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="subnetName">The subnet name for the virtual nodes to run.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableVirtualNodes(this ManagedClusterBuilder builder, Input<string> subnetName)
        {
            builder.AddonProfiles.Add("aciConnectorLinux", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
                Config = new InputMap<string>
                {
                    { "subnetName", subnetName }
                }
            });
            return builder;
        }

        /// <summary>
        /// The name of the resource group that will contain all of the agent pool nodes and 
        /// other additional resources that are created (i.e. Public IPs, Vnets).
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="resourceGroupName">The name of the resource group that will contain all of the agent pool nodes.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder AddedResourceGroupName(this ManagedClusterBuilder builder, Input<string> resourceGroupName)
        {
            builder.Arguments.NodeResourceGroup = resourceGroupName;
            return builder;
        }

        /// <summary>
        /// The Application Gateway Ingress Controller (AGIC) is a Kubernetes application, which makes it possible for 
        /// Azure Kubernetes Service (AKS) customers to leverage Azure's native Application Gateway L7 load-balancer to expose cloud software to the Internet.
        /// See <see href="https://docs.microsoft.com/en-us/azure/application-gateway/ingress-controller-overview">this link</see> and 
        /// <see href="https://docs.microsoft.com/en-us/azure/application-gateway/tutorial-ingress-controller-add-on-existing">this link</see> for further details.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="appGatewayName">The Name of the new Application Gateway to integrate with the ingress controller of this Kubernetes Cluster. 
        /// See <see href="https://docs.microsoft.com/en-us/azure/application-gateway/tutorial-ingress-controller-add-on-existing">this link</see> for further details.</param>
        /// <param name="subnetAddressRange">The subnet CIDR to be used to create an Application Gateway, which in turn will be integrated with the ingress controller of this Kubernetes Cluster. 
        /// See <see href="https://docs.microsoft.com/en-us/azure/application-gateway/tutorial-ingress-controller-add-on-existing">this link</see> for further details.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableIngressWithNewAppGateway(this ManagedClusterBuilder builder, Input<string> appGatewayName, Input<string> subnetAddressRange)
        {
            builder.AddonProfiles.Add("ingressApplicationGateway", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
                Config = new InputMap<string>
                {
                    { "applicationGatewayName", appGatewayName},
                    { "subnetPrefix", subnetAddressRange },
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
        /// <param name="appGatewayId">The ID of an existing Application Gateway, which in turn will be integrated with the ingress controller of this Kubernetes Cluster.
        /// See <see href="https://docs.microsoft.com/en-us/azure/application-gateway/tutorial-ingress-controller-add-on-existing">this link</see> for further details.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableIngressWithExistingAppGateway(this ManagedClusterBuilder builder, Input<string> appGatewayId)
        {
            builder.AddonProfiles.Add("ingressApplicationGateway", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
                Config = new InputMap<string>
                {
                    { "applicationGatewayId", appGatewayId},
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
            builder.AddonProfiles.Add("omsAgent", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
                Config = new InputMap<string>
                {
                    { "logAnalyticsWorkspaceResourceID", logAnalyticsWorkspaceId },
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
            builder.AddonProfiles.Add("kubeDashboard", new ManagedClusterAddonProfileArgs
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
            builder.AddonProfiles.Add("httpApplicationRouting", new ManagedClusterAddonProfileArgs
            {
                Enabled = true,
            });
            return builder;
        }

        /// <summary>
        /// Azure Policy makes it possible to manage and report on the compliance state of your Kubernetes clusters from one place. 
        /// Azure Policy for Kubernetes only supports Linux node pools and built-in policy definitions. Built-in policy definitions are in the Kubernetes category. 
        /// 
        /// %%% CURRENTLY BROKEN %%%
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder EnableAzurePolicy(this ManagedClusterBuilder builder)
        {
            builder.AddonProfiles.Add("azurePolicy", new ManagedClusterAddonProfileArgs 
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
        /// %%% NEEDS FIX %%%
        /// Sets up the profile for the Linux VMs in the Managed Cluster, setting the username as specified (default value is "stize") and 
        /// generating a new Elliptic Curve private key (default EC is 'P224') for SSH.
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
        /// Sets up the profile for the Linux VMs in the Managed Cluster, setting the username as specified (default value is "stize") and 
        /// generating a new RSA private key (default RSA bits is 4096) for SSH.
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

        /// <summary>
        /// The upgrade channel for this Kubernetes Cluster.
        /// Valid values: 'Rapid', 'Stable', 'Patch', 'Node_image', and 'None'.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="channel">The upgrade channel for this Kubernetes Cluster. Valid values: 'Rapid', 'Stable', 'Patch', 'Node_image', and 'None'.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder AutoUpgradeChannel(this ManagedClusterBuilder builder, InputUnion<string, UpgradeChannel> channel)
        {
            builder.Arguments.AutoUpgradeProfile = new ManagedClusterAutoUpgradeProfileArgs { UpgradeChannel = channel };
            return builder;
        }


        /// Need to figure out a good way to add AutoScaler properties.
        /// There's a lot.
        /// https://www.pulumi.com/docs/reference/pkg/azure-native/containerservice/managedcluster/#managedclusterpropertiesautoscalerprofile
        /// <summary>
        /// Detect similar node groups and balance the number of nodes between them. Defaults to false.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="isEnabled">Detect similar node groups and balance the number of nodes between them. Defaults to false.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder BalanceSimilarNodeGroups(this ManagedClusterBuilder builder, bool isEnabled)
        {
            builder.AutoScalerProfile.BalanceSimilarNodeGroups = isEnabled.ToString().ToLower();
            return builder;
        }

        /// <summary>
        /// Expander to use. Possible values are 'least-waste', 'priority', 'most-pods' and 'random'. Defaults to 'random'.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="expander">Expander to use. Possible values are 'least-waste', 'priority', 'most-pods' and 'random'. Defaults to 'random'.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ExpanderType(this ManagedClusterBuilder builder, InputUnion<string,Expander> expander)
        {
            builder.AutoScalerProfile.Expander = expander;
            return builder;
        }

        /// <summary>
        /// Maximum number of seconds the cluster autoscaler waits for pod termination when trying to scale down a node. Defaults to 600.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="seconds">Maximum number of seconds the cluster autoscaler waits for pod termination when trying to scale down a node. Defaults to 600.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder MaxWaitForPodTermination(this ManagedClusterBuilder builder, int seconds)
        {
            builder.AutoScalerProfile.MaxGracefulTerminationSec = seconds + "s";
            return builder;
        }

        /// <summary>
        /// Maximum time the autoscaler waits for a node to be provisioned. Defaults to 15m.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="minutes">Maximum time the autoscaler waits for a node to be provisioned. Defaults to 15m.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder MaxWaitForProvisioningNode(this ManagedClusterBuilder builder, int minutes)
        {
            builder.AutoScalerProfile.MaxNodeProvisionTime = minutes + "m";
            return builder;
        }

        /// <summary>
        /// Maximum Number of allowed unready nodes. Defaults to 3.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="numberOfNodes">Maximum Number of allowed unready nodes. Defaults to 3.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder MaxNumberOfUnreadyNodes(this ManagedClusterBuilder builder, int numberOfNodes)
        {
            builder.AutoScalerProfile.OkTotalUnreadyCount = numberOfNodes.ToString();
            return builder;
        }

        /// <summary>
        /// Maximum percentage of unready nodes the cluster autoscaler will stop if the percentage is exceeded. Defaults to 45.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="percentage">Maximum percentage of unready nodes the cluster autoscaler will stop if the percentage is exceeded. Defaults to 45.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder MaxPercentOfUnreadyNodes(this ManagedClusterBuilder builder, int percentage)
        {
            builder.AutoScalerProfile.MaxTotalUnreadyPercentage = percentage.ToString();
            return builder;
        }

        /// <summary>
        /// For scenarios like burst/batch scale where you don't want CA to act before the kubernetes scheduler could schedule all the pods, 
        /// you can tell CA to ignore unscheduled pods before they're a certain age. Defaults to 10s.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="seconds">For scenarios like burst/batch scale where you don't want CA to act before the kubernetes scheduler could schedule all the pods, 
        /// you can tell CA to ignore unscheduled pods before they're a certain age. Defaults to 10s.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder NewPodScaleUpDelay(this ManagedClusterBuilder builder, int seconds)
        {
            builder.AutoScalerProfile.NewPodScaleUpDelay = seconds + "s";
            return builder;
        }

        /// <summary>
        /// How long after the scale up of AKS nodes the scale down evaluation resumes. Defaults to 10m.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="minutes">How long after the scale up of AKS nodes the scale down evaluation resumes. Defaults to 10m.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ScaleDownDelayAfterAdd(this ManagedClusterBuilder builder, int minutes)
        {
            builder.AutoScalerProfile.ScaleDownDelayAfterAdd = minutes + "m";
            return builder;
        }

        /// <summary>
        /// How long after node deletion that scale down evaluation resumes. Defaults to the value used for ScanInterval.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="seconds">How long after node deletion that scale down evaluation resumes. Defaults to the value used for ScanInterval.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ScaleDownDelayAfterDelete(this ManagedClusterBuilder builder, int seconds)
        {
            builder.AutoScalerProfile.ScaleDownDelayAfterDelete = seconds + "s";
            return builder;
        }

        /// <summary>
        /// How long after scale down failure that scale down evaluation resumes. Defaults to 3m.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="minutes">How long after scale down failure that scale down evaluation resumes. Defaults to 3m.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ScaleDownDelayAfterFailure(this ManagedClusterBuilder builder, int minutes)
        {
            builder.AutoScalerProfile.ScaleDownDelayAfterFailure = minutes + "m";
            return builder;
        }

        /// <summary>
        /// How often the AKS Cluster should be re-evaluated for scale up/down. Defaults to 10s.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="seconds">How often the AKS Cluster should be re-evaluated for scale up/down. Defaults to 10s.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ScanInterval(this ManagedClusterBuilder builder, int seconds)
        {
            builder.AutoScalerProfile.ScanInterval = seconds + "s";
            return builder;
        }

        /// <summary>
        /// How long a node should be unneeded before it is eligible for scale down. Defaults to 10m.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="minutes">How long a node should be unneeded before it is eligible for scale down. Defaults to 10m.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ScaleDownUnneededNodes(this ManagedClusterBuilder builder, int minutes)
        {
            builder.AutoScalerProfile.ScaleDownUnneededTime = minutes + "m";
            return builder;
        }

        /// <summary>
        /// How long an unready node should be unneeded before it is eligible for scale down. Defaults to 20m.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="minutes">How long an unready node should be unneeded before it is eligible for scale down. Defaults to 20m.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ScaleDownUnreadyNodes(this ManagedClusterBuilder builder, int minutes)
        {
            builder.AutoScalerProfile.ScaleDownUnneededTime = minutes + "m";
            return builder;
        }

        /// <summary>
        /// Node utilization level, defined as sum of requested resources divided by capacity, below which a node can be considered for scale down. Defaults to 0.5.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="threshold">Node utilization level, defined as sum of requested resources divided by capacity, below which a node can be considered for scale down. Defaults to 0.5.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder ScaleDownUtilizationThreshold(this ManagedClusterBuilder builder, Input<string> threshold)
        {
            builder.AutoScalerProfile.ScaleDownUtilizationThreshold = threshold;
            return builder;
        }

        /// <summary>
        /// Maximum number of empty nodes that can be deleted at the same time. Defaults to 10.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="maxNodes">Maximum number of empty nodes that can be deleted at the same time. Defaults to 10.</param>
        /// <returns></returns>
        public static ManagedClusterBuilder MaxBulkDeleteOfNodes(this ManagedClusterBuilder builder, int maxNodes)
        {
            builder.AutoScalerProfile.MaxEmptyBulkDelete = maxNodes.ToString();
            return builder;
        }

        /// <summary>
        /// Allows the cluster autoscaler to delete nodes with pods that have local storage, when necessary.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder AllowDeletionOfNodesWithLocalStorage(this ManagedClusterBuilder builder)
        {
            builder.AutoScalerProfile.SkipNodesWithLocalStorage = "false";
            return builder;
        }

        /// <summary>
        /// Allows the cluster autoscaler to delete nodes with pods from Kube-System, when necessary.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ManagedClusterBuilder AllowDeletionOfNodesWithKubeSystem(this ManagedClusterBuilder builder)
        {
            builder.AutoScalerProfile.SkipNodesWithSystemPods = "false";
            return builder;
        }


        public static ManagedClusterBuilder EnablePodIdentityAddon(this ManagedClusterBuilder builder)
        {
            // NEEDS FINISHING
            builder.Arguments.PodIdentityProfile = new ManagedClusterPodIdentityProfileArgs
            {
            };
            return builder;
        }
    }
}