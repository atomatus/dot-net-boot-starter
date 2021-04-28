using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Register given Context to services as target database connection parameter options.
    /// </summary>
    internal static class ContextConnectionExtensions
    {
        private static void CheckAndWriteNoBuilderActionWarning<TContext>(
            bool noBuilderAction,
            [NotNull] string memberName,
            [NotNull] string filePath)
        {
            if (noBuilderAction)
            {
                Type cType = typeof(TContext);
                string dbTypeName = memberName.Replace(nameof(AddDbContextAs), "");                
                
                ConsoleLog
                    .Warn()
                    .Console()
                    .Debug()
                    .WriteLine($"The DbContext ({cType.Name}) Service was defined to start as \"{dbTypeName}\"")
                    .WriteLine($"in \"IServiceCollection#{memberName}\", however, no construction value has")
                    .WriteLine("been defined (in builderAction parameter). Therefore, the connection will be")
                    .WriteLine("initialized by the \"ConnectionString\" if defined it in \"appsettings.json\",")
                    .WriteLine("otherwise using the standard connection parameters according to the")
                    .WriteLine($"{dbTypeName} supplier's documentation.")
                #if DEBUG
                    .Write("- Path:      ")
                    .WriteLine(filePath, ConsoleColor.Blue)
                    .Write("- Operation: ")
                    .WriteLine(memberName, ConsoleColor.Blue)
                #endif
                    .Dispose();
            }
        }

        internal static IServiceCollection AddDbContextAs<TContext>(
            [NotNull] this IServiceCollection services,
            [NotNull] Func<ContextConnection.Builder, IServiceProvider, DbContextOptionsBuilder, DbContextOptionsBuilder> dbContextOptionsBuilderCallback,
            [AllowNull] Action<ContextConnection.Builder> builderAction = null,
            [AllowNull] Action<IContextServiceCollection> serviceAction = null,
            [NotNull] ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            [NotNull] ServiceLifetime optionsLifetime = ServiceLifetime.Scoped,
            [AllowNull, CallerMemberName] string memberName = null,
            [AllowNull, CallerFilePath] string filePath = null) 
            where TContext : ContextBase
        {
            CheckAndWriteNoBuilderActionWarning<TContext>(builderAction is null, memberName, filePath);
            
            var service = new ContextServiceCollection<TContext>(services);
            serviceAction?.Invoke(service);
            service.Dispose();

            return services.AddDbContext<TContext>(
                optionsAction: (prov, opt) => 
                {
                    var builder = new ContextConnection.Builder().Database<TContext>();
                    builderAction?.Invoke(builder);
                    dbContextOptionsBuilderCallback.Invoke(builder, prov, opt);
                },
                contextLifetime: contextLifetime,
                optionsLifetime: optionsLifetime);
        }
        
        internal static IServiceCollection AddDbContextAs(
            [NotNull] this IServiceCollection services,
            [NotNull] Func<ContextConnection.Builder, IServiceProvider, DbContextOptionsBuilder, DbContextOptionsBuilder> dbContextOptionsBuilderCallback,
            [AllowNull] Action<ContextConnection.Builder> builderAction = null,
            [AllowNull] Action<IContextServiceCollection> serviceAction = null,
            [NotNull] ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            [NotNull] ServiceLifetime optionsLifetime = ServiceLifetime.Scoped,
            [AllowNull, CallerMemberName] string memberName = null,
            [AllowNull, CallerFilePath] string filePath = null)
        {
            if(serviceAction is null)
            {
                throw new InvalidOperationException("Service action not set!\n" +
                    "When using DbContext generation by dynamic type, you must set " +
                    "services in serviceAction parameter to identify and recover all entities " +
                    "target binding to dbContext.");
            }

            using (var service = new ContextServiceTypeCollection())
            {
                var builder = new ContextConnection.Builder();
                builderAction?.Invoke(builder);

                serviceAction.Invoke(service);

                using (var ctBuilder = new ContextTypeBuilder(builder, service))
                {
                    Type dbContextType = ctBuilder.Build();
                    typeof(ContextConnectionExtensions)
                        .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                        .First(m => m.Name == nameof(AddDbContextAs) && m.IsGenericMethod)
                        .MakeGenericMethod(dbContextType)
                        .Invoke(null, new object[] {
                            services,
                            dbContextOptionsBuilderCallback,
                            builderAction,
                            serviceAction,
                            contextLifetime,
                            optionsLifetime,
                            memberName,
                            filePath
                        });

                    return services;
                }
            }
        }
    }
}
