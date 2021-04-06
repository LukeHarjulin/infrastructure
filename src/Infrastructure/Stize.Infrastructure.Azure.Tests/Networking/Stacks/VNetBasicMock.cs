﻿using Pulumi.Testing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Stize.Infrastructure.Azure.Tests.Networking.Stacks
{
    public class VNetBasicMock : IMocks
    {
        public Task<(string? id, object state)> NewResourceAsync(string type, string name, ImmutableDictionary<string, object> inputs, string? provider, string? id)
        {
            var outputs = ImmutableDictionary.CreateBuilder<string, object>();

            // Forward all input parameters as resource outputs, so that we could test them.
            outputs.AddRange(inputs);

            // Default the resource ID to `{name}_id`.
            if (id == null || id == "")
            {
                id = $"{name}_id";
            }
            outputs.Add("id", id);
            switch (type)
            {
                case "azure-native:network:VirtualNetwork": return NewVNet(type, name, inputs, provider, id, outputs);
                default: return Task.FromResult((id, (object)outputs));
            }
        }
        public Task<object> CallAsync(string token, ImmutableDictionary<string, object> inputs, string? provider)
        {
            // We don't use this method in this particular test suite.
            // Default to returning whatever we got as input.
            return Task.FromResult((object)inputs);
        }

        public Task<(string? id, object state)> NewVNet(string type, string name, ImmutableDictionary<string, object> inputs,
            string? provider, string? id, ImmutableDictionary<string, object>.Builder outputs)
        {
            outputs.Add("name", inputs["virtualNetworkName"]);

            return Task.FromResult((id, (object)outputs));
        }
    }
}
