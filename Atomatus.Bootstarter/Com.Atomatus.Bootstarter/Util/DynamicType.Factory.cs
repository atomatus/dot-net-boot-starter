using System;

namespace Com.Atomatus.Bootstarter
{
    internal class DynamicTypeFactory
    {
        private static readonly Lazy<DynamicTypeContext> dynamicTypeContextLazy;
        private static readonly Lazy<DynamicTypeService> dynamicTypeServiceLazy;

        static DynamicTypeFactory()
        {
            dynamicTypeContextLazy = new Lazy<DynamicTypeContext>(
                valueFactory: () => new DynamicTypeContext(), 
                isThreadSafe: true);

            dynamicTypeServiceLazy = new Lazy<DynamicTypeService>(
                valueFactory: () => new DynamicTypeService(),
                isThreadSafe: true);
        }

        public static DynamicTypeContext AsContext() => dynamicTypeContextLazy.Value;

        public static DynamicTypeService AsService() => dynamicTypeServiceLazy.Value;

    }
}
