# Atomatus BootStarter

[![Help Wanted](https://img.shields.io/github/issues/atomatus/runtime/dot-net-boot-starter?style=flat-square&color=%232EA043&label=help%20wanted)](https://github.com/atomatus/dot-net-boot-starter/issues?q=is%3Aissue+is%3Aopen+label%3A%22up-for-grabs%22)
[![NuGet version (Com.Atomatus.BootStarter)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter/)

## What is BootStarter?
<p>
  A multiplatform library to simple fast starter 
  <a href="https://docs.microsoft.com/pt-br/ef/core/get-started/overview/install" target="_blank">Entity Framework Core</a> projects by domain aplication 
  using <a href="https://docs.microsoft.com/pt-br/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0" target="_blank">Middlewares pipelines</a> strategy by 
  <a href="https://docs.microsoft.com/pt-br/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0" target="_blank">dependency-injection</a>.
</p>

<p>
  Boot Starter project contains abstract and contract classes (
  <a href="https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/IModel.cs">IModel</a>, 
  <a href="https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/Auditable/IAudit.cs">IAudit</a>, 
  <a href="https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/ModelBase.cs">ModelBase</a>, 
  <a href="https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/Auditable/AuditableModel.cs">AuditableModel</a>) 
  to modularize domain entities, services (
  <a href="https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Services/IService.cs">IService</a>) 
  pipelines as Repositories mode to persist data using 
  Entity Framework and DbContext (
  <a href="https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Context/ContextBase.cs">ContextBase</a>) as UnityOfWork.
</p>

### How to Install?
Install by Nuget [![NuGet version (Com.Atomatus.BootStarter)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter/)

### Bootstarter database types available

- Relational
  - SQL Server [![NuGet version (Com.Atomatus.BootStarter.SqlServer)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.SqlServer.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.SqlServer/)
  - Postgres [![NuGet version (Com.Atomatus.BootStarter.Postgres)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.Postgres.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.Postgres/)
  - SQLite [![NuGet version (Com.Atomatus.BootStarter.SqlServer)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.SQLite.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.SQLite/)
- NoSQL
  - Cosmos [![NuGet version (Com.Atomatus.BootStarter.Cosmos)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.Cosmos.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.Cosmos/)

## How to Use?
<p>
  First, choose your strategy design, for example, [DDD](https://en.wikipedia.org/wiki/Domain-driven_design).
</p>

### Short abstract about DDD layers

- Presentation:
  - Layer responsible for covering everything that concerns the UI (user interface).
    - Desktop UI (WinForms, WPF);
    - Web UI (Angular, React, Vue);
    - Mobile UI (Android, Xamarin).

- Service:
  - All ways of remote communication must take place here.
    - Web API (REST);
    - SignalR;
    - WebSockets.
    
- Application
  - Layer responsible for direct communication with the Domain layer. Implementing here:
    - Application services Classes;
    - Contracts (Interfaces);
    - Data Transfer Objects (DTO);
    - Auto Mapper.
  
- [Domain](#domain)
  - This layer contains the domain entities, stand-alone domain services, respositories contracts (interfaces), domain entity validation and bussines rules. So, DDD happens here.
    - Entities;
    - Service Contracts;
    - Respository Contracts;
    - Stand-Alone domain services;
    - Validations;
  
- Infrastructure (#infrastructure)
  - Layer that supports the other layers. Which is currently divided into two layers (Infra and CrossCutting) with their respective contents.
    - Infra:    
      - Sublayer of infrascture, responsible to persist data.
          - Repositories;
          - Data Model;
          - Data Persistence;
          
    - CrossCutting:
      - Sublayer of infrascture, responsible to cross all other layers applying Ioc (Inversion of control), dependency-injection.
          - Ioc;
          - Dependency Injection;
          - Register all Dependency reference and implementation, see more about it [here](https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/ioc);
  
### Domain
<p>
  Then, in your <i>Domain layer</i> contains the domain entities, stand-alone domain services.<br/>
  Remember that any domain concepts that depends on external resouces, are defined by interfaces
  and this interfaces implementations have to be in <i>Infrascture Layer</i>.  
</p>
<p>
  Now, following up the above concept were created the entity example class Client.  
</p>

- Domain class from <a href="https://github.com/atomatus/dot-net-boot-starter/blob/0487b192aa193c1d4f411396c4865520b5db250b/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/IModel.cs#L25">IModel<ID></a> (and optionally of <a href="https://github.com/atomatus/dot-net-boot-starter/blob/0487b192aa193c1d4f411396c4865520b5db250b/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/IModel.cs#L13">IModelAltenateKey</a>) Full Implementatin<br/>
![image](https://user-images.githubusercontent.com/10169901/116135710-ce8c9100-a6a7-11eb-8a33-db034bcff1b9.png)

- Domain class from <a href="https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/ModelBase.cs">ModelBase</a><br/>
*This class contains the properties Id and Uuid defined it.* <br/>
![image](https://user-images.githubusercontent.com/10169901/116171240-c6037d00-a6de-11eb-800b-75c3d5f84ece.png)

- Domain class from <a href="https://github.com/atomatus/dot-net-boot-starter/blob/main/Atomatus.Bootstarter/Com.Atomatus.Bootstarter/Model/Auditable/AuditableModel.cs">AuditableModel</a><br/>
*This class contains the properties Id and Uuid defined it, and when created or updated register the date time for it in entity.* <br/>
![image](https://user-images.githubusercontent.com/10169901/116171623-7ffae900-a6df-11eb-985d-a362112c5d30.png)

- Domain class Validation<br/>
*Sometimes you must implement somes validations, for it, override Validate method from IModel<> implementation.*<br/>
![image](https://user-images.githubusercontent.com/10169901/116171982-3959be80-a6e0-11eb-974d-bcdb3815ca7d.png)

- Domain class as Repository Entity<br/>
*When use the domain class as Repository Entity, you can explicit create a non public Configure method to define entity creationg rules.<br/> 
Then your ContextBase object (DBContext) when creating or migrating database in OnModelCreating(ModelBuilder) will looking for explicit definition of IEntityTypeConfiguration
for current Entities defined in DbSet<> properties or by explicit definition, like below.*<br/>
![image](https://user-images.githubusercontent.com/10169901/116172471-37dcc600-a6e1-11eb-9c78-294f54f2fa65.png)<br/>
*Warning: Define only your own Properties, the Id and UUid (when present) are defined automatically*

### Infrastructure 
In *Infrastructure* project layer, you can define explicitly the ContextBase as an UnitOfWork, the entities repositroy configuration map.<br/>
How like, displaying bellow.<br/>

#### Entity Configuration mapping

![image](https://user-images.githubusercontent.com/10169901/116485288-3c2fed00-a861-11eb-8cd5-76bd6d5c5562.png)<br/>
*Mapping entity explicitly, if using ContextBase class all entities mapping will be found automatically as along as existing
at same assembly entity definition. Otherwise, must setup this configurations mapping on ContextBase.OnModelCreating(ModelBuilder)*<br/>

#### [Db]ContextBase as UnitOfWork

<p>
Take on that the client mapping below is not present in the same assembly of entity client definition,
We must set it manually.
</p>

![image](https://user-images.githubusercontent.com/10169901/116485969-f07e4300-a862-11eb-921f-dc363740a438.png)

#### Registering the context setup direct to database type. 

<p>
The IServiceCollection can setup the dbContext using the method AddDbContext, to use as Bootstarter the method name start are equals, but  with sufix AddDbContext[[AsDatabaseType]]. 
</p>

![image](https://user-images.githubusercontent.com/10169901/116490346-828b4900-a86d-11eb-8e56-d55c23a182f6.png)

<p>
Or can you register the context base as dynamic and using Ioc concept to register the entities services CRUD, how like, displaying bellow.
</p>

![image](https://user-images.githubusercontent.com/10169901/116490585-2117aa00-a86e-11eb-9d84-ad881aea9eaa.png)

###  Infrastructure CrossCuting
Generate and add services to Entities. You can setup it explity or from implicit way in dynamic context.

![image](https://user-images.githubusercontent.com/10169901/116491638-b7e56600-a870-11eb-92d9-ea11ba401778.png)


## License

BootStarter (including the runtime repo) is licensed under the [Apache](LICENSE) license.
