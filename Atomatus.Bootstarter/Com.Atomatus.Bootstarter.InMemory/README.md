# Using InMemory Database with LocalContext in ASP.NET Core

This guide demonstrates how to use the InMemory database as a data source for your `LocalContext`, which inherits from `ContextBase`, and ultimately derives from `DbContext` in an ASP.NET Core project's infrastructure layer.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) or another code editor of your choice.

## Installation

To get started, install the necessary NuGet packages for Entity Framework Core and SQLite support in your project. You can do this using the command-line interface (CLI) or the NuGet Package Manager in Visual Studio.

```shell
dotnet add package Atomatus.Bootstarter.InMemory
```

## 1. Define the Local Context Class

First, create your local context class that inherits from `ContextBase`. Assuming you already have a class called `LocalContext`, you can do the following:

```csharp
using Com.Atomatus.Bootstarter.Context;
using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Infrastructure
{
    public class LocalContext : ContextBase
    {
        public LocalContext(DbContextOptions<LocalContext> options) : base(options) { }

        // Define your DbSets and configure your model here.
    }
}
```

## 2. Configure Dependency Injection in Startup.cs

In your Startup.cs file, in the ConfigureServices method, configure the dependency injection for your InMemory database context. Make sure to register the LocalContext with the correct options.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace YourNamespace
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // ...

            // Configure the DbContext to use InMemory as the database
            services.AddDbContext<LocalContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDatabase"); // Name of the InMemory database
            });

            // ...
        }
    }
}
```

## 3. Use the LocalContext in Your Service or Controller

Now that you've configured your InMemory database context, you can use it in your services or controllers like this:

```csharp
using YourNamespace.Infrastructure;

namespace YourNamespace.Services
{
    public class YourService
    {
        private readonly LocalContext _context;

        public YourService(LocalContext context)
        {
            _context = context;
        }

        public void SomeOperation()
        {
            // You can use _context to interact with the InMemory database
            // For example: _context.YourDbSet.Add(entity);
            // ...

            _context.SaveChanges();
        }
    }
}
```

Replace "YourNamespace" with your actual project namespaces.

Make sure to add the necessary references to Entity Framework Core and ASP.NET Core's infrastructure assemblies in your project.

That's it! You've successfully configured your LocalContext to use InMemory as the database in your ASP.NET Core project's infrastructure layer.

## License

BootStarter (including the runtime repository) is licensed under the [Apache](LICENSE) license.

---

© Atomatus - All Rights Reserveds.
