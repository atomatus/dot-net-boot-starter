namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForCopyEqualStrategy
    {
        [Fact]
        public void Utils_ObjectMapper_Copy_Equal_Strategy_Successfully()
        {
            var e0 = new EqualCopyTest
            {
                BoolValue = true,
                IntValue = 10,
                StrValue = "abc",
                DoubleValue = 0.123d,
                DecimalValue = 1.234m,
                IntList = new List<int> { 1, 2, 3 },
                StrList = new List<string> { "a", "b", "c" },
                Child = new List<IEqualCopyTest>
                {
                    new EqualCopyTest { BoolValue = false, IntValue = 11, StrValue = "def"},
                    new EqualCopyTest { BoolValue = true, IntValue = 12, StrValue = "ghi"}
                }
            };

            var e1 = new EqualCopyTest();
            Assert.True(ObjectMapper.Copy(e0, e1));
            Assert.Equal(e0.GetHashCode(), e1.GetHashCode());
        }

        interface IEqualCopyTest
        {
            bool BoolValue { get; set; }
            int IntValue { get; set; }
            string? StrValue { get; set; }
        }

        class EqualCopyTest : IEqualCopyTest
        {
            public bool BoolValue { get; set; }
            public int IntValue { get; set; }
            public string? StrValue { get; set; }
            public double DoubleValue { get; set; }
            public decimal DecimalValue { get; set; }
            public List<string>? StrList { get; set; }
            public List<int>? IntList { get; set; }
            public List<IEqualCopyTest>? Child { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }
    }
}