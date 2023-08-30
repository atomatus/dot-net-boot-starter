namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForDefaultValue
    {
        [Fact]
        public void Utils_ObjectMapper_GetDefaultValue_Successfully()
        {
            Assert.Null(ObjectMapper.GetDefaultValue(typeof(string)));
            Assert.Null(ObjectMapper.GetDefaultValue(typeof(List<int>)));
            Assert.Null(ObjectMapper.GetDefaultValue(typeof(IList<int>)));
            Assert.Null(ObjectMapper.GetDefaultValue(typeof(IList<>)));

            Assert.NotNull(ObjectMapper.GetDefaultValue(typeof(bool)));
            Assert.NotNull(ObjectMapper.GetDefaultValue(typeof(int)));

            Assert.Equal(ObjectMapper.GetDefaultValue(typeof(bool)), false);
            Assert.Equal(ObjectMapper.GetDefaultValue(typeof(int)), 0);
            Assert.Equal(ObjectMapper.GetDefaultValue(typeof(float)), 0f);
            Assert.Equal(ObjectMapper.GetDefaultValue(typeof(decimal)), 0m);
        }
    }
}
