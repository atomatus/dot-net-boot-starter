﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Postgres")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlite")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlserver")]
namespace Com.Atomatus.Bootstarter
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineIf([NotNull] this StringBuilder sb, bool condition, params object[] value)
        {
            return condition ? value.Aggregate(sb, (acc, curr) => acc.Append(curr)).AppendLine() : sb;
        }

        public static StringBuilder AppendIf([NotNull] this StringBuilder sb, bool condition, params object[] value)
        {
            return condition ? value.Aggregate(sb, (acc, curr) => acc.Append(curr)) : sb;
        }

        public static StringBuilder AppendOrElse([NotNull] this StringBuilder sb, [AllowNull] string target, [NotNull] string elseOption)
        {
            return !string.IsNullOrEmpty(target) ?
                sb.Append(target) :
                sb.Append(elseOption);
        }

        public static StringBuilder AppendOrElse([NotNull] this StringBuilder sb, [AllowNull] int? target, int elseOption)
        {
            return target.HasValue && target.Value > 0 ? 
                sb.Append(target) : 
                sb.Append(elseOption);
        }

        public static StringBuilder AppendOrThrow([NotNull] this StringBuilder sb, [AllowNull] string target, [NotNull] string errorMessage)
        {
            return !string.IsNullOrEmpty(target) ? 
                sb.Append(target) : 
                throw new ArgumentException(errorMessage);
        }

    }
}
