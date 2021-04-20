using System;
using System.Linq;

namespace Com.Atomatus.Bootstarter.Context
{
    internal sealed class ContextTypeBuilder : IDisposable
    {
        private IContextServiceTypes service;

        public ContextTypeBuilder(IContextServiceTypes service)
        {
            this.service = service;
        }

        void IDisposable.Dispose()
        {
            this.service = null;
        }

        public Type Build()
        {
            return DynamicTypeFactory
                .AsContext()
                .GetOrCreate(service
                    .Select(st => st.GetIServiceGenericArgument())
                    .Distinct());
        }
    }
}
