using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100790ebf3db21fd725034bc88eb382f2bfd3f17fb28e5a3b80c2ec0fce9621b3e94722d7ee3d0f842ef940f82a250495cdaf6867eb82ba987d475b185c5738b49428e9c7e782676fabeb91191678768c211774018e6527dd2a1ccdedeca941408a3a9696d9365326125530cb2b8c557f34b490ae15fd8016edccf33e47f6ea77f3")]
namespace Com.Atomatus.Bootstarter.Util
{
    /// <summary>
    /// <para>Object Mapper</para>
    /// <para>
    /// Provides a set of utility methods for copying and parsing objects, including property value copying,
    /// object creation, and type conversion. The methods in this class allow for flexible handling of copying
    /// and converting objects, using a combination of strategies to achieve the desired result.
    /// </para>
    /// </summary>
    /// <author>Carlos Matos</author>
    /// <date>2023-08-10</date>
    public static partial class ObjectMapper
    {
        /// <summary>
        /// Parses the <paramref name="source"/> object to the specified <typeparamref name="T"/> by applying
        /// type conversion or object copying as needed.
        /// </summary>
        /// <param name="source">The source object to be parsed or copied.</param>
        /// <returns>
        /// The parsed or copied object of the specified <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <typeparamref name="T"/> is null.</exception>
        /// <remarks>
        /// If <paramref name="source"/> is null, this method returns the default value of <typeparamref name="T"/>.
        /// If <paramref name="source"/> is already of the same type as <typeparamref name="T"/>,
        /// it is returned as is.
        /// If both <paramref name="source"/> and <typeparamref name="T"/> are reference types (classes),
        /// and they are not string types, the method attempts to create a new instance of <typeparamref name="T"/>
        /// and copy the properties from <paramref name="source"/> to the new instance.
        /// Otherwise, if type conversion is possible using <see cref="Convert.ChangeType(object, Type)"/>,
        /// it is performed and the converted value is returned.
        /// </remarks>
        public static T Parse<T>([MaybeNull] object source)
        {
            return (T) Parse(source, typeof(T));
        }

        /// <summary>
        /// Parses the <paramref name="source"/> object to the specified <paramref name="targetType"/> by applying
        /// type conversion or object copying as needed.
        /// </summary>
        /// <param name="source">The source object to be parsed or copied.</param>
        /// <param name="targetType">The target type to which the <paramref name="source"/> will be converted or copied.</param>
        /// <returns>
        /// The parsed or copied object of the specified <paramref name="targetType"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetType"/> is null.</exception>
        /// <remarks>
        /// If <paramref name="source"/> is null, this method returns the default value of <paramref name="targetType"/>.
        /// If <paramref name="source"/> is already of the same type as <paramref name="targetType"/>,
        /// it is returned as is.
        /// If both <paramref name="source"/> and <paramref name="targetType"/> are reference types (classes),
        /// and they are not string types, the method attempts to create a new instance of <paramref name="targetType"/>
        /// and copy the properties from <paramref name="source"/> to the new instance.
        /// Otherwise, if type conversion is possible using <see cref="Convert.ChangeType(object, Type)"/>,
        /// it is performed and the converted value is returned.
        /// </remarks>
        public static object Parse([MaybeNull] object source, [NotNull] Type targetType)
        {
            if (targetType is null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }
            else if (source is null)
            {
                return GetDefaultValue(targetType);
            }

            Type sourceType = source.GetType();

            if (sourceType == targetType || ((targetType.IsInterface || targetType.IsAbstract) && targetType.IsAssignableFrom(sourceType)))
            {
                return source;
            }
            else if (targetType.IsPrimitive || targetType == typeof(string) || targetType.IsEnum || !targetType.IsClass)
            {
                return Convert.ChangeType(source, targetType);
            }
            else
            {
                object target = Activator.CreateInstance(targetType);
                if (Copy(source, target))
                {
                    return target;
                }
                else
                {
                    throw new InvalidOperationException($"Was not possible convert \"{sourceType.FullName}\" to \"{targetType.FullName}\"!");
                }
            }
        }

        /// <summary>
        /// Parses the <paramref name="source"/> object to the specified <typeparamref name="T"/> list by applying
        /// type conversion or object copying as needed.
        /// </summary>
        /// <param name="source">The source object to be parsed or copied.</param>
        /// <returns>
        /// The parsed or copied object of the specified <typeparamref name="T"/> list item.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <typeparamref name="T"/> is null.</exception>
        /// <remarks>
        /// If <paramref name="source"/> is null, this method returns the default value of <typeparamref name="T"/>.
        /// If <paramref name="source"/> is already of the same type as <typeparamref name="T"/>,
        /// it is returned as is.
        /// If both <paramref name="source"/> and <typeparamref name="T"/> are reference types (classes),
        /// and they are not string types, the method attempts to create a new instance of <typeparamref name="T"/>
        /// and copy the properties from <paramref name="source"/> to the new instance.
        /// Otherwise, if type conversion is possible using <see cref="Convert.ChangeType(object, Type)"/>,
        /// it is performed and the converted value is returned.
        /// </remarks>
        public static List<T> ParseList<T>([MaybeNull] object source)
        {
            return Parse<List<T>>(source);
        }

        /// <summary>
        /// Parses the <paramref name="source"/> object to the specified <typeparamref name="TTarget"/> list by applying
        /// type conversion or object copying as needed.
        /// </summary>
        /// <param name="source">The source object to be parsed or copied.</param>
        /// <returns>
        /// The parsed or copied object of the specified <typeparamref name="TTarget"/> list item.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <typeparamref name="TTarget"/> is null.</exception>
        /// <remarks>
        /// If <paramref name="source"/> is null, this method returns the default value of <typeparamref name="TTarget"/>.
        /// If <paramref name="source"/> is already of the same type as <typeparamref name="TTarget"/>,
        /// it is returned as is.
        /// If both <paramref name="source"/> and <typeparamref name="TTarget"/> are reference types (classes),
        /// and they are not string types, the method attempts to create a new instance of <typeparamref name="TTarget"/>
        /// and copy the properties from <paramref name="source"/> to the new instance.
        /// Otherwise, if type conversion is possible using <see cref="Convert.ChangeType(object, Type)"/>,
        /// it is performed and the converted value is returned.
        /// </remarks>
        public static List<TTarget> ParseList<TSource, TTarget>([MaybeNull] IList<TSource> source)
        {
            return source?.Select(s => Parse<TTarget>(s)).ToList() ?? new List<TTarget>();
        }

        /// <summary>
        /// Retrieves the default value for a given <paramref name="type"/>.
        /// For reference types (classes), it returns null, and for value types (structs),
        /// it creates and returns an instance of the type with its default values.
        /// </summary>
        /// <param name="type">The Type for which to retrieve the default value.</param>
        /// <returns>
        /// The default value for the specified <paramref name="type"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="type"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="type"/> is of an unsupported type.
        /// </exception>
        public static object GetDefaultValue([NotNull] Type type)
        {
            if (type.IsClass)
            {
                return null; // Return null for reference types (classes).
            }
            else if (type.IsValueType)
            {
                return Activator.CreateInstance(type); // Create an instance for value types (structs).
            }
            else
            {
                throw new ArgumentException("Unsupported type: " + type);
            }
        }
    }
}
