using Com.Atomatus.Bootstarter.Util;

namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForCopyCommonPropertyStrategy
    {
        [Fact]
        public void Utils_ObjectMapper_Copy_Common_Property_Strategy_Successfully()
        {
            A a = new() 
            { 
                Points = new List<APoint> 
                {
                    new () { X = 1, Y = 2 },
                    new () { X = 3, Y = 4 },
                    new () { X = 5, Y = 6 }
                },
                ToDict = new List<APoint>
                {
                    new () { X = 1, Y = 2 },
                    new () { X = 3, Y = 4 },
                },
                Dict = new Dictionary<string, APoint>
                {
                    { "A", new APoint() { X = 1, Y = 1 } },
                    { "B", new APoint() { X = 2, Y = 2 } }
                },
                IsValid = true,
                Value = 2d,
                Point = new () { X = 1, Y = 2}
            };
            B b = new();

            Assert.True(ObjectMapper.Copy(a, b));
            Assert.Equal(a.IsValid, b.IsValid);
            Assert.Equal(a.Value, b.Value);
            Assert.Equal(a.Point.GetHashCode(), b.Point?.GetHashCode());
            Assert.Equal(a.Points.Sum(p => (long) p.GetHashCode()), b.Points?.Sum(p => (long) p.GetHashCode()));
            Assert.Equal(a.Dict.Keys.Sum(v => (long) v.GetHashCode()), b.Dict?.Keys.Sum(v => (long) v.GetHashCode()));
            Assert.Equal(a.Dict.Values.Sum(v => (long) v.GetHashCode()), b.Dict?.Values.Sum(v => (long) v.GetHashCode()));
            Assert.Equal(a.ToDict.Sum(v => (long)v.GetHashCode()), b.ToDict?.Keys.Sum(v => (long)v.GetHashCode()));
            Assert.Equal(b.ToDict?.Values.Sum(v => v), 0);//default int
        }

        class APoint
        {
            public int X { get; set; }
            public int Y { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }

        class BPoint
        {
            public int X { get; set; }
            public int Y { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }

        class A
        {
            public List<APoint>? Points { get; set; }

            public List<APoint>? ToDict { get; set; }

            public Dictionary<string, APoint>? Dict { get; set; }

            public bool IsValid { get; set; }

            public double Value { get; set; }

            public BPoint? Point { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }

        class  B 
        {
            public List<BPoint>? Points { get; set; }

            public Dictionary<BPoint, int>? ToDict { get; set; }

            public Dictionary<string, BPoint>? Dict { get; set; }

            public bool IsValid { get; set; }

            public int Value { get; set; }

            public APoint? Point { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }
    }
}
