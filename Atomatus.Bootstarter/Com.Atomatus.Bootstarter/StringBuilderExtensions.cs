using System;
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
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool condition, params object[] value)
        {
            return condition ? value.Aggregate(sb, (acc, curr) => acc.Append(curr)).AppendLine() : sb;
        }

        public static StringBuilder AppendIf(this StringBuilder sb, bool condition, params object[] value)
        {
            return condition ? value.Aggregate(sb, (acc, curr) => acc.Append(curr)) : sb;
        }

        public static StringBuilder AppendOrElse(this StringBuilder sb, string target, string elseOption)
        {
            return !string.IsNullOrEmpty(target) ?
                sb.Append(target) :
                sb.Append(elseOption);
        }

        public static StringBuilder AppendOrElse(this StringBuilder sb, int? target, int elseOption)
        {
            return target.HasValue && target.Value > 0 ? 
                sb.Append(target) : 
                sb.Append(elseOption);
        }

        public static StringBuilder AppendOrThrow(this StringBuilder sb, string target, string errorMessage)
        {
            return !string.IsNullOrEmpty(target) ? 
                sb.Append(target) : 
                throw new ArgumentException(errorMessage);
        }

    }
}
