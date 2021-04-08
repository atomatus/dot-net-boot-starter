using System;

namespace Com.Atomatus.Bootstarter.Model
{
    public interface IModel 
    { 
        object Id { get; } 
    }

    public interface IModelAltenateKey
    {
        Guid Uuid { get; set; }
    }

    public interface IModel<ID> : IModel
    {
        new ID Id { get; set; }
    }
}
