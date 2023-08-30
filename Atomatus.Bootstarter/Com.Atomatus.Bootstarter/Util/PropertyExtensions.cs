using System;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// PropertyInfo Extensions.
    /// </summary>
    internal static class PropertyExtensions
	{
        private static readonly string[] IgnoreAttrs;

        static PropertyExtensions()
        {
            IgnoreAttrs = new[]
            {
                nameof(IgnoreAttribute),
                nameof(XmlIgnoreAttribute),
                "JsonIgnoreAttribute"
            };
        }

        /// <summary>
        /// Check if property is not marked as ignored (<c>[JsonIgnore]</c>, <c>[XmlIgnore]</c> or <c>[Ignore]</c>).
        /// </summary>
        /// <param name="property"></param>
        /// <returns>true, is not marked as ignore attribute, otherwise false (is ignore)</returns>
        public static bool IsNotIgnored(this PropertyInfo property)
		{
            return !property.IsIgnored();
		}

        /// <summary>
        /// Check if property is not as ignored (Contains some Attribute annotation that includes Ignore,
        /// how like, <c>[JsonIgnore]</c>, <c>[XmlIgnore]</c> or <c>[Ignore]</c>.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>true, is marked as ignore attribute, otherwise false</returns>
        public static bool IsIgnored(this PropertyInfo property)
        {
            return property.GetCustomAttributes(true)
                .Any(attr => IgnoreAttrs.Any(iAttr => iAttr.SequenceEqual(attr.GetType().Name)));
        }

        /// <summary>
        /// Check if property is not marked as <c>virtual</c>.
        /// </summary>
        /// <param name="property">current property</param>
        /// <returns>true, is not marked as virtual, otherwise false (is virtual)</returns>
        public static bool IsNotVirtual(this PropertyInfo property)
		{
			return !property.IsVirtual();
		}

        /// <summary>
        /// Check if property is marked as <c>virtual</c>.
        /// </summary>
        /// <param name="property">current property</param>
        /// <returns>true, is virtual, otherwise false</returns>
        public static bool IsVirtual(this PropertyInfo property)
        {
            return (property.CanRead && property.GetGetMethod().IsVirtual) ||
                (property.CanWrite && property.GetSetMethod().IsVirtual);
        }
    }
}
