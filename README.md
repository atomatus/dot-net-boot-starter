# .Net BootStarter ⊞📦💻📱

<p>
  Library for multiplatforms to simple fast starter 
  <a href="https://docs.microsoft.com/pt-br/ef/core/get-started/overview/install">Entity Framework Core</a> projects by domain aplication 
  using <a href="https://docs.microsoft.com/pt-br/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0">Middlewares pipelines</a> strategy by 
  <a href="https://docs.microsoft.com/pt-br/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0">dependency-injection</a>.
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
