using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Com.Atomatus.Bootstarter.Util
{
    internal sealed class CommonPropertyCopyStrategy : ICopyStrategy
    {
        private static bool TryGetTargetProperty(
            [NotNull] IEnumerable<PropertyInfo> sourceProperties,
            [NotNull] IEnumerable<PropertyInfo> targetProperties,
            [NotNull] PropertyInfo sourceProp,
            out PropertyInfo targetProp)
        {
            if(sourceProperties == targetProperties)
            {
                targetProp = sourceProp;
                return true;
            }
            else
            {
               return (targetProp = targetProperties.FirstOrDefault(p => p.Name == sourceProp.Name)) != null;
            }
        }

        internal static bool TryHandleProperties(
            [NotNull] object source, 
            [NotNull] object target,
            [NotNull] IEnumerable<PropertyInfo> sourceProperties,
            [NotNull] IEnumerable<PropertyInfo> targetProperties)
        {
            bool handled = false;
            foreach (PropertyInfo sourceProp in sourceProperties)
            {
                try
                {
                    if (TryGetTargetProperty(sourceProperties, targetProperties, sourceProp, out PropertyInfo targetProp))
                    {
                        handled = true;
                        object sourceValue = sourceProp.GetValue(source);

                        Type sType = sourceProp.PropertyType;
                        Type tType = targetProp.PropertyType;

                        try
                        {
                            object targetValue = ObjectMapper.Parse(sourceValue, tType);
                            targetProp.SetValue(target, targetValue, null);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            throw;
#else

                            Debug.WriteLine(ex);
                            handled = false;
                            break;
#endif
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    handled = false;
                }
            }
            return handled;
        }

        public bool TryHandle([NotNull] object source, [NotNull] object target)
        {
            Type sType = source.GetType();
            Type tType = target.GetType();

            if (sType == tType)
            {
                var props = source.GetType().GetProperties().Where(e => e.CanRead && e.CanWrite);
                return TryHandleProperties(source, target, props, props);
            }
            else
            {
                return TryHandleProperties(source, target,
                    source.GetType().GetProperties().Where(e => e.CanRead),
                    target.GetType().GetProperties().Where(e => e.CanWrite));
            }
        }
    }
}
