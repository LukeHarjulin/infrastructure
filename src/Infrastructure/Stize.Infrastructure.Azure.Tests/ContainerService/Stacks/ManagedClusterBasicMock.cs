using Pulumi.Testing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Stize.Infrastructure.Azure.Tests.ContainerService.Stacks
{
    public class ManagedClusterBasicMock : IMocks
    {

        public Task<(string? id, object state)> NewResourceAsync(MockResourceArgs args)
        {
            var outputs = ImmutableDictionary.CreateBuilder<string, object>();

            // Forward all input parameters as resource outputs, so that we could test them.
            outputs.AddRange(args.Inputs);

            // Default the resource ID to `{name}_id`.
            if (args.Id == null || args.Id == "")
            {
                args.Id = $"{args.Name}_id";
            }
            outputs.Add("id", args.Id);

            switch (args.Type)
            {
                case "azure-native:containerservice:ManagedCluster": return NewManagedCluster(args, outputs);
                default: return Task.FromResult((args.Id, (object)outputs));
            }
        }

        public Task<object> CallAsync(MockCallArgs args)
        {
            throw new NotImplementedException();
        }

        public Task<(string? id, object state)> NewManagedCluster(MockResourceArgs args, ImmutableDictionary<string, object>.Builder outputs)
        {
            outputs.Add("name", args.Inputs["resourceName"]);

            return Task.FromResult((args.Id, (object)outputs));
        }

    }
}
