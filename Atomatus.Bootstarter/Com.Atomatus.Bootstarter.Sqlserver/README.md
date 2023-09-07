# Using MS SQL Server Database with LocalContext in ASP.NET Core

This repository provides a .NET application that simplifies the configuration and interaction with SQL Server databases using Entity Framework Core. It includes a custom context connection for SQL Server and extension methods to streamline the setup of DbContext instances for SQL Server.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) or another code editor of your choice.

## Installation

To get started, install the necessary NuGet packages for Entity Framework Core and SQLite support in your project. You can do this using the command-line interface (CLI) or the NuGet Package Manager in Visual Studio.

```shell
dotnet add package Atomatus.Bootstarter.Sqlserver
```

## `ContextConnectionSqlServer` Class

The `ContextConnectionSqlServer` class extends a base `ContextConnection` class and is used to configure the connection to a SQL Server database. It generates the connection string based on the provided options.

### `GetConnectionString` Method

This method constructs the connection string using a `StringBuilder`. It includes various parameters like server name, port, database name, user credentials, and other settings.

### `Attach` Method

The `Attach` method is responsible for attaching the SQL Server configuration to `DbContextOptionsBuilder` so that you can use it when configuring your DbContext.

## `ContextConnectionSqlServerExtensions` Class

The `ContextConnectionSqlServerExtensions` class provides extension methods for working with SQL Server configurations.

### `AsSqlServer` Method

This extension method is used to mark the builder to build a SQL Server context connection. It configures the builder to use SQL Server as the database provider.

### `AsSqlServerDbContextOptionsBuilder` Method

This method sets up the SQL Server context and attaches it to a `DbContextOptionsBuilder`. It's useful when you want to configure your DbContext with SQL Server options.

### `AddDbContextAsSqlServer` Methods

These methods are used to register a SQL Server DbContext service in the dependency injection container. They allow you to specify builder options and service actions, which can be used for configuring the DbContext and its services.

#### Overloads for Dynamic and Explicit DbContext Types

There are also overloads for these methods that allow you to work with both dynamic and explicit DbContext types.

This code provides a convenient way to configure and work with SQL Server databases in a .NET application using Entity Framework Core. It abstracts away much of the boilerplate code involved in setting up database connections and DbContext instances.

#### Configure the SQL Server Connection

In your application, you can configure the SQL Server connection using the provided `ContextConnectionSqlServer` class and the `AsSqlServer` extension method:

```csharp
using Com.Atomatus.Bootstarter.Context;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContextAsSqlServer<MyDbContext>(builder =>
            {
                builder
                    .Host("localhost")                  // Set your SQL Server host
                    .Port(1433)                        // Set your SQL Server port
                    .Database("YourDatabaseName")      // Set your database name
                    .User("YourUsername")              // Set your SQL Server username
                    .Password("YourPassword");         // Set your SQL Server password
            })
            .BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        // Now, you can use dbContext to interact with the SQL Server database.
        // Example: dbContext.Database.Migrate();
    }
}
```

In this example, we configure the SQL Server connection using the builder parameter in the `AddDbContextAsSqlServer` method. You can set the host, port, database name, username, and password as needed for your SQL Server instance.

#### Register DbContext as a Service

Register your DbContext as a service in the dependency injection container using the `AddDbContextAsSqlServer` method. This makes it available for use throughout your application:

```csharp
using Com.Atomatus.Bootstarter.Context;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextAsSqlServer<MyDbContext>(builder =>
        {
            builder
                .Host("localhost")
                .Port(1433)
                .Database("YourDatabaseName")
                .User("YourUsername")
                .Password("YourPassword");
        });
        
        // Additional services and configuration...
    }
}
```

Now, you can inject MyDbContext wherever you need it in your application, and Entity Framework Core will automatically use the SQL Server configuration you provided.

#### Use DbContext

You can now use your DbContext to interact with the SQL Server database as you normally would with Entity Framework Core. Here's an example of how to retrieve data from a table:

```csharp
using Com.Atomatus.Bootstarter.Context;
using Microsoft.Extensions.DependencyInjection;

public class MyService
{
    private readonly MyDbContext _dbContext;

    public MyService(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<MyEntity> GetAllEntities()
    {
        return _dbContext.MyEntities.ToList();
    }
}
```

## Usage

To use this SQL Server configuration in your .NET application, follow these steps:

1. Add the necessary NuGet packages for Entity Framework Core and SQL Server support.

2. Use the `AsSqlServer` extension method to mark your builder for SQL Server.

3. Configure the DbContext using the `AsSqlServerDbContextOptionsBuilder` method, specifying your application's `IServiceProvider`.

4. Register your DbContext as a service in the dependency injection container using the `AddDbContextAsSqlServer` methods.

5. Start using your DbContext to interact with the SQL Server database seamlessly.

For more details, refer to the code and examples in this repository.

## License

BootStarter (including the runtime repository) is licensed under the [Apache](LICENSE) license.

---

© Atomatus - All Rights Reserveds.
