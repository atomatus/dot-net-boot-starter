using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Com.Atomatus.Bootstarter.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConsoleApp1
{
    public class Client : AuditableModel<long>
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDbContextAsSqlServer(s => s.AddServiceTo<Client>());

            var provider = services.BuildServiceProvider();
            var service  = provider.GetService<IServiceCrud<Client, long>>();
            var list = service.List();

            Console.WriteLine("Hello World!");
        }
    }
}
