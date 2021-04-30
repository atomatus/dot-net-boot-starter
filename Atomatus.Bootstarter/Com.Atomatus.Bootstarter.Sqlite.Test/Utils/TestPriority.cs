using System;

namespace Com.Atomatus.Bootstarter
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public TestPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestPriorityLowAttribute : TestPriorityAttribute
    {
        public TestPriorityLowAttribute() : base(int.MaxValue - 2) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestPriorityXLowAttribute : TestPriorityAttribute
    {
        public TestPriorityXLowAttribute() : base(int.MaxValue - 1) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestPriorityXXLowAttribute : TestPriorityAttribute
    {
        public TestPriorityXXLowAttribute() : base(int.MaxValue) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestPriorityHighAttribute : TestPriorityAttribute
    {
        public TestPriorityHighAttribute() : base(int.MinValue + 2) { }
    }
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestPriorityXHighAttribute : TestPriorityAttribute
    {
        public TestPriorityXHighAttribute() : base(int.MinValue + 1) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestPriorityXXHighAttribute : TestPriorityAttribute
    {
        public TestPriorityXXHighAttribute() : base(int.MinValue) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestPriorityNormalAttribute : TestPriorityAttribute
    {
        public TestPriorityNormalAttribute() : base(0) { }
    }
}
