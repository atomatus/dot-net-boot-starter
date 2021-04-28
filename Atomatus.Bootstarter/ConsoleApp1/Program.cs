using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Com.Atomatus.Bootstarter.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConsoleApp1
{
    public class Client : AuditableModel<long>
    {
        public int Age { get; set; }

        public string Name { get; set; }
    }

    public class Test : AuditableModel<long>
    {
        public string Desc { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection()
                .AddDbContextAsSqlServer(
                    b => b.Database("dbTest")
                          .EnsureCreated()
                          .EnsureDeletedOnDispose(),
                    s => s.AddServiceTo<Client>()
                          .AddServiceTo<Test>());

            ServiceProvider provider = services.BuildServiceProvider();
            var service = provider.GetServiceTo<Client, long>();
            var list = service.List();

            var service1 = provider.GetServiceTo<Test, long>();
            var list1 = service1.List();

            Console.WriteLine("Hello World!");

            provider.Dispose();
        }
    }
}
