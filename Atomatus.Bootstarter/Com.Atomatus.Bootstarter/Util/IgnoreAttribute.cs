using System;

namespace Com.Atomatus.Bootstarter.Util
{
    /// <summary>
    /// Use this attribute to ignore field for <see cref="ObjectMapper"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute { }
}
