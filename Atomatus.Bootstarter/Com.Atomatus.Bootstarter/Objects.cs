using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Com.Atomatus.Bootstarter
{
    internal static class Objects
    {
        /// <summary>
        /// <para>
        /// Returns a hash code based on the contents of the specified array.If
        /// the array contains other arrays as elements, the hash code is based on
        /// their identities rather than their contents.It is therefore
        /// acceptable to invoke this method on an array that contains itself as an
        /// element, either directly or indirectly through one or more levels of
        /// arrays.
        /// </para>
        ///
        /// <para>
        /// For any two arrays "a" and "b" such that
        /// "Arrays.Equals(a, b)", it is also the case that
        /// "a.GetHashCode() == b.GetHashCode()".
        /// </para>
        ///
        /// <para>
        /// The value returned by this method is equal to the value that would
        /// be returned by" a.ToList().GetHashCode()", unless" a"
        /// is "null", in which case "0" is returned.
        /// </para>
        /// 
        /// </summary>
        /// <param name="args">the array whose content-based hash code to compute</param>
        /// <returns>content-based hash code for "args"</returns>
        public static int GetHashCode(params object[] args)
        {
            return args.Length == 0 ? 0 : GetHashCode((IEnumerable) args);
        }

        /// <summary>
        /// <para>
        /// Returns a hash code based on the contents of the specified array.If
        /// the array contains other arrays as elements, the hash code is based on
        /// their identities rather than their contents.It is therefore
        /// acceptable to invoke this method on an array that contains itself as an
        /// element, either directly or indirectly through one or more levels of
        /// arrays.
        /// </para>
        ///
        /// <para>
        /// For any two arrays "a" and "b" such that
        /// "Array.Equals(a, b)", it is also the case that
        /// "a.GetHashCode() == b.GetHashCode()".
        /// </para>
        ///
        /// <para>
        /// The value returned by this method is equal to the value that would
        /// be returned by" a.ToList().GetHashCode()", unless" a"
        /// is "null", in which case "0" is returned.
        /// </para>
        /// 
        /// </summary>
        /// <param name="en">the array whose content-based hash code to compute</param>
        /// <returns>content-based hash code for "en"</returns>
        public static int GetHashCode([AllowNull] IEnumerable en)
        {            
            if(en is null)
            {
                return 0;
            }

            int result = 1;
            int weight = 31;

            foreach (object e in en)
            {
                result = e is IEnumerable aux ?
                        GetHashCode(aux) :
                        weight * result + (e?.GetHashCode() ?? 0);
            }

            return result;
        }

        /// <summary>
        /// <para>
        /// Returns a hash code based on the contents of the specified target object public fields like an array.
        /// If the array contains other arrays as elements, the hash code is based on
        /// their identities rather than their contents.It is therefore
        /// acceptable to invoke this method on an array that contains itself as an
        /// element, either directly or indirectly through one or more levels of
        /// arrays.
        /// </para>
        ///
        /// </summary>
        /// <param name="target">target object</param>
        /// <returns>content-based hash code for "target"</returns>
        public static int GetHashCodeFromPublicFields([DisallowNull] this object target)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            return target is null ? 0 : GetHashCode(
                   target.GetType().GetFields(flags).Select(f => f.GetValue(target)).Union(
                   target.GetType().GetProperties(flags).Select(f => f.GetValue(target))));
        }

        /// <summary>
        /// Compare generic types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static bool Compare<T>([AllowNull] T t0, [AllowNull] T t1)
        {
            return (t0 == null && t1 == null) ||
                (t0 != null && t1 != null && 
                EqualityComparer<T>.Default.Equals(t0, t1));
        }

    }
}
