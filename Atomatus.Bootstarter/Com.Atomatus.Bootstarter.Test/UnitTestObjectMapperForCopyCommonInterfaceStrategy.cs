using Com.Atomatus.Bootstarter.Util;

namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForCopyCommonInterfaceStrategy
    {
        [Fact]
        public void Utils_ObjectMapper_Copy_Common_Interface_Strategy_Successfully()
        {
            A a = new() { IsValid = true, Name = "Test", List = new List<string> { "a", "b", "c" }, Points = new List<Point> { new Point { X = 1, Y = 2 }, new Point { X = 2, Y = 3 } } };
            B b = new();
            Assert.True(ObjectMapper.Copy(a, b));
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        class Point
        {
            public float X { get; set; }
            public float Y { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }

        interface IA
        {
            bool IsValid { get; set; }
            string? Name { get; set; }

            List<string>? List { get; set; }

            List<Point>? Points { get; set; }
        }

        class A : IA
        {
            public bool IsValid { get; set; }
            public string? Name { get; set; }
            public List<string>? List { get; set; }
            public List<Point>? Points { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }

        class B : IA
        {
            public bool IsValid { get; set; }
            public string? Name { get; set; }
            public List<string>? List { get; set; }
            public List<Point>? Points { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }
    }
}
