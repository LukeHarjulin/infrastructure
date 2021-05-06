using System;
using System.Collections.Generic;
using System.Text;
using Pulumi;
using Pulumi.AzureNative.ContainerService;
using Pulumi.AzureNative.ContainerService.Inputs;
namespace Stize.Infrastructure.Azure.ContainerService
{
    public static class AgentPoolExtensions
    {
        public static AgentPoolBuilder Name(this AgentPoolBuilder builder, Input<string> name)
        {
            builder.Arguments.Name = name;
            return builder;
        }

        public static AgentPoolBuilder Count(this AgentPoolBuilder builder, Input<int> count)
        {
            builder.Arguments.Count = count;
            return builder;
        }

        public static AgentPoolBuilder Mode(this AgentPoolBuilder builder, InputUnion<string, AgentPoolMode> mode)
        {
            builder.Arguments.Mode = mode;
            return builder;
        }

        public static AgentPoolBuilder OsType(this AgentPoolBuilder builder, InputUnion<string, OSType> osType)
        {
            builder.Arguments.OsType = osType;
            return builder;
        }

        public static AgentPoolBuilder Type(this AgentPoolBuilder builder, InputUnion<string, AgentPoolType> type)
        {
            builder.Arguments.Type = type;
            return builder;
        }

        public static AgentPoolBuilder Type(this AgentPoolBuilder builder, Input<string> size)
        {
            builder.Arguments.VmSize = size;
            return builder;
        }
    }
}
