# Atomatus BootStarter

[![GitHub issues](https://img.shields.io/github/issues/atomatus/dot-net-boot-starter?style=flat-square&color=%232EA043&label=help%20wanted)](https://github.com/atomatus/dot-net-boot-starter)

[![NuGet version (Com.Atomatus.BootStarter)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter/)

## What is BootStarter?

BootStarter is a cross-platform library designed to simplify and accelerate the startup of [Entity Framework Core](https://docs.microsoft.com/pt-br/ef/core/get-started/overview/install) projects. It follows a domain-driven design approach and leverages [middleware pipelines](https://docs.microsoft.com/pt-br/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0) and [dependency injection](https://docs.microsoft.com/pt-br/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0) strategies.

The BootStarter project provides a set of abstract and contract classes to help modularize domain entities and services, including:

- [IModel](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/IModel.cs)  
- [IAudit](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/Auditable/IAudit.cs)  
- [ModelBase](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/ModelBase.cs)  
- [SoftDeleteModel](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/SoftDelete/SoftDeleteModel.cs)  
- [AuditableModel](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/Auditable/AuditableModel.cs)  
- [SoftDeleteAuditableModel](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/SoftDelete/SoftDeleteAuditableModel.cs)

It also includes service contracts like:

- [IService](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Services/IService.cs)

BootStarter implements a repository pipeline pattern to persist data using Entity Framework and DbContext, with support for Unit of Work through:

- [ContextBase](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Context/ContextBase.cs)

### How to Install?
[![NuGet version (Com.Atomatus.BootStarter)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter/)

#### Installing the _`Com.Atomatus.BootStarter`_ Package via NuGet

To begin, you'll need [NuGet](https://www.nuget.org/) installed in your project. NuGet is a package manager for .NET projects that makes it easy to add and update third-party libraries and packages.

Here are the steps to install the _`Com.Atomatus.BootStarter`_ package:

1. **Open Visual Studio or Your Preferred IDE**:

   Make sure your .NET project is open in your IDE.

2. **Access the NuGet Package Manager Window**:

   - In Visual Studio, go to `Tools > NuGet Package Manager > Manage NuGet Packages for Solution`.

3. **Search for the Package**:

   In the NuGet Package Manager, on the "Browse" tab, type `Com.Atomatus.BootStarter` into the search bar.

4. **Select the Package**:

   When the `Com.Atomatus.BootStarter` package appears in the search results, click on it to select it.

5. **Install the Package**:

   Click the "Install" button to initiate the package installation. NuGet will automatically download and install the package along with its dependencies into your project.

6. **Verify the Installation**:

   After the installation is complete, you will see the package listed in your solution, and relevant files will be added to your project.

7. **Import the Package in Your Code**:

   To start using the _`Com.Atomatus.BootStarter`_ package, you can import the relevant namespaces in your code files as needed.

    ```csharp
    using Com.Atomatus.BootStarter;
    ```


### Bootstarter database types available

- **Relational**
  - SQL Server 
    - How To Use:  [README.md](/Atomatus.Bootstarter/Com.Atomatus.Bootstarter.Sqlserver/README.md)

    - How To Install: [![NuGet version (Com.Atomatus.BootStarter.SqlServer)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.SqlServer.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.SqlServer/)

  - Postgres 
    - How To Use:  [README.md](/Atomatus.Bootstarter/Com.Atomatus.Bootstarter.Postgres/README.md)

    - How To Install: [![NuGet version (Com.Atomatus.BootStarter.Postgres)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.Postgres.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.Postgres/)

  - SQLite 
    - How To Use:  [README.md](/Atomatus.Bootstarter/Com.Atomatus.Bootstarter.Sqlite/README.md)

    - How To Install: [![NuGet version (Com.Atomatus.BootStarter.SqlServer)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.Sqlite.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.Sqlite/)

- **NoSQL**
  - Cosmos 
    - How To Use: [README.md](/Atomatus.Bootstarter/Com.Atomatus.Bootstarter.Cosmos/README.md)

    - How To Install: [![NuGet version (Com.Atomatus.BootStarter.Cosmos)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.Cosmos.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.Cosmos/)

- **In Memory**
    - How To Use: [README.md](/Atomatus.Bootstarter/Com.Atomatus.Bootstarter.InMemory/README.md)

    - How To Install: [![NuGet version (Com.Atomatus.BootStarter.InMemory)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.InMemory.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.InMemory/)

## How to Use?

- **First**: choose your strategy design, for example, ***[DDD](https://en.wikipedia.org/wiki/Domain-driven_design)***;

  - Short abstract about DDD layers

    - **Presentation**
      - Layer responsible for covering everything that concerns the UI (user interface).
        - Desktop UI (WinForms, WPF);
        - Web UI (Angular, React, Vue);
        - Mobile UI (Android, Xamarin).

    - **[Service](#services)**
      - All ways of remote communication must take place here.
        - Web API (REST);
        - SignalR;
        - WebSockets.
        
    - **[Application](#services)**
      - Layer responsible for direct communication with the Domain layer. Implementing here:
        - Application services Classes;
        - Contracts (Interfaces);
        - Data Transfer Objects (DTO);
        - Auto Mapper.
      
    - **[Domain](#domain)**
      - This layer contains the domain entities, stand-alone domain services, respositories contracts (interfaces), domain entity validation and bussines rules. So, DDD happens here.
        - Entities;
        - Service Contracts;
        - Respository Contracts;
        - Stand-Alone domain services;
        - Validations;
      
    - **[Infrastructure](#infrastructure)**
      - Layer that supports the other layers. Which is currently divided into two layers (Infra and CrossCutting) with their respective contents.
        - **Infra:**    
          - Sublayer of infrascture, responsible to persist data.
              - Repositories;
              - Data Model;
              - Data Persistence;
              
        - **CrossCutting:**
          - Sublayer of infrascture, responsible to cross all other layers applying Ioc (Inversion of control), dependency-injection.
              - Ioc;
              - Dependency Injection;
              - Register all Dependency reference and implementation, see more about it [here](https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/ioc);
  
- **Second**: choorse your architecture, for example, ***Clean Architecture***.
In Clean Architecture, software systems are organized into distinct layers or concentric circles, each with a specific purpose. Here's an explanation of the layers in `Clean Architecture`:

    - **Domain Layer**: The Domain Layer represents the core of the application and contains the business logic, rules, and entities. It defines the fundamental building blocks of the application and is independent of any external concerns, such as the user interface or database. In this layer, you define your domain models, entities, value objects, and business logic. It should be free from any framework-specific code.

    - **Infrastructure Layer**: The Infrastructure Layer is responsible for handling external concerns and technical details. It includes components like databases, file systems, external services, and frameworks. This layer is where you interact with data storage, handle cross-cutting concerns like logging and configuration, and implement infrastructure-specific code. It should not contain any business logic and should be replaceable without affecting the core domain.

    - **Application Layer**: The Application Layer contains the application-specific logic that coordinates the interaction between the Domain Layer and the Infrastructure Layer. It includes use cases, services, and application-specific rules. Use cases represent specific application actions or scenarios, often corresponding to user stories or system functionality. Services can encapsulate business logic that doesn't naturally fit into entities or value objects. The Application Layer acts as a bridge between the domain and infrastructure, ensuring that business rules are applied and data is retrieved or stored appropriately.
    
    - **Presentation Layer**: The Presentation Layer is responsible for interacting with the user or external systems. It includes components like user interfaces (UI), web APIs, and controllers. This layer translates user inputs or external requests into actions that the Application Layer can understand and execute. It also presents information from the Application Layer to the user or external systems in a format they can consume. The Presentation Layer is typically the most replaceable part of the system, as long as it adheres to the contracts established by the Application Layer.

In `Clean Architecture`, the key principle is the separation of concerns, ensuring that each layer has a specific and well-defined responsibility. This separation allows for easier maintenance, testability, and adaptability of the software system. Changes in one layer should not require modifications in other layers, promoting flexibility and long-term sustainability. Additionally, the architecture enforces a clear boundary between business logic and technical details, making it easier to reason about and evolve the software over time.

## Layer Types In Use

### Domain
Then, in your _Domain layer_ contains the domain entities, stand-alone domain services.

Remember that any domain concepts that depends on external resouces, are defined by interfaces and this interfaces implementations have to be in _Infrascture Layer_.  

Now, following up the above concept were created the entity example class Client.

  - **Domain class from [IModel<ID>](https://github.com/atomatus/dot-net-boot-starter/blob/0487b192aa193c1d4f411396c4865520b5db250b/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/IModel.cs#L25") (and optionally of [IModelAltenateKey](https://github.com/atomatus/dot-net-boot-starter/blob/0487b192aa193c1d4f411396c4865520b5db250b/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/IModel.cs#L13)) Full Implementatin.**

    ```csharp
    public partial class Client : IModel<long>, IModelAlternateKey 
    {
      public long Id { get; set; }

      public Guid Uuid { get; set; }

      public int Age { get; set; }

      public string Name { get; set; }
    }
    ```

- **Domain class from [ModelBase](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/ModelBase.cs)**

  *This class contains the properties Id and Uuid defined it.*

  ```csharp
  public partial class Client : ModelBase<long> 
  {
    public int Age { get; set; }

    public string Name { get; set; }
  }
  ```

- **Domain class from [AuditableModel](https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/Auditable/AuditableModel.cs)**

  *This class contains the properties Id and Uuid defined it, and when created or updated register the date time for it in entity.*

  ```csharp
  public partial class Client : AuditableModel<long> 
  {
    public int Age { get; set; }

    public string Name { get; set; }
  }
  ```

- **Domain class Validation**

  *Sometimes you must implement somes validations, for it, override Validate method from IModel<> implementation.*

  ```csharp
  public partial class Client 
  {
    protected override IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
      if (Age < 1 || Age > 200) 
      {
        yield return new ValidationResult("Invalid Age value", new[] { nameof(Age) });
      }
      else if (string.isNullOrWhiseSpace(Name)) 
      {
        yield return new ValidationResult("Invalid Name!", new[] { nameof(Name) });
      }
    }
  }
  ```

- **Domain class as Repository Entity**

  When use the domain class as Repository Entity, you can explicit create a non public Configure method to define entity creationg rules. 

  Then your ContextBase object (DBContext) when creating or migrating database in OnModelCreating(ModelBuilder) will looking for explicit definition of IEntityTypeConfiguration
  for current Entities defined in DbSet<> properties or by explicit definition, like below.

  ```csharp
  public partial class Client 
  {
    protected void Configure(EntityTypeBuilder<Client> builder) {
        builder
          .Property(e => e.Age)
          .HasMaxLength(3)
          .IsRequired();

        builder
          .Property(e => e.Name)
          .HasMaxLength(100)
          .IsRequired();
    }
  }
  ```
  ***Warning:*** *Define only your own Properties, the Id and UUid (when present) are defined automatically*

### Infrastructure 

In *Infrastructure* project layer, you can define explicitly the ContextBase as an UnitOfWork, the entities repositroy configuration map.

How like, displaying bellow.

#### Entity Configuration mapping

```csharp
public class ClientMapping : EntityTypeConfigurationBase<Client, long> 
{
  protected override void OnConfigure(EntityTypeBuilder<Client> builder) 
  {
        builder
          .Property(e => e.Age)
          .HasMaxLength(3)
          .IsRequired();

        builder
          .Property(e => e.Name)
          .HasMaxLength(100)
          .IsRequired();
  }
}
```

*Mapping entity explicitly, if using ContextBase class all entities mapping will be found automatically as along as existing
at same assembly entity definition. Otherwise, must setup this configurations mapping on ContextBase.OnModelCreating(ModelBuilder)*<br/>

#### [Db]ContextBase as UnitOfWork

Take on that the client mapping below is not present in the same assembly of entity client definition,
We must set it manually.

```csharp
public class LocalContext : ContextBase
{
    public DbSet<Client> Clients { get; internal set; }

    public LocalContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Here, the base class will attempt to load entity mappings
        // from the same assembly as each DbSet property in LocalContext.
        base.OnModelCreating(modelBuilder);
        
        // However, please note that the client mapping below is not 
        // included in the same assembly as the Client entity definition.
        // Therefore, we need to set it manually.
        modelBuilder.ApplyConfiguration(new ClientMapping());
    }
}
```

#### Registering the context setup direct to database type. 

The `IServiceCollection` can setup the dbContext using the method AddDbContext, to use as Bootstarter the method name start are equals, but  with sufix `AddDbContext[[AsDatabaseType]]`. 

```csharp
public class Startup
{
  //...

  public void ConfigureServices(IServiceCollection services) {
    services.AddDbContextAsSqlServer<LocalContext>();
  }
}
```

Or can you register the context base as dynamic and using Ioc concept to register the entities services CRUD, how like, displaying bellow.

```csharp
public class Startup
{
  //...

  public void ConfigureServices(IServiceCollection services) {
    //Add DbContext for SQL Server database type
    //for dynamic context, must setyp entities.
    //But, you can setup connection configurations, services and context lifetime.
    services.AddDbContextAsSqlServer(s => s.AddServiceTo<Client>());
  }
}
```

### Services

Generate services in application layer to consume infrastructure and provides an access to it. Services can encapsulate business logic that doesn't naturally fit into entities or value objects. The Application Layer acts as a bridge between the domain and infrastructure, ensuring that business rules are applied and data is retrieved or stored appropriately.

```csharp
public interface IClientService : IServiceCrud<Client, long> { }
```

```csharp
internal class ClientService :
ServiceCrud<LocalContext, Client, long>, 
IClientService
{
  //using constructor to discovery
  //client dbset
  public ClientService([NotNull] LocalContext context) 
  : base(context) {}

  //or

  //using constructor explitily setup
  //cient dbset
  public ClientService([NotNull] LocalContext context) 
  : base(context, context.Clients) {}
}
```

###  Infrastructure CrossCuting
Generate and add services to Entities. You can setup it explity or from implicit way in dynamic context.

```csharp
public void ConfigureServices(IServiceCollection services) 
{
  services.AddDbContextAsSqlServer<LocalContext>()
  //using dynamic service CRUD generation for your context and model.
  //This service is accessible by
  //IServiceProvider method GetServiceTo<Client>() or
  //passing the GetService<IServiceCrud<Client>>.
  .AddService<LocalContext, Client, long>()
  //or explicitly setup your service implementation.
  .AddScoped<IClientService, ClientService>();

  //dynamic DBContext way

  //Creating service to entity client
  //to generate accessible service to IServiceCrud.
  services.AddDbContextAsSqlServer(
    s => s.AddServiceTo<Client>()
  );
}
```

See more database types in ***[Bootstarter database types available readme.md](#Bootstarter-database-types-available)*** files.

## License

BootStarter (including the runtime repository) is licensed under the [Apache](LICENSE) license.

---

© Atomatus - All Rights Reserveds.