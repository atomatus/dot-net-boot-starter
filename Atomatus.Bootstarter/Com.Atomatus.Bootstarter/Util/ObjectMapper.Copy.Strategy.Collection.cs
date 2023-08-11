using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Com.Atomatus.Bootstarter.Util
{
    internal sealed class CollectionCopyStrategy : ICopyStrategy
    {
        public bool TryHandle([NotNull] object source, [NotNull] object target)
        {
            using CopyHandler handler = new CopyHandler(() => new ArrayStrategy());
            handler
                .Next(() => new IDictionaryStrategy())
                .Next(() => new GenericCollectionStrategy())
                .Next(() => new IListStrategy());
            return handler.Handle(source, target);
        }

        private abstract class LocalCollectionStrategy : ICopyStrategy
        {
            protected static bool TryAddToTargetCollection<S, T>(
                S source,
                T target,
                object sItem,
                Type tItemType,
                Action<T, object> addCallback)
            {
                try
                {
                    Type sItemType = sItem.GetType();

                    if (sItemType.IsPrimitive || sItemType == typeof(string) || !sItemType.IsClass)
                    {
                        object item = sItemType == tItemType ? sItem : Convert.ChangeType(sItem, tItemType);
                        addCallback.Invoke(target, item);
                    }
                    else if (sItemType.IsClass)
                    {
                        if (tItemType.IsInterface || tItemType.IsAbstract)
                        {
                            if (tItemType.IsAssignableFrom(sItemType))
                            {
                                addCallback.Invoke(target, sItem);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Is not possible convert source \"{source.GetType().FullName}\" to target type \"{target.GetType().FullName}\",\r\n" +
                                                                        $"because source element type \"{sItemType.FullName}\" is not assigned " +
                                                                        $"to target element type \"{tItemType}\" (incompatible types).");
                            }
                        }
                        else
                        {
                            object item = Activator.CreateInstance(tItemType);
                            if (ObjectMapper.Copy(sItem, item))
                            {
                                addCallback.Invoke(target, item);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Is not possible convert source \"{source.GetType().FullName}\" to target type \"{target.GetType().FullName}\",\r\n" +
                                    $"because source element type \"{sItem.GetType().FullName}\" is not convertible " +
                                    $"to target element type \"{item.GetType()}\".");
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Is not possible convert source \"{source.GetType().FullName}\" to target type \"{target.GetType().FullName}\".");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
#if DEBUG
                    throw;
#else
                            return false;
#endif
                }
                return true;
            }

            bool ICopyStrategy.TryHandle([NotNull] object source, [NotNull] object target)
            {
                return source is IEnumerable sCollection && TryHandle(sCollection, target);
            }

            public abstract bool TryHandle([NotNull] IEnumerable source, [NotNull] object target);
        }

        private class ArrayStrategy : LocalCollectionStrategy
        {
            public override bool TryHandle([NotNull] IEnumerable source, [NotNull] object target)
            {
                bool handled = false;
                if (target is Array array)
                {
                    Type tItemType = array.GetType().GetElementType();

                    int i = 0;
                    int l = array.Length;
                    foreach (var e in source)
                    {
                        if (i == l)
                        {
                            break;
                        }

                        handled |= TryAddToTargetCollection(source, array, e, tItemType,
                            (t, e) => t.SetValue(e, i++));
                    }
                }
                return handled;
            }
        }

        private class GenericCollectionStrategy : LocalCollectionStrategy
        {
            protected static bool IsGenericTypeAndImplementingInterface(Type type, Type interfaceType)
            {
                return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
            }

            public override bool TryHandle([NotNull] IEnumerable source, [NotNull] object target)
            {
                bool handled = false;
                if (IsGenericTypeAndImplementingInterface(target.GetType(), typeof(ICollection<>)))
                {
                    Type tType = target.GetType();
                    Type tItemType = tType.GetGenericArguments()[0];
                    MethodInfo addMethod = tType.GetMethod("Add");

                    foreach (var sItem in source)
                    {
                        handled |= TryAddToTargetCollection(source, target, sItem, tItemType,
                            (t, i) => addMethod.Invoke(t, new object[] { i }));
                    }
                }
                return handled;
            }
        }

        private class IDictionaryStrategy : GenericCollectionStrategy
        {
            private static bool IsKeyValuePair(object obj)
            {
                Type keyValuePairType = typeof(KeyValuePair<,>);
                Type objectType = obj.GetType();

                return objectType.IsGenericType &&
                       objectType.GetGenericTypeDefinition() == keyValuePairType;
            }

            private static object GetKeyValue(object keyValuePair, string propertyName)
            {
                Type keyValuePairType = keyValuePair.GetType();
                PropertyInfo propertyInfo = keyValuePairType.GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(keyValuePair);
                }

                throw new ArgumentException($"Property '{propertyName}' not found in KeyValuePair.");
            }

            private static bool TryParseKeyValue(object keyValuePairItem, Type[] targetDictionaryKeyValueTypes, 
                int keyValeTypeIndex, string keyValuePropName, out object keyValue)
            {
                if (targetDictionaryKeyValueTypes.Length != 2) //object dictionary
                {
                    keyValue = GetKeyValue(keyValuePairItem, keyValuePropName);
                }
                else //generic dictionary
                {
                    Type sourceKeyValueType = keyValuePairItem.GetType().GetGenericArguments()[keyValeTypeIndex];
                    Type targetKeyValueType = targetDictionaryKeyValueTypes[keyValeTypeIndex];

                    if (targetKeyValueType.IsAssignableFrom(sourceKeyValueType))
                    {
                        keyValue = GetKeyValue(keyValuePairItem, keyValuePropName);
                    }
                    else
                    {
                        object sValue = GetKeyValue(keyValuePairItem, keyValuePropName);

                        try
                        {
                            keyValue = ObjectMapper.Parse(sValue, targetKeyValueType);
                        } 
                        catch (Exception ex)
                        {
#if DEBUG
                            throw;
#else
                            Debug.WriteLine(ex);
                            keyValue = null;
                            return false;//ignore this pair.
#endif
                        }
                    }
                }
                return true;
            }

            public override bool TryHandle([NotNull] IEnumerable source, [NotNull] object target)
            {
                bool handled = false;
                if(target is IDictionary dictionary)
                {
                    Type dictionaryType = dictionary.GetType();
                    Type[] targetDictKeyValueType = IsGenericTypeAndImplementingInterface(dictionaryType, typeof(IDictionary<,>)) ?
                        dictionaryType.GetGenericArguments() : Array.Empty<Type>();

                    foreach (var s in source)
                    {
                        if(s != null)
                        {
                            if (IsKeyValuePair(s))
                            {
                                if (TryParseKeyValue(s, targetDictKeyValueType, 0, "Key", out object key) &&
                                    TryParseKeyValue(s, targetDictKeyValueType, 1, "Value", out object value))
                                {
                                    dictionary.Add(key, value);
                                    handled = true;
                                }
                            }
                            else if (targetDictKeyValueType.Length == 2)
                            {
                                try
                                {
                                    Type targetKeyType = targetDictKeyValueType[0];
                                    Type targetValueType = targetDictKeyValueType[1];
                                    object key = ObjectMapper.Parse(s, targetKeyType);
                                    dictionary.Add(key, ObjectMapper.GetDefaultValue(targetValueType));
                                    handled = true;
                                } 
                                catch (Exception ex)
                                {
#if DEBUG
                                    throw;
#else
                                    Debug.WriteLine(ex);
                                    continue;
#endif
                                }
                            }
                            else
                            {
                                dictionary.Add(s, null);
                                handled = true;
                            }
                        }
                    }
                }
                return handled;
            }
        }

        private sealed class IListStrategy : LocalCollectionStrategy
        {
            public override bool TryHandle([NotNull] IEnumerable source, [NotNull] object target)
            {
                bool handled = false;
                if (target is IList tCollection)
                {
                    foreach (var e in source)
                    {
                        handled |= TryAddToTargetCollection(source, tCollection, e, e.GetType(),
                            (t, e) => t.Add(e));
                    }
                }
                return handled;
            }
        }
    }
}
