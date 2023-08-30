using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter
{
    public static partial class ObjectMapper
    {
        /// <summary>
        /// <para>
        /// Copies property values from the <paramref name="source"/> object to the <paramref name="target"/> object.
        /// </para>
        /// <para>
        /// This method facilitates the transfer of property values between objects, applying a series of strategies
        /// to handle different copying scenarios.
        /// </para>
        /// </summary>
        /// <param name="source">source object</param>
        /// <param name="target">target object to accept the values</param>
        /// <returns>True, source properties was copied to target, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Throws when source or target is null</exception>
        /// <exception cref="InvalidOperationException">Throws when is not possible copy value from source to target</exception>
        public static bool Copy([NotNull] object source, [NotNull] object target)
        {
            using var handler = new CopyHandler(() => new EqualTypeCopyStrategy());
            handler
                .Next(() => new CollectionCopyStrategy())
                .Next(() => new AssignableTypeCopyStrategy())
                .Next(() => new CommonInterfaceCopyStrategy())
                .Next(() => new CommonPropertyCopyStrategy());
            return handler.Handle(source, target);
        }

        /// <summary>
        /// Create a copy of <paramref name="source"/> object.
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <param name="source">source object to be copied</param>
        /// <returns>copy of source object, otherwise null</returns>
        /// <exception cref="InvalidOperationException">Throws when is not possible copy value from source to target</exception>
        public static T Copy<T>([MaybeNull] T source)
        {
            if (source is null || object.ReferenceEquals(source, default(T)))
            {
                return default;
            }
            else
            {
                Type sType = source.GetType();
                SolveNullableType(ref sType);

                if (sType.IsClass && sType != typeof(string))
                {
                    T target = (T) Activator.CreateInstance(sType);
                    if(Copy(source, target))
                    {
                        return target;
                    }
                    else
                    {
                        throw new InvalidOperationException("Was not possible create a copy of object type: " + sType.FullName);
                    }
                }
                else
                {
                    return source;
                }
            }
        }
    }
}
