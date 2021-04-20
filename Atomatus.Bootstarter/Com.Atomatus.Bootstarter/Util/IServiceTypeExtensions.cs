using Com.Atomatus.Bootstarter.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Com.Atomatus.Bootstarter
{
    internal static class IServiceTypeExtensions
    {
        /// <summary>
        /// Recovery arguments Entity and Id from <see cref="IService{TEntity, ID}"/> (<paramref name="sType"/>) type.
        /// </summary>
        /// <param name="sType">generic service type</param>
        /// <param name="entityArgType">argument entity type extracted from service type</param>
        /// <param name="idArgType">argument id entity type extracted from service type</param>
        public static void GetIServiceGenericArguments([NotNull] this Type sType, out Type entityArgType, out Type idArgType)
        {
            Type genSType = sType.GetGenericInterfaceType(typeof(IService<,>)) ??
                throw new ArgumentException($"Service type \"{sType.Name}\" " +
                    $"does not implements {typeof(IService<,>)}, " +
                    $"therefore is impossible identify entity and id type!");

            entityArgType   = genSType.GetGenericArguments().First(); //entity type.
            idArgType       = genSType.GetGenericArguments().Last(); //entity id type.
        }

        /// <summary>
        /// Recovery arguments Entity from <see cref="IService{TEntity}"/> (<paramref name="sType"/>) type.
        /// </summary>
        /// <param name="sType">generic service type</param>        
        /// <returns>argument entity type extracted from service type</returns>
        public static Type GetIServiceGenericArgument([NotNull] this Type sType)
        {
            Type genSType = sType.GetGenericInterfaceType(typeof(IService<>)) ??
                throw new ArgumentException($"Service type \"{sType.Name}\" " +
                    $"does not implements {typeof(IService<>)}, " +
                    $"therefore is impossible identify entity type!");

            return genSType.GetGenericArguments().First(); //entity type.
        }

    }
}
