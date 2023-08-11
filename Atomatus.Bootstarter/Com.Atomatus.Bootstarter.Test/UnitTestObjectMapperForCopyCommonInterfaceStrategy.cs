using Com.Atomatus.Bootstarter.Util;

namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForCopyCommonInterfaceStrategy
    {
        [Fact]
        public void Utils_ObjectMapper_Copy_Common_Interface_Strategy_Successfully()
        {
            A a = new() { IsValid = true, Name = "Test" };
            B b = new();
            Assert.True(ObjectMapper.Copy(a, b));
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        interface IA
        {
            bool IsValid { get; set; }
            string? Name { get; set; }
        }

        class A : IA
        {
            public bool IsValid { get; set; }
            public string? Name { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }

        class B : IA
        {
            public bool IsValid { get; set; }
            public string? Name { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }
    }
}
