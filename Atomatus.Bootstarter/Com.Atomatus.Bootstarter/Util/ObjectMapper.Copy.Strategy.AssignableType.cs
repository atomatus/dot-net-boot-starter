using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter
{
    internal sealed class AssignableTypeCopyStrategy : ICopyStrategy
    {
        private static bool TryFindCommonType(Type type1, Type type2, out Type commonType)
        {
            ObjectMapper.SolveNullableType(ref type1);
            ObjectMapper.SolveNullableType(ref type2);

            Type originalType2 = type2;

            while (type1 != null)
            {
                type2 = originalType2;

                while (type2 != null)
                {
                    if (type1 == type2 && type1 != typeof(object))
                    {
                        return (commonType = type1) != null;
                    }

                    type2 = type2.BaseType;
                }

                type1 = type1.BaseType;
            }

            commonType = null;
            return false;
        }

        public bool TryHandle([NotNull] object source, [NotNull] object target)
        {
            return TryFindCommonType(source.GetType(), target.GetType(), out Type commonType) &&
                EqualTypeCopyStrategy.TryHandleSameType(source, target, commonType);
        }
    }
}
