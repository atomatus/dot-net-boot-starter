﻿using Com.Atomatus.Bootstarter.Util;

namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForCopyAssinableTypeStrategy
    {
        [Fact]
        public void Utils_ObjectMapper_Copy_Assinable_Type_Strategy_Successfully()
        {
            A a = new() { X = 1, Y = 2 };
            B b = new() { Z = 3};
            Assert.True(ObjectMapper.Copy(a, b));
            Assert.Equal(a.X, b.X);
            Assert.Equal(a.Y, b.Y);
            Assert.Equal(3, b.Z);

            a = new() { X = 3, Y = 4 };
            b = new() { Z = 5 };
            Assert.True(ObjectMapper.Copy(b, a));
            Assert.Equal(a.X, b.X);
            Assert.Equal(a.Y, b.Y);
            Assert.Equal(5, b.Z);
        }

        class A
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        class B : A
        {
            public int Z { get; set; }
        }
    }
}
