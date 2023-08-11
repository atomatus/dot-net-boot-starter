using Com.Atomatus.Bootstarter.Util;
using System.Collections;

namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForCopyCollectionStrategy
    {
        [Fact]
        public void Utils_ObjectMapper_Copy_Collection_Strategy_Successfully_Using_ArraList()
        {
            var l0 = new ArrayList {
                "a","b","c"
            };

            var l1 = new ArrayList();
            Assert.True(ObjectMapper.Copy(l0, l1));
            Assert.Contains(l0.ToArray(), l1.Contains);
        }

        [Fact]
        public void Utils_ObjectMapper_Copy_Collection_Strategy_Successfully_Using_ArraList_List()
        {
            var l0 = new ArrayList {
                "a","b","c"
            };

            var l1 = new List<string>();
            Assert.True(ObjectMapper.Copy(l0, l1));
            Assert.Contains(l0.ToArray(), l1.Contains);
        }

        [Fact]
        public void Utils_ObjectMapper_Copy_Collection_Strategy_Successfully_Using_List()
        {
            var l0 = new List<string> {
                "a","b","c"
            };

            var l1 = new List<string>();
            Assert.True(ObjectMapper.Copy(l0, l1));
            Assert.True(l0.All(l1.Contains));
        }

        [Fact]
        public void Utils_ObjectMapper_Copy_Collection_Strategy_Successfully_Using_Array_List()
        {
            var l0 = new string [] {
                "a","b","c"
            };

            var l1 = new List<string>();
            Assert.True(ObjectMapper.Copy(l0, l1));
            Assert.True(l0.All(l1.Contains));
        }

        [Fact]
        public void Utils_ObjectMapper_Copy_Collection_Strategy_Successfully_Using_List_Array()
        {
            var l0 = new List<string> {
                "a","b","c"
            };

            var l1 = new string[3];
            Assert.True(ObjectMapper.Copy(l0, l1));
            Assert.True(l0.All(l1.Contains));
        }

        [Fact]
        public void Utils_ObjectMapper_Copy_Collection_Strategy_Successfully_Using_List_Object()
        {
            var l0 = new List<CollectionObject> {
                new CollectionObject { IsValid = false, Name = "a" },
                new CollectionObject { IsValid = true, Name = "b" },
                new CollectionObject { IsValid = false, Name = "c" }
            };

            var l1 = new CollectionObject[3];
            Assert.True(ObjectMapper.Copy(l0, l1));
            Assert.Equal(l0.Sum(l => l.GetHashCode()), l1.Sum(l => l.GetHashCode()));
        }

        [Fact]
        public void Utils_ObjectMapper_Copy_Collection_Strategy_Successfully_Using_List_Object_Not_Same()
        {
            var l0 = new List<CollectionObject> {
                new CollectionObject { IsValid = false, Name = "a" },
                new CollectionObject { IsValid = true, Name = "b" },
                new CollectionObject { IsValid = false, Name = "c" }
            };

            var l1 = new CollectionObjectDTO[3];
            Assert.True(ObjectMapper.Copy(l0, l1));

            Assert.Equal(l0.Count, l1.Length);

            for(int i = 0; i < l0.Count; i++)
            {
                Assert.Equal(l0[i].IsValid, l1[i].IsValid);
            }
        }

        [Fact]
        public void Utils_ObjectMapper_Copy_Collection_Strategy_Successfully_Using_List_Object_Common_Interface()
        {
            var l0 = new List<CollectionObject> {
                new CollectionObject { IsValid = false, Name = "a" },
                new CollectionObject { IsValid = true, Name = "b" },
                new CollectionObject { IsValid = false, Name = "c" }
            };

            var l1 = new CollectionObjectCommonInterface[3];
            Assert.True(ObjectMapper.Copy(l0, l1));

            Assert.Equal(l0.Count, l1.Length);

            for (int i = 0; i < l0.Count; i++)
            {
                Assert.Equal(l0[i].IsValid, l1[i].IsValid);
            }
        }

        interface ICollectionObjectB
        {
            bool IsValid { get; set; }
        }

        class CollectionObjectDTO
        {
            public bool IsValid { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }

        class CollectionObjectCommonInterface : ICollectionObjectB
        {
            public bool IsValid { get; set; }

            public override int GetHashCode()
            {
                return Objects.GetHashCodeFromPublicFields(this);
            }
        }

        class CollectionObject : ICollectionObjectB
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
