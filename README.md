# Atomatus BootStarter

[![Help Wanted](https://img.shields.io/github/issues/atomatus/runtime/dot-net-boot-starter?style=flat-square&color=%232EA043&label=help%20wanted)](https://github.com/atomatus/dot-net-boot-starter/issues?q=is%3Aissue+is%3Aopen+label%3A%22up-for-grabs%22)
[![NuGet version (SoftCircuits.Silk)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter/)

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

## How to Use?
<p>
  First, choose your strategy design, for example, [DDD](https://en.wikipedia.org/wiki/Domain-driven_design).
</p>

### Small resume about DDD Layers

* Presentation:
  *  Layer responsible for covering everything that concerns the UI (user interface):
    *  - Desktop UI (WinForms, WPF);
    *  - Web UI (Angular, React, Vue);
    *  - Mobile UI (Android, Xamarin).
    
* Service:
  * All ways of remote communication must take place here:
    * - Web API (REST);
    * - SignalR
    * - WebSockets
    
* Application
  * a
  
* Domain
  * a
  
* Infrastructure
  * a
  
### Domain
<p>
  Then, in your <i>Domain layer</i> contains the domain entities, stand-alone domain services.<br/>
  Remember that any domain concepts that depends on external resouces, are defined by interfaces
  and this interfaces implementations have to be in <i>Infrascture Layer</i>.  
</p>
<p>
  Now, following up the above concept were created the entity class Client.  
</p>

* Domain class from IModel<> Full Implementatin
* Domain class from ModelBase
* Domain class from AuditableModel


## License

.NET (including the runtime repo) is licensed under the [Apache](LICENSE) license.
