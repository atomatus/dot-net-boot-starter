# Using SQL Postgres Database with LocalContext in ASP.NET Core

This README provides an overview of the `ContextConnectionPostgres` class and related extensions for working with PostgreSQL databases in an Entity Framework Core context. It's a part of the `Com.Atomatus.Bootstarter.Context` namespace.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) or another code editor of your choice.

## Installation

To get started, install the necessary NuGet packages for Entity Framework Core and SQLite support in your project. You can do this using the command-line interface (CLI) or the NuGet Package Manager in Visual Studio.

```shell
dotnet add package Atomatus.Bootstarter.Postgres
```

## ContextConnectionPostgres Class

The `ContextConnectionPostgres` class is used for configuring a connection to a PostgreSQL database within an Entity Framework Core context. Here's a brief overview of this class:

```csharp
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;

namespace Com.Atomatus.Bootstarter.Context
{
    internal sealed class ContextConnectionPostgres : ContextConnection
    {
        // Constructor and constants here...

        protected override string GetConnectionString()
        {
            // Connection string building logic...
        }

        protected internal override DbContextOptionsBuilder Attach(DbContextOptionsBuilder options)
        {
            // DbContext options configuration...
        }
    }
}
```

## ContextConnectionPostgres Extensions
Extensions for configuring PostgreSQL connections and contexts are also provided. Below are some of the key extension methods:

### AsPostgres Method
Use this method to mark the builder for building a PostgreSQL connection:

```csharp
public static ContextConnection.Builder AsPostgres(this ContextConnection.Builder builder)
```

### AsPostgresDbContextOptionsBuilder Method

This method allows you to configure DbContextOptionsBuilder for PostgreSQL:

```csharp
public static DbContextOptionsBuilder AsPostgresDbContextOptionsBuilder(
    this ContextConnection.Builder builder,
    IServiceProvider provider,
    DbContextOptionsBuilder options)
```

### AddDbContextAsPostgres Methods

These methods register the given context as a PostgreSQL DbContext service.

```csharp
public static IServiceCollection AddDbContextAsPostgres<TContext>(
    this IServiceCollection services,
    Action<ContextConnection.Builder> builderAction,
    Action<IContextServiceCollection> serviceAction,
    ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
    ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : ContextBase
```

You can use these extensions to set up PostgreSQL database connections and contexts in your .NET applications.

## License

BootStarter (including the runtime repository) is licensed under the [Apache](LICENSE) license.

---

© Atomatus - All Rights Reserveds.
