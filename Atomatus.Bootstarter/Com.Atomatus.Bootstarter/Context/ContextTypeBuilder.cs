using System;

namespace Com.Atomatus.Bootstarter.Context
{
    internal sealed class ContextTypeBuilder : IDisposable
    {
        private ContextConnection.Builder builder;
        private IContextServiceTypes service;
        
        public ContextTypeBuilder(ContextConnection.Builder builder, IContextServiceTypes service)
        {
            this.builder = builder;
            this.service = service;
        }

        void IDisposable.Dispose()
        {
            this.builder = null;
            this.service = null;
        }

        public Type Build()
        {
            return DynamicTypeFactory
                .AsContext()
                .Ensures(builder)
                .Entities(service)
                .GetOrCreate();
        }
    }
}
