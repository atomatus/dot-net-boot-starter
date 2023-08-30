namespace Com.Atomatus.Bootstarter
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The <c>Number</c> struct provides a versatile numeric type that supports various numeric conversions, bitwise operations, and comparisons.
    /// </summary>
    /// <remarks>
    /// This struct implements interfaces for numeric conversion, comparison, bitwise operations, and formatting.
    /// It allows implicit and explicit conversions from/to various numeric types.
    /// </remarks>
    /// <example>
    /// The following example demonstrates the usage of the <c>Number</c> struct for numeric conversions and comparisons:
    /// <code>
    /// Number num1 = 42;
    /// Number num2 = 3.14;
    /// 
    /// int intValue = (int)num1;
    /// double doubleValue = num2;
    /// 
    /// bool comparisonResult = num1 > num2;
    /// </code>
    /// </example>
    /// <author>Carlos Matos</author>
    /// <date>2023-08-30</date>    
    [Serializable]
    public readonly struct Number :
        IComparable, IConvertible,
        ISpanFormattable, IFormattable,
        IComparable<Number>, IEquatable<Number>
    {
        private const double EPSILON = 1e-9;//0.000000001

        private readonly object value;

        private Number([NotNull] object value)
        {
            this.value = value;
        }

        #region implicit operators

        #region To Number
        /// <summary>
        /// Implicitly converts a <see cref="bool"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="bool"/> value to convert.</param>
        public static implicit operator Number(bool val) => new Number(val ? 1 : 0);

        /// <summary>
        /// Implicitly converts a <see cref="sbyte"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="sbyte"/> value to convert.</param>
        public static implicit operator Number(sbyte val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="byte"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="byte"/> value to convert.</param>
        public static implicit operator Number(byte val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="ushort"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="ushort"/> value to convert.</param>
        public static implicit operator Number(ushort val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="short"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="short"/> value to convert.</param>
        public static implicit operator Number(short val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="uint"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="uint"/> value to convert.</param>
        public static implicit operator Number(uint val) => new Number(val);

        /// <summary>
        /// Implicitly converts an <see cref="int"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="int"/> value to convert.</param>
        public static implicit operator Number(int val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="ulong"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="ulong"/> value to convert.</param>
        public static implicit operator Number(ulong val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="long"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="long"/> value to convert.</param>
        public static implicit operator Number(long val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="float"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="float"/> value to convert.</param>
        public static implicit operator Number(float val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="double"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="double"/> value to convert.</param>
        public static implicit operator Number(double val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="decimal"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="decimal"/> value to convert.</param>
        public static implicit operator Number(decimal val) => new Number(val);

        /// <summary>
        /// Implicitly converts a <see cref="char"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="char"/> value to convert.</param>
        public static implicit operator Number(char val) => new Number(Convert.ToInt16(val));

        /// <summary>
        /// Implicitly converts a <see cref="string"/> value to a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="val">The <see cref="string"/> value to convert.</param>
        public static implicit operator Number(string val) => new Number(double.TryParse(val, out double parsedValue) ? parsedValue : 0d);
        #endregion

        #region From Number
        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="bool"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator bool(Number num) => num > 0;

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="sbyte"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator sbyte(Number num) => Convert.ToSByte(Math.Max(Math.Min((long)num, sbyte.MaxValue), sbyte.MinValue));

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="byte"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator byte(Number num) => Convert.ToByte(Math.Max(Math.Min((long)num, byte.MaxValue), byte.MinValue));

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="ushort"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator ushort(Number num) => Convert.ToUInt16(Math.Max(Math.Min((long)num, ushort.MaxValue), ushort.MinValue));

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="short"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator short(Number num) => Convert.ToInt16(Math.Max(Math.Min((long)num, short.MaxValue), short.MinValue));

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="uint"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator uint(Number num) => Convert.ToUInt32(Math.Max(Math.Min((long)num, uint.MaxValue), uint.MinValue));

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to an <see cref="int"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator int(Number num) => Convert.ToInt32(Math.Max(Math.Min((long)num, int.MaxValue), int.MinValue));

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="ulong"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator ulong(Number num) => Convert.ToUInt64(num.value);

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="long"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator long(Number num) => Convert.ToInt64(num.value);

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="float"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator float(Number num) => Convert.ToSingle(Math.Max(Math.Min((double)num, float.MaxValue), float.MinValue));

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="double"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator double(Number num) => Convert.ToDouble(num.value);

        /// <summary>
        /// Implicitly converts a <see cref="Number"/> instance to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="num">The <see cref="Number"/> instance to convert.</param>
        public static implicit operator decimal(Number num) => Convert.ToDecimal(num.value);
        #endregion

        #endregion

        #region Comparer Operators
        /// <summary>
        /// Checks whether two <see cref="Number"/> instances are equal within a small epsilon range.
        /// </summary>
        /// <param name="n0">The first <see cref="Number"/> to compare.</param>
        /// <param name="n1">The second <see cref="Number"/> to compare.</param>
        /// <returns><c>true</c> if the two instances are equal within the epsilon range, otherwise <c>false</c>.</returns>
        public static bool operator ==(Number n0, Number n1)
        {
            return Math.Abs((double)n0 - (double)n1) < EPSILON;
        }

        /// <summary>
        /// Checks whether two <see cref="Number"/> instances are not equal within a small epsilon range.
        /// </summary>
        /// <param name="n0">The first <see cref="Number"/> to compare.</param>
        /// <param name="n1">The second <see cref="Number"/> to compare.</param>
        /// <returns><c>true</c> if the two instances are not equal within the epsilon range, otherwise <c>false</c>.</returns>
        public static bool operator !=(Number n0, Number n1)
        {
            return !(n0 == n1);
        }

        /// <summary>
        /// Checks if the left <see cref="Number"/> instance is less than the right <see cref="Number"/> instance.
        /// </summary>
        /// <param name="left">The left <see cref="Number"/> instance.</param>
        /// <param name="right">The right <see cref="Number"/> instance.</param>
        /// <returns><c>true</c> if the left instance is less than the right instance, otherwise <c>false</c>.</returns>
        public static bool operator <(Number left, Number right)
        {
            return left != right && ((double)left) < ((double)right);
        }

        /// <summary>
        /// Checks if the left <see cref="Number"/> instance is less than or equal to the right <see cref="Number"/> instance.
        /// </summary>
        /// <param name="left">The left <see cref="Number"/> instance.</param>
        /// <param name="right">The right <see cref="Number"/> instance.</param>
        /// <returns><c>true</c> if the left instance is less than or equal to the right instance, otherwise <c>false</c>.</returns>
        public static bool operator <=(Number left, Number right)
        {
            return left == right || ((double)left) <= ((double)right);
        }

        /// <summary>
        /// Checks if the left <see cref="Number"/> instance is greater than the right <see cref="Number"/> instance.
        /// </summary>
        /// <param name="left">The left <see cref="Number"/> instance.</param>
        /// <param name="right">The right <see cref="Number"/> instance.</param>
        /// <returns><c>true</c> if the left instance is greater than the right instance, otherwise <c>false</c>.</returns>
        public static bool operator >(Number left, Number right)
        {
            return left != right && ((double)left) > ((double)right);
        }

        /// <summary>
        /// Checks if the left <see cref="Number"/> instance is greater than or equal to the right <see cref="Number"/> instance.
        /// </summary>
        /// <param name="left">The left <see cref="Number"/> instance.</param>
        /// <param name="right">The right <see cref="Number"/> instance.</param>
        /// <returns><c>true</c> if the left instance is greater than or equal to the right instance, otherwise <c>false</c>.</returns>
        public static bool operator >=(Number left, Number right)
        {
            return left == right || ((double)left) >= ((double)right);
        }
        #endregion

        #region IComparable
        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            return value is IComparable c ? c.CompareTo(obj) : -1;
        }

        /// <inheritdoc/>
        public int CompareTo(Number other)
        {
            return this > other ? 1 : this < other ? -1 : 0;
        }
        #endregion

        #region BitwiseOperators
        /// <summary>
        /// Performs a bitwise AND operation between two <see cref="Number"/> instances.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A <see cref="Number"/> instance representing the result of the bitwise AND operation.</returns>
        public static Number operator &(Number left, Number right)
        {
            long dl = left;
            long dr = right;
            return dl & dr;
        }

        /// <summary>
        /// Performs a bitwise OR operation between two <see cref="Number"/> instances.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A <see cref="Number"/> instance representing the result of the bitwise OR operation.</returns>
        public static Number operator |(Number left, Number right)
        {
            long dl = left;
            long dr = right;
            return dl | dr;
        }

        /// <summary>
        /// Performs a bitwise XOR operation between two <see cref="Number"/> instances.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A <see cref="Number"/> instance representing the result of the bitwise XOR operation.</returns>
        public static Number operator ^(Number left, Number right)
        {
            long dl = left;
            long dr = right;
            return dl ^ dr;
        }

        /// <summary>
        /// Performs a bitwise NOT operation on a <see cref="Number"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="Number"/> operand.</param>
        /// <returns>A <see cref="Number"/> instance representing the result of the bitwise NOT operation.</returns>
        public static Number operator ~(Number value)
        {
            long d = value;
            return ~d;
        }
        #endregion

        #region ISpanFormattable
        /// <inheritdoc/>
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
        {
            if(value is ISpanFormattable sf)
            {
                return sf.TryFormat(destination, out charsWritten, format, provider);
            }
            double d = this;
            return d.TryFormat(destination, out charsWritten, format, provider);
        }

        /// <inheritdoc/>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (value is ISpanFormattable sf)
            {
                return sf.ToString(format, formatProvider);
            }
            double d = this;
            return d.ToString(format, formatProvider);
        }
        #endregion

        #region IEquatable
        /// <inheritdoc/>
        public bool Equals(Number other)
        {
            return this.value == other.value || this == other;
        }
        #endregion

        #region IConvertible
        /// <inheritdoc/>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Double;
        }

        /// <inheritdoc/>
        public bool ToBoolean(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public byte ToByte(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar((short)this);
        }

        /// <inheritdoc/>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return DateTime.FromBinary(this);
        }

        /// <inheritdoc/>
        public decimal ToDecimal(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public double ToDouble(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public short ToInt16(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public int ToInt32(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public long ToInt64(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public sbyte ToSByte(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public float ToSingle(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public string ToString(IFormatProvider provider)
        {
            return this.ToString();
        }

        /// <inheritdoc/>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType((double)value, conversionType);
        }

        /// <inheritdoc/>
        public ushort ToUInt16(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public uint ToUInt32(IFormatProvider provider)
        {
            return this;
        }

        /// <inheritdoc/>
        public ulong ToUInt64(IFormatProvider provider)
        {
            return this;
        }
        #endregion

        #region Math
        /// <summary>
        /// Returns the larger of the two <see cref="Number"/> values.
        /// </summary>
        /// <param name="other">The second <see cref="Number"/> value to compare.</param>
        /// <returns>The larger of the two <see cref="Number"/> values.</returns>
        public Number Max(Number other)
        {
            return Max(this, other);
        }

        /// <summary>
        /// Returns the smaller of the two <see cref="Number"/> values.
        /// </summary>
        /// <param name="other">The second <see cref="Number"/> value to compare.</param>
        /// <returns>The smaller of the two <see cref="Number"/> values.</returns>
        public Number Min(Number other)
        {
            return Min(this, other);
        }

        /// <summary>
        /// Returns the larger of two <see cref="Number"/> values.
        /// </summary>
        /// <param name="left">The first <see cref="Number"/> value to compare.</param>
        /// <param name="right">The second <see cref="Number"/> value to compare.</param>
        /// <returns>The larger of the two <see cref="Number"/> values.</returns>
        public static Number Max(Number left, Number right)
        {
            return left.CompareTo(right) == 1 ? left : right;
        }

        /// <summary>
        /// Returns the smaller of two <see cref="Number"/> values.
        /// </summary>
        /// <param name="left">The first <see cref="Number"/> value to compare.</param>
        /// <param name="right">The second <see cref="Number"/> value to compare.</param>
        /// <returns>The smaller of the two <see cref="Number"/> values.</returns>
        public static Number Min(Number left, Number right)
        {
            return left.CompareTo(right) == -1 ? left : right;
        }
        #endregion

        /// <inheritdoc/>
        public override string ToString() => value.ToString();

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.value == obj || this.value.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }

}
