namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForSolveNullableType
    {
        [Fact]
        public void Utils_ObjectMapper_SolveNullableType_Successfully()
        {
             foreach (var p in typeof(A).GetProperties())
            {
                var type = p.PropertyType;
                var nType = p.PropertyType;
                ObjectMapper.SolveNullableType(ref type);
                Assert.NotEqual(type, nType);
            }
        }

        class A
        {
            public long? L { get; set; }

            public int? I { get; set; }

            public Abc? E { get; set; }
        }

        enum Abc
        {
            A, B, C
        }
    }
}
