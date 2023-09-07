# Using NoSQL Cosmos Database with LocalContext in ASP.NET Core

This README provides a step-by-step guide on how to use Cosmos DB as the database in an ASP.NET Core project's infrastructure layer. We'll create a `LocalContext` class that derives from `ContextBase`, which in turn derives from `DbContext`. This guide assumes you have basic knowledge of ASP.NET Core and Entity Framework Core.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) or another code editor of your choice.

## Installation

To get started, install the necessary NuGet packages for Entity Framework Core and SQLite support in your project. You can do this using the command-line interface (CLI) or the NuGet Package Manager in Visual Studio.

```shell
dotnet add package Atomatus.Bootstarter.Cosmos
```

## 1. Define the `LocalContext` Class

First, create your `LocalContext` class that inherits from `ContextBase`. This class will be responsible for configuring the Cosmos DB context.

```csharp
using Com.Atomatus.Bootstarter.Context;
using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Infrastructure
{
    public class LocalContext : ContextBase
    {
        public LocalContext(DbContextOptions<LocalContext> options) : base(options) { }

        // Define your entities as DbSet and configure the model here.
        public DbSet<YourEntity> YourEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your model here.
        }
    }
}
```

Be sure to replace YourNamespace with your project's actual namespace and configure your entities and model as needed.

## 2. Configure Dependency Injection
Next, configure dependency injection for the LocalContext in your project's Startup.cs file. This informs ASP.NET Core how to create instances of LocalContext when needed.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Com.Atomatus.Bootstarter.Context;

namespace YourNamespace
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // ...

            // Configure the Cosmos connection and register LocalContext
            services.AddDbContext<LocalContext>(options =>
            {
                options.UseCosmos(
                    accountEndpoint: Configuration["CosmosDb:AccountEndpoint"],
                    accountKey: Configuration["CosmosDb:AccountKey"],
                    databaseName: Configuration["CosmosDb:DatabaseName"]);
            });

            // ...

            services.AddTransient<YourRepository>(); // Example repository registration if needed.

            // ...
        }
    }
}
```

In the above code, replace the configuration keys CosmosDb:AccountEndpoint, CosmosDb:AccountKey, and CosmosDb:DatabaseName with the actual values from your appsettings.json or relevant configuration.

## 3. Using LocalContext in a Service

Now you can use the `LocalContext` in your services or controllers. Let's create a service named YourService that uses `LocalContext` to interact with Cosmos.

```csharp
using YourNamespace.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace YourNamespace.Services
{
    public class YourService
    {
        private readonly LocalContext _context;

        public YourService(LocalContext context)
        {
            _context = context;
        }

        public IEnumerable<YourEntity> GetAllEntities()
        {
            return _context.YourEntities;
        }

        public void AddEntity(YourEntity entity)
        {
            _context.YourEntities.Add(entity);
            _context.SaveChanges();
        }

        // Add other methods as needed.
    }
}
```

Remember to replace YourNamespace and YourEntity with your project's actual namespace and entity.

You have now successfully configured Cosmos DB as the database for LocalContext in an ASP.NET Core project's infrastructure layer. Make sure to set your Cosmos DB access keys in the appsettings.json file.

## License

BootStarter (including the runtime repository) is licensed under the [Apache](LICENSE) license.

---

© Atomatus - All Rights Reserveds.
