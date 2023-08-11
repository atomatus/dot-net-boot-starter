using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Com.Atomatus.Bootstarter.Util
{
    internal sealed class EqualTypeCopyStrategy : ICopyStrategy
    {
        internal static bool TryHandleSameType([NotNull] ICopyStrategy strategy,[NotNull] object source, [NotNull] object target, Type sourceType)
        {
            #region Collection
            CollectionCopyStrategy collectionCopy = new CollectionCopyStrategy();
            if (collectionCopy.TryHandle(source, target))
            {
                return true;
            }
            #endregion

            #region Properties
            PropertyInfo[] sourceProperties = sourceType.GetProperties();

            if(sourceProperties.Length == 0)
            {
                throw new InvalidOperationException($"Source type {sourceType.FullName} does not contains no one public property!");
            }

            foreach (PropertyInfo property in sourceProperties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    object value = property.GetValue(source);

                    if (value != null)
                    {
                        if (property.PropertyType.IsPrimitive ||
                            property.PropertyType == typeof(string) ||
                            !property.PropertyType.IsClass)
                        {
                            property.SetValue(target, value);
                        }
                        else
                        {
                            object valueCopy = Activator.CreateInstance(property.PropertyType);
                            if (strategy.TryHandle(value, valueCopy))
                            {
                                property.SetValue(target, valueCopy);
                            }
#if DEBUG
                            else
                            {
                                //This exception never can be throwed.
                                throw new InvalidOperationException($"The property {property.Name} of class {sourceType.FullName} is not compatible with the target type {target.GetType().FullName}!");
                            }
#endif
                        }
                    }
                }
            }
            return true;
            #endregion
        }

        public bool TryHandle([NotNull] object source, [NotNull] object target)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();
            return sourceType == targetType && TryHandleSameType(this, source, target, sourceType);
        }
    }
}
