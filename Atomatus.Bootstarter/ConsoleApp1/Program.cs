using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Com.Atomatus.Bootstarter.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConsoleApp1
{
    public class Client : AuditableModel<long> { }
/*
    internal class LocalContext : ContextBase
    {
        public DbSet<Client> Clients { get; set; }
        
        public LocalContext(DbContextOptions<LocalContext> options) : base(options) { }
    }

    internal class LocalContext1 : ContextBase
    {
        public DbSet<Client> Clients { get; set; }

        public LocalContext1(DbContextOptions<LocalContext1> options) : base(options) { }
    }
*/
    public interface IClientService : IServiceCrud<Client, long> { }

    public interface IClientService2 : IServiceCrud<Client, long> { }

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddDbContextAsSqlServer(serviceAction: s => s
                    .AddScoped<IClientService>()
                    .AddScoped<IClientService2>());

            var provider = services.BuildServiceProvider();
            var service  = provider.GetService<IClientService>();
            var service2 = provider.GetService<IClientService2>();
            var list = service.List();            
            var c = service.Insert(new Client { });
            list = service.List();

            Console.WriteLine("Hello World!");
        }
    }
}
