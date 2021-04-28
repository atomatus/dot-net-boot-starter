using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Atomatus.Bootstarter
{
    internal static class TypeExtensions
    {
        public static bool IsSubclassOfRawGenericType(this Type type, Type superType)
        {
            return type != null && (type.IsSubclassOf(superType) ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == superType) ||
                IsSubclassOfRawGenericType(type.BaseType, superType));
        }

        public static Type GetGenericInterfaceType(this Type type, Type genericInterfaceType)
        {
            return type.GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == genericInterfaceType);
        }

        public static IEnumerable<Guid> ToFlattenGuids(this IEnumerable<Type> types)
        {
            return types.SelectMany(t => t.IsGenericType ?
                t.GetGenericArguments().ToFlattenGuids() :
                new[] { t.GUID });
        }
    }
}
