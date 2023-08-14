using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter.Test
{
    public sealed class UnitTestObjectHashCode
    {
        [Fact]
        public void Utils_Object_HashCode_Equals_Successfully()
        {
            TestItem testItem0 = new() { Code = "A", Amount = 1.5m, Name = "test" };
            TestItem testItem1 = new() { Code = "A", Amount = 1.5m, Name = "test2" };

            Assert.Equal(testItem0.GetHashCode(), testItem1.GetHashCode());
        }

        [Fact]
        public void Utils_Object_HashCode_NotEquals_Successfully()
        {
            TestItem testItem0 = new() { Code = "A", Amount = 1.5m, Name = "test" };
            TestItem testItem1 = new() { Code = "A", Amount = 1.2m, Name = "test2" };

            Assert.NotEqual(testItem0.GetHashCode(), testItem1.GetHashCode());
        }

        [Fact]
        public void Utils_Object_HashCode_Ignore_Virtual_Properties_Successfully()
        {
            TestItem testItem0 = new() { Code = "A", Amount = 1.5m, Name = "test" };
            TestItem2 testItem1 = new() { Code = "A", Amount = 1.5m };

            Assert.Equal(testItem0.GetHashCode(), testItem1.GetHashCode());
        }

        class TestItem : ModelBase<long>
        {
            public string? Code { get; set; }

            public decimal Amount { get; set; }

            public virtual string? Name { get; set; }
        }

        class TestItem2 : ModelBase<long>
        {
            public string? Code { get; set; }

            public decimal Amount { get; set; }
        }
    }
}
