# Using SQLite Database with LocalContext in ASP.NET Core

This guide demonstrates how to configure and use Entity Framework Core with SQLite as the database provider in your .NET application. SQLite is a lightweight and embedded relational database engine.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) or another code editor of your choice.

## Installation

To get started, install the necessary NuGet packages for Entity Framework Core and SQLite support in your project. You can do this using the command-line interface (CLI) or the NuGet Package Manager in Visual Studio.

```shell
dotnet add package Atomatus.Bootstarter.Sqlite
```

## Configuration

### Step 1: Configure the SQLite Connection

In your application, you can configure the SQLite connection using the provided ContextConnectionSqlite class and the AsSqlite extension method:

```csharp
using Com.Atomatus.Bootstarter.Context;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .AddDbContextAsSqlite<MyDbContext>(builder =>
    {
        builder
            .Database("YourDatabaseName.db")  // Set your SQLite database file name
            .Options(options => options.MigrationsAssembly("Your.Migrations.Assembly.Name"));
    })
    .BuildServiceProvider();
```
You can specify the database file name and migrations assembly name in the builder action.

### Step 2: Register DbContext as a Service

Register your DbContext as a service in the dependency injection container using the AddDbContextAsSqlite method:

```csharp
services.AddDbContextAsSqlite<MyDbContext>(builder =>
{
    builder
        .Database("YourDatabaseName.db")  // Set your SQLite database file name
        .Options(options => options.MigrationsAssembly("Your.Migrations.Assembly.Name"));
});
```

### Step 3: Use DbContext
You can now use your MyDbContext to interact with the SQLite database as you normally would with Entity Framework Core:

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

This project is a comprehensive guide on configuring and using Entity Framework Core with SQLite as the database provider in a .NET application. It provides step-by-step instructions on installation, configuration, and usage of SQLite databases within your .NET projects. By following this guide, developers can seamlessly integrate SQLite into their applications, allowing for efficient data storage and retrieval while leveraging the power and flexibility of Entity Framework Core. Whether you're building a console application or an ASP.NET Core web application, this project equips you with the knowledge and code snippets needed to get started with SQLite and Entity Framework Core quickly.

## License

BootStarter (including the runtime repository) is licensed under the [Apache](LICENSE) license.

---

© Atomatus - All Rights Reserveds.
