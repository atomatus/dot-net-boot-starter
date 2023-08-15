using Com.Atomatus.Bootstarter.Util;

namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForParseListInterface
    {
        [Fact]
        public void Utils_ObjectMapper_Parse_List_Interface_Successfully()
        {
            List<Test> list = new List<Test> 
            {
                new Test { Id = 1, Name = "a"  },
                new Test { Id = 1, Name = "b"  },
                new Test { Id = 1, Name = "c"  }
            };

            IList<Test> newList = (IList<Test>) ObjectMapper.Parse(list, typeof(IList<Test>));

            Assert.NotNull(newList);
            Assert.NotEmpty(newList);
            Assert.Equal(list, newList);
        }

        [Fact]
        public void Utils_ObjectMapper_Parse_List_Interface_Other_Item_Successfully()
        {
            List<Test> list = new List<Test>
            {
                new Test { Id = 1, Name = "a"  },
                new Test { Id = 1, Name = "b"  },
                new Test { Id = 1, Name = "c"  }
            };

            IList<Test2> newList = (IList<Test2>)ObjectMapper.Parse(list, typeof(IList<Test2>));

            Assert.NotNull(list);
            Assert.NotNull(newList);
            Assert.NotEmpty(newList);
            Assert.Equal(list.Count, newList.Count);

            for (int i = 0, l = list.Count; i < l; i++)
            {
                Assert.Equal(list[i].Id, newList[i].Id);
                Assert.Equal(list[i].Name, newList[i].Name);
            }
        }

        [Fact]
        public void Utils_ObjectMapper_ParseList_Source_Successfull()
        {
            List<Test> list = new List<Test>
            {
                new Test { Id = 1, Name = "a"  },
                new Test { Id = 1, Name = "b"  },
                new Test { Id = 1, Name = "c"  }
            };

            IList<Test> newList = ObjectMapper.ParseList<Test>(list);

            Assert.NotNull(newList);
            Assert.NotEmpty(newList);
            Assert.Equal(list, newList);
        }

        [Fact]
        public void Utils_ObjectMapper_ParseList_Successfull()
        {
            List<Test> list = new List<Test>
            {
                new Test { Id = 1, Name = "a"  },
                new Test { Id = 1, Name = "b"  },
                new Test { Id = 1, Name = "c"  }
            };

            IList<Test2> newList = ObjectMapper.ParseList<Test, Test2>(list);

            Assert.NotNull(list);
            Assert.NotNull(newList);
            Assert.NotEmpty(newList);
            Assert.Equal(list.Count, newList.Count);

            for (int i = 0, l = list.Count; i < l; i++)
            {
                Assert.Equal(list[i].Id, newList[i].Id);
                Assert.Equal(list[i].Name, newList[i].Name);
            }
        }

        class Test
        {
            public int Id { get; set; }

            public string? Name { get; set; }
        }

        class Test2
        {
            public int Id { get; set; }

            public string? Name { get; set; }
        }
    }
}
