using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Atomatus.Bootstarter.Context
{
	public static class ContextBaseExtensions
	{
        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        /// <typeparam name="TContext">context base target</typeparam>
        /// <param name="provider">current service provider</param>
        /// <returns>current service provider</returns>
        public static IServiceProvider RequireMigration<TContext>(this IServiceProvider provider)
            where TContext : ContextBase
		{
            using (var scope = provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TContext>();
                context.Database.Migrate();
            }
            return provider;
        }
	}
}
