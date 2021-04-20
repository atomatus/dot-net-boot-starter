using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
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
                string prefix = "[Warning]: ";
                string message = $"builderAction was not set to " +
                    $"{dbTypeName} dbContext build at \"IServiceCollection#{memberName}\" request.\r\n" +
                    "Therefore will be used the default connection parameters to " +
                    $"build the DbContext ({cType.Name}).\r\n";

                Debug.Write($"{prefix}{message}");

                #if DEBUG
                Debug.Write(
                    $"\t- Path\t\t: {filePath}\r\n" +
                    $"\t- Operation\t: {memberName}\r\n");
                #endif

                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(prefix);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(message);
                Console.ForegroundColor = color;
                #if DEBUG
                Console.Write(" - Path\t\t: ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(filePath);
                Console.ForegroundColor = color;
                Console.Write(" - Operation\t: ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(memberName);
                #endif
                Console.ForegroundColor = color;
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
            var builder = new ContextConnection.Builder().Database<TContext>();
            builderAction?.Invoke(builder);

            var service = new ContextServiceCollection<TContext>(services);
            serviceAction?.Invoke(service);
            service.Dispose();

            return services.AddDbContext<TContext>(
                optionsAction: (prov, opt) => dbContextOptionsBuilderCallback.Invoke(builder, prov, opt),
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
                serviceAction.Invoke(service);
                using (var ctBuilder = new ContextTypeBuilder(service))
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
