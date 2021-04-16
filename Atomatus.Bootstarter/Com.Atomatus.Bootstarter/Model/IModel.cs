using System;

namespace Com.Atomatus.Bootstarter.Model
{
    public interface IModel  {  }

    public interface IModelAltenateKey
    {
        Guid Uuid { get; set; }
    }

    public interface IModel<ID> : IModel
    {
        ID Id { get; set; }
    }
}
