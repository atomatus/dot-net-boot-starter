
using Microsoft.EntityFrameworkCore;
using System;

namespace Com.Atomatus.Bootstarter.Context.Configuration.Connection
{
    public static class ContextConnectionSqlServerExtensions
    {
        private static bool OnBuildAsSqlServerCallback(ContextConnection.Builder builder, out ContextConnection conn)
        {
            bool toBuild = builder.IsDatabaseType(ContextConnectionParameters.DatabaseTypes.SqlServer);
            conn = toBuild ? new ContextConnectionSqlServer(builder) : null;
            return toBuild;
        }

        public static ContextConnection.Builder AsSqlServer(this ContextConnection.Builder context)
        {
            return context
                .DatabaseType(ContextConnection.DatabaseTypes.SqlServer)
                .AddBuildCallback(OnBuildAsSqlServerCallback);
        }

        public static DbContextOptionsBuilder BuildContextOptionsAsSqlServer(this ContextConnection.Builder context, IServiceProvider provider, DbContextOptionsBuilder options)
        {
            return options.UseSqlServer(context
                .Configuration(provider)
                .AsSqlServer()
                .Build());
        }
    }
}
