using System;
using Pulumi;
using Stize.Infrastructure.Azure;
using Stize.Infrastructure.Azure.Sql;

namespace Stize.Infrastructure.Tests.Azure.Sql.Stacks
{
    public class DatabaseBasicStack : Stack
    {
        public DatabaseBasicStack()
        {
            var rg = new ResourceGroupBuilder("rg1")
                .Name("rg1")
                .Location("westeurope")
                .Build();

            var server = new SqlServerBuilder("sql1")
                .Name("my-server")
                .Location("westeurope")
                .ResourceGroup(rg)
                .AdministratorLogin("admin")
                .AdministratorPassword("stize")
                .Parent(rg)
                .Build();

            var db = new SqlDatabaseBuilder("db1")
                .Server(server.Name)
                .ResourceGroup(rg.Name)
                .Location(server.Location)
                .Name("my-db")
                .Parent(server)
                .SkuTier("Basic")
                .SkuServiceObjectiveName("S0")
                .Build();
        }
    }
}
