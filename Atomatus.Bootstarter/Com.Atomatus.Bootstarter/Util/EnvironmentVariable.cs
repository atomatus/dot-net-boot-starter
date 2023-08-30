using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// <para><b>Environment Variable</b></para> 
    /// <para>
    /// System Environment variable abstraction how structure.
    /// </para>
    /// <para>    
    /// <strong>Allow to load environment variable by your name:</strong>
    /// </para>
    /// <para>
    /// <i>
    /// <list type="table">
    /// <item>● By constructor; </item>
    /// <item>● [Or] The static method <c><see cref="GetFromProcess(string, object)"/></c>;</item>
    /// <item>● [Or] The static method <c><see cref="GetFromUser(string, object)"/></c>;</item>
    /// <item>● [Or] The static method <c><see cref="GetFromMachine(string, object)"/></c>;</item>
    /// <item>● [Or] Implicit operator through a simple text <c><see cref="EnvironmentVariable"/> env = "env_name"</c>.</item>
    /// <item>● [Or] Implicit Tuples <c><see cref="EnvironmentVariable"/> env = ("env_name").</c></item>
    /// <item>● [Or] Implicit Tuples ("env_name", [default_value]).</item>
    /// <item>● [Or] Implicit Tuples ("env_name", [default_value], <see cref="EnvironmentVariableTarget"/>).</item>
    /// <item>● [Or] Implicit Tuples ("env_name", [default_number_value], [min_value]).</item>
    /// <item>● [Or] Implicit Tuples ("env_name", [default_number_value], [min_value], <see cref="EnvironmentVariableTarget"/>).</item>
    /// <item>● [Or] Implicit Tuples ("env_name", [default_number_value], [min_value], [max_value]).</item>
    /// <item>● [Or] Implicit Tuples ("env_name", [default_number_value], [min_value], [max_value], <see cref="EnvironmentVariableTarget"/>).</item>
    /// </list>
    /// </i>
    /// </para>
    /// </summary>
    /// <author>Carlos Matos</author>
    /// <date>2021-01-19</date>    
    public readonly struct EnvironmentVariable :
        IComparable, IComparable<EnvironmentVariable>,
        IConvertible, IEquatable<EnvironmentVariable>
    {
        private delegate bool TryParse<T>(string value, out T result);

        private readonly string name;
        private readonly object defaultValue;
        private readonly EnvironmentVariableTarget target;
        
        private object SyncRoot => string.Intern($"{nameof(EnvironmentVariable)}#{target}_{name}");

        /// <summary>
        /// Gets the name of the environment variable.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the value of the environment variable.
        /// </summary>
        public string Value
        {
            get
            {
                lock (SyncRoot)
                {
                    return Environment.GetEnvironmentVariable(name, target) ?? defaultValue?.ToString() ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Indicates whether the current environment variable value is null or empty.
        /// </summary>
        public bool IsNullOrEmpty => string.IsNullOrEmpty(Value);

        /// <summary>
        /// Indicates whether current environment variable has a value.
        /// </summary>
        public bool HasValue => !IsNullOrEmpty;

        /// <summary>
        /// Load system environment variable by name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="defaultValue">default value to be used whether the variable not found or current value is empty</param>
        /// <param name="target">specifies the location where an environment variable is stored or retrieved</param>
        private EnvironmentVariable(string name, object defaultValue = null, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            this.name           = name ?? throw new ArgumentNullException(nameof(name));
            this.defaultValue   = defaultValue;
            this.target         = target;
        }

        /// <summary>
        /// Try get environment variable value whether non null and non empty.
        /// </summary>
        /// <param name="value">recovered non null and non empty value</param>
        /// <returns></returns>
        public bool TryGetValue(out string value)
        {
            lock (SyncRoot)
            {
                value = Environment.GetEnvironmentVariable(name, target) ?? defaultValue?.ToString() ?? string.Empty;
                return !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// Get environment variable value or default value whether current variable not set.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetValueOrDefault(object defaultValue)
        {
            string aux = Value;
            return string.IsNullOrWhiteSpace(aux) ? defaultValue?.ToString() ?? string.Empty : aux;
        }

        /// <summary>
        /// Creates, modifies, or deletes an environment variable stored in the current process
        /// or in the Windows operating system registry key reserved for the current user
        /// or local machine.
        /// </summary>
        /// <param name="value">
        /// A value to assign to variable
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// variable contains a zero-length string, an initial hexadecimal zero character
        /// (0x00), or an equal sign ("="). -or- The length of variable or value is greater
        /// than or equal to 32,767 characters. -or- An error occurred during the execution
        /// of this operation.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission to perform this operation.
        /// </exception>
        public void SetValue(string value)
        {
            lock (SyncRoot)
            {
                Environment.SetEnvironmentVariable(name, value, target);
            }
        }

        private T ParseOrDefault<T>(TryParse<T> tryParse) => tryParse.Invoke(Value, out T result) ? result : default;

        /// <inheritdoc/>
        public override string ToString() => Value;

        #region IComparable
        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            return obj is EnvironmentVariable otherEnv ? this.CompareTo(otherEnv) : -1;
        }

        /// <inheritdoc/>
        public int CompareTo(EnvironmentVariable otherEnv)
        {
            int i;
            return (this == otherEnv ? 0 :
                (i = this.Name?.CompareTo(otherEnv.Name) ?? -1) == 0 ?
                (this.Value?.CompareTo(otherEnv.Value) ?? -1) : i);
        }
        #endregion

        #region IConvertible
        /// <inheritdoc/>
        public TypeCode GetTypeCode() => TypeCode.String;

        /// <inheritdoc/>
        public bool ToBoolean(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public byte ToByte(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public char ToChar(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public DateTime ToDateTime(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public decimal ToDecimal(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public double ToDouble(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public short ToInt16(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public int ToInt32(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public long ToInt64(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public sbyte ToSByte(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public float ToSingle(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public string ToString(IFormatProvider provider) => this.Value;

        /// <inheritdoc/>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(this.Value, conversionType);
        }

        /// <inheritdoc/>
        public ushort ToUInt16(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public uint ToUInt32(IFormatProvider provider = null) => this;

        /// <inheritdoc/>
        public ulong ToUInt64(IFormatProvider provider = null) => this;
        #endregion

        #region IEquatable
        /// <inheritdoc/>
        public bool Equals(EnvironmentVariable other)
        {
            return this.CompareTo(other) == 0;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region BitwiseOperators
        /// <summary>
        /// Performs a bitwise AND operation on two <see cref="EnvironmentVariable"/> instances, treating them as long integers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result of the bitwise AND operation.</returns>
        public static Number operator &(EnvironmentVariable left, EnvironmentVariable right)
        {
            return ((long)left) & ((long)right);
        }

        /// <summary>
        /// Performs a bitwise OR operation on two <see cref="EnvironmentVariable"/> instances, treating them as long integers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result of the bitwise OR operation.</returns>
        public static Number operator |(EnvironmentVariable left, EnvironmentVariable right)
        {
            return ((long)left) | ((long)right);
        }

        /// <summary>
        /// Performs a bitwise XOR operation on two <see cref="EnvironmentVariable"/> instances, treating them as long integers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result of the bitwise XOR operation.</returns>
        public static Number operator ^(EnvironmentVariable left, EnvironmentVariable right)
        {
            return ((long)left) ^ ((long)right);
        }

        /// <summary>
        /// Performs a bitwise NOT operation on a <see cref="EnvironmentVariable"/> instance, treating it as a long integer.
        /// </summary>
        /// <param name="value">The operand.</param>
        /// <returns>The result of the bitwise NOT operation.</returns>
        public static Number operator ~(EnvironmentVariable value)
        {
            return ~((long)value);
        }
        #endregion

        #region Comparer Operators
        /// <summary>
        /// Determines whether two <see cref="EnvironmentVariable"/> instances are equal.
        /// </summary>
        /// <param name="env0">The first instance to compare.</param>
        /// <param name="env1">The second instance to compare.</param>
        /// <returns>True if the instances are equal; otherwise, false.</returns>
        public static bool operator ==(EnvironmentVariable env0, EnvironmentVariable env1)
        {
            return env0.GetHashCode() == env1.GetHashCode();
        }

        /// <summary>
        /// Determines whether two <see cref="EnvironmentVariable"/> instances are not equal.
        /// </summary>
        /// <param name="env0">The first instance to compare.</param>
        /// <param name="env1">The second instance to compare.</param>
        /// <returns>True if the instances are not equal; otherwise, false.</returns>
        public static bool operator !=(EnvironmentVariable env0, EnvironmentVariable env1)
        {
            return env0.GetHashCode() != env1.GetHashCode();
        }

        /// <summary>
        /// Compares two <see cref="EnvironmentVariable"/> instances to determine if the first is less than the second.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>True if the first instance is less than the second; otherwise, false.</returns>
        public static bool operator <(EnvironmentVariable left, EnvironmentVariable right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Compares two <see cref="EnvironmentVariable"/> instances to determine if the first is less than or equal to the second.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>True if the first instance is less than or equal to the second; otherwise, false.</returns>
        public static bool operator <=(EnvironmentVariable left, EnvironmentVariable right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Compares two <see cref="EnvironmentVariable"/> instances to determine if the first is greater than the second.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>True if the first instance is greater than the second; otherwise, false.</returns>
        public static bool operator >(EnvironmentVariable left, EnvironmentVariable right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Compares two <see cref="EnvironmentVariable"/> instances to determine if the first is greater than or equal to the second.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>True if the first instance is greater than or equal to the second; otherwise, false.</returns>
        public static bool operator >=(EnvironmentVariable left, EnvironmentVariable right)
        {
            return left.CompareTo(right) >= 0;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implicitly converts a string to an <see cref="EnvironmentVariable"/> instance with the specified variable name.
        /// </summary>
        /// <param name="variableName">The variable name.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable(string variableName) => new EnvironmentVariable(variableName);

        /// <summary>
        /// Implicitly converts a tuple containing a string and an object to an <see cref="EnvironmentVariable"/> instance with the specified variable name and default value.
        /// </summary>
        /// <param name="pair">A tuple containing the variable name and default value.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable((string, object) pair) => new EnvironmentVariable(pair.Item1, pair.Item2);

        /// <summary>
        /// Implicitly converts a tuple containing a string and a numeric value to an <see cref="EnvironmentVariable"/> instance with the specified variable name and default numeric value.
        /// </summary>
        /// <param name="pair">A tuple containing the variable name and default numeric value.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable((string, int) pair) => new EnvironmentVariable(pair.Item1, pair.Item2);

        /// <summary>
        /// Implicitly converts a tuple containing a string, an object, and an <see cref="EnvironmentVariableTarget"/> value to an <see cref="EnvironmentVariable"/> instance with the specified variable name, default value, and target.
        /// </summary>
        /// <param name="pair">A tuple containing the variable name, default value, and target.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable((string, object, EnvironmentVariableTarget) pair) => new EnvironmentVariable(pair.Item1, pair.Item2, pair.Item3);

        /// <summary>
        /// Implicitly converts a tuple containing a string and an <see cref="EnvironmentVariableTarget"/> value to an <see cref="EnvironmentVariable"/> instance with the specified variable name and target.
        /// </summary>
        /// <param name="pair">A tuple containing the variable name and target.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable((string, EnvironmentVariableTarget) pair) => new EnvironmentVariable(pair.Item1, target: pair.Item2);

        /// <summary>
        /// Implicitly converts a tuple containing a string and a numeric value to an <see cref="EnvironmentVariable"/> instance with the specified variable name and default numeric value.
        /// </summary>
        /// <param name="pair">A tuple containing the variable name, default and minimal numeric value.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable((string, int, int) pair) => (pair.Item1, pair.Item2, pair.Item3, int.MaxValue);

        /// <summary>
        /// Implicitly converts a tuple containing a string and a numeric value to an <see cref="EnvironmentVariable"/> instance with the specified variable name and default numeric value.
        /// </summary>
        /// <param name="pair">A tuple containing the variable name, default and minimal numeric value.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable((string, long, long) pair) => (pair.Item1, pair.Item2, pair.Item3, long.MaxValue);

        /// <summary>
        /// Implicitly converts a tuple containing a string and a numeric value to an <see cref="EnvironmentVariable"/> instance with the specified variable name and default numeric value.
        /// </summary>
        /// <param name="pair">A tuple containing the variable name, default and minimal numeric value.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable((string, double, double) pair) => (pair.Item1, pair.Item2, pair.Item3, double.MaxValue);

        /// <summary>
        /// Implicitly converts a tuple containing a string and a numeric value to an <see cref="EnvironmentVariable"/> instance with the specified variable name and default numeric value.
        /// </summary>
        /// <param name="pair">A tuple containing the variable name, default, minimal and maximum numeric value.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable((string, Number, Number, Number) pair) => new EnvironmentVariable(pair.Item1, pair.Item4.Min(pair.Item2.Max(pair.Item3)));

        /// <summary>
        /// Implicitly converts a tuple containing a string to an <see cref="EnvironmentVariable"/> instance with the specified variable name.
        /// </summary>
        /// <param name="tuple">A tuple containing the variable name.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable(Tuple<string> tuple) => new EnvironmentVariable(tuple.Item1);

        /// <summary>
        /// Implicitly converts a tuple containing a string and an object to an <see cref="EnvironmentVariable"/> instance with the specified variable name and default value.
        /// </summary>
        /// <param name="tuple">A tuple containing the variable name and default value.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable(Tuple<string, object> tuple) => new EnvironmentVariable(tuple.Item1, tuple.Item2);

        /// <summary>
        /// Implicitly converts a tuple containing a string, an object, and an <see cref="EnvironmentVariableTarget"/> value to an <see cref="EnvironmentVariable"/> instance with the specified variable name, default value, and target.
        /// </summary>
        /// <param name="tuple">A tuple containing the variable name, default value, and target.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable(Tuple<string, object, EnvironmentVariableTarget> tuple) => new EnvironmentVariable(tuple.Item1, tuple.Item2, tuple.Item3);

        /// <summary>
        /// Implicitly converts a tuple containing a string and an <see cref="EnvironmentVariableTarget"/> value to an <see cref="EnvironmentVariable"/> instance with the specified variable name and target.
        /// </summary>
        /// <param name="tuple">A tuple containing the variable name and target.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable(Tuple<string, EnvironmentVariableTarget> tuple) => new EnvironmentVariable(tuple.Item1, target: tuple.Item2);

        /// <summary>
        /// Implicitly converts a key-value pair containing a string and an object to an <see cref="EnvironmentVariable"/> instance with the specified variable name and default value.
        /// </summary>
        /// <param name="tuple">A key-value pair containing the variable name and default value.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable(KeyValuePair<string, object> tuple) => new EnvironmentVariable(tuple.Key, tuple.Value);

        /// <summary>
        /// Implicitly converts a key-value pair containing a string and an <see cref="EnvironmentVariableTarget"/> value to an <see cref="EnvironmentVariable"/> instance with the specified variable name and target.
        /// </summary>
        /// <param name="tuple">A key-value pair containing the variable name and target.</param>
        /// <returns>An <see cref="EnvironmentVariable"/> instance.</returns>
        public static implicit operator EnvironmentVariable(KeyValuePair<string, EnvironmentVariableTarget> tuple) => new EnvironmentVariable(tuple.Key, target: tuple.Value);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a string, retrieving its value.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a string.</returns>
        public static implicit operator string(EnvironmentVariable env) => env.Value;

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a byte.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a byte.</returns>
        public static implicit operator byte(EnvironmentVariable env) => env.ParseOrDefault<byte>(byte.TryParse);
        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a signed byte.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a signed byte.</returns>
        public static implicit operator sbyte(EnvironmentVariable env) => env.ParseOrDefault<sbyte>(sbyte.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a boolean.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a boolean.</returns>
        public static implicit operator bool(EnvironmentVariable env) => env.ParseOrDefault<bool>(bool.TryParse) || bool.TrueString.Equals(env.Value, StringComparison.InvariantCultureIgnoreCase) || env == 1;

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a character.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a character.</returns>
        public static implicit operator char(EnvironmentVariable env) => env.ParseOrDefault<char>(char.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a short integer.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a short integer.</returns>
        public static implicit operator short(EnvironmentVariable env) => env.ParseOrDefault<short>(short.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to an unsigned short integer.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as an unsigned short integer.</returns>
        public static implicit operator ushort(EnvironmentVariable env) => env.ParseOrDefault<ushort>(ushort.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to an integer.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as an integer.</returns>
        public static implicit operator int(EnvironmentVariable env) => env.ParseOrDefault<int>(int.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to an unsigned integer.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as an unsigned integer.</returns>
        public static implicit operator uint(EnvironmentVariable env) => env.ParseOrDefault<uint>(uint.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a long integer.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a long integer.</returns>
        public static implicit operator long(EnvironmentVariable env) => env.ParseOrDefault<long>(long.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to an unsigned long integer.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as an unsigned long integer.</returns>
        public static implicit operator ulong(EnvironmentVariable env) => env.ParseOrDefault<ulong>(ulong.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a single-precision floating-point number.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a single-precision floating-point number.</returns>
        public static implicit operator float(EnvironmentVariable env) => env.ParseOrDefault<float>(float.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a double-precision floating-point number.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a double-precision floating-point number.</returns>
        public static implicit operator double(EnvironmentVariable env) => env.ParseOrDefault<double>(double.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a decimal number.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a decimal number.</returns>
        public static implicit operator decimal(EnvironmentVariable env) => env.ParseOrDefault<decimal>(decimal.TryParse);

        /// <summary>
        /// Implicitly converts an <see cref="EnvironmentVariable"/> instance to a <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="env">The <see cref="EnvironmentVariable"/> instance.</param>
        /// <returns>The value of the environment variable as a <see cref="DateTime"/> instance.</returns>
        public static implicit operator DateTime(EnvironmentVariable env) => env.ParseOrDefault<DateTime>(DateTime.TryParse);
        #endregion

        #region Get
        /// <summary>
        /// Load a environment variable by your name from current process.
        /// </summary>
        /// <param name="variableName">variable name</param>
        /// <param name="defaultValue">default value to be used whether the variable not found or current value is empty</param>
        /// <returns></returns>
        public static EnvironmentVariable GetFromProcess([CallerMemberName] string variableName = null, object defaultValue = null)
        {
            return new EnvironmentVariable(variableName, defaultValue);
        }

        /// <summary>
        /// Load a environment variable by your name from current user.
        /// </summary>
        /// <param name="variableName">variable name</param>
        /// <param name="defaultValue">default value to be used whether the variable not found or current value is empty</param>
        /// <returns></returns>
        public static EnvironmentVariable GetFromUser([CallerMemberName] string variableName = null, object defaultValue = null)
        {
            return new EnvironmentVariable(variableName, defaultValue, EnvironmentVariableTarget.User);
        }

        /// <summary>
        /// Load a environment variable by your name from local machine.
        /// </summary>
        /// <param name="variableName">variable name</param>
        /// <param name="defaultValue">default value to be used whether the variable not found or current value is empty</param>
        /// <returns></returns>
        public static EnvironmentVariable GetFromMachine([CallerMemberName] string variableName = null, object defaultValue = null)
        {
            return new EnvironmentVariable(variableName, defaultValue, EnvironmentVariableTarget.Machine);
        }
        #endregion
    }
}
