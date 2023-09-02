namespace Com.Atomatus.Bootstarter.Test
{
    public class UnitTestObjectMapperForDTO
    {
        [Fact]
        public void Utils_ObjectMapper_Parse_For_DTO_Successfully()
        {
            User user = new()
            {
                Uuid = Guid.NewGuid(),
                Username = "test",
                Password = "123456",
            };

            var dto = ObjectMapper.Parse<UserDTO>(user);

            Assert.NotNull(user);
            Assert.NotNull(dto);
            Assert.Equal(user.Uuid, dto.Uuid);
            Assert.Equal(user.Username, dto.Username);
        }

        [Fact]
        public void Utils_ObjectMapper_Copy_Circular_Ref_Ignored_Successfully()
        {
            Order order = new()
            {
                Uuid = Guid.NewGuid()
            };
            order.Items = new List<Item>(new Item[]
            {
                new () { Name = "Item 1 ", Order = order },
                new () { Name = "Item 2 ", Order = order },
                new () { Name = "Item 3 ", Order = order }
            });

            Order copy = ObjectMapper.Copy(order);

            Assert.NotNull(order);
            Assert.NotNull(copy);
            Assert.NotNull(copy.Items);
            Assert.Equal(order.Uuid, copy.Uuid);
            Assert.Equal(order.Items.Count, copy.Items.Count);
            Assert.True(order.Items.SequenceEqual(copy.Items));
        }

        [Fact]
        public void Utils_ObjectMapper_Parse_Circular_Ref_Ignored_Successfully()
        {
            OrderDTO dto = new()
            {
                Uuid = Guid.NewGuid()
            };
            dto.Items = new List<ItemDTO>(new ItemDTO[]
            {
                new () { Name = "Item 1 ", Order = dto },
                new () { Name = "Item 2 ", Order = dto },
                new () { Name = "Item 3 ", Order = dto }
            });

            Order order = ObjectMapper.Parse<Order>(dto);

            Assert.NotNull(dto);
            Assert.NotNull(order);
            Assert.NotNull(order.Items);
            Assert.Equal(dto.Uuid, order.Uuid);
            Assert.Equal(dto.Items.Count, order.Items.Count);

            for(int i=0, l = dto.Items.Count; i < l; i++)
            {
                Assert.Equal(dto.Items[i].Name, order.Items[i].Name);
            }
        }

        [Theory]
        [InlineData(ABC.A)]
        [InlineData(ABC.B)]
        [InlineData(ABC.C)]
        [InlineData(ABC.A | ABC.B)]
        [InlineData(ABC.A | ABC.C)]
        [InlineData(ABC.B | ABC.C)]
        [InlineData(ABC.A | ABC.B | ABC.C)]
        public void Utils_ObjectMapper_Parse_Item_Enum_Successfully(ABC abc)
        {
            AbcDTO dto = new() { Abc = abc };
            
            AbcByteDTO byteDTO = ObjectMapper.Parse<AbcByteDTO>(dto);
            Assert.NotNull(dto);
            Assert.NotNull(byteDTO);
            Assert.Equal((byte) dto.Abc, byteDTO.Abc);

            AbcIntDTO intDTO = ObjectMapper.Parse<AbcIntDTO>(dto);
            Assert.NotNull(dto);
            Assert.NotNull(intDTO);
            Assert.Equal((int)dto.Abc, intDTO.Abc);

            AbcDTO fromByteDTO = ObjectMapper.Parse<AbcDTO>(byteDTO);
            Assert.NotNull(byteDTO);
            Assert.NotNull(fromByteDTO);
            Assert.Equal(fromByteDTO.Abc, (ABC) byteDTO.Abc);

            AbcDTO fromIntDTO = ObjectMapper.Parse<AbcDTO>(intDTO);
            Assert.NotNull(intDTO);
            Assert.NotNull(fromIntDTO);
            Assert.Equal(fromIntDTO.Abc, (ABC)intDTO.Abc);
        }

        interface IModel
        {
            Guid Uuid { get; set; }
        }

        interface IUser : IModel
        {
            string? Username { get; set; }
        }

        interface IUserAuth: IUser
        {
            public string? Password { get; set; }
        }

        class AuditableModel : IModel
        {
            public Guid Uuid { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        class User : AuditableModel, IUserAuth
        {
            public string? Username { get; set; }

            public string? Password { get; set; }
        }

        class BaseDTO
        {
            public Guid Uuid { get; set; }
        }

        class UserDTO : BaseDTO, IUser
        {
            public string? Username { get; set; }
        }

        interface IItem : IModel
        {
            string? Name { get; set; }
        }

        interface IOrder : IModel
        {
            public List<IItem>? Items { get; set; }
        }

        class Item : AuditableModel, IItem
        {
            public string? Name { get; set; }

            [Ignore]
            public Order? Order { get; set; }

            public override bool Equals(object? obj)
            {
                return obj is Item i && i.Name == this.Name;
            }

            public override int GetHashCode()
            {
                return Name?.GetHashCode() ?? 0;
            }
        }

        class Order : AuditableModel, IOrder
        {
            public List<Item>? Items { get; set; }

            List<IItem>? IOrder.Items
            {
                get => Items?.OfType<IItem>().ToList();
                set => Items = ObjectMapper.ParseList<Item>(value);
            }
        }

        class ItemDTO : BaseDTO, IItem
        {
            public string? Name { get; set; }

            [Ignore]
            public OrderDTO? Order { get; set; }

        }

        class OrderDTO : BaseDTO, IOrder
        {
            public List<ItemDTO>? Items { get; set; }

            List<IItem>? IOrder.Items
            {
                get => Items?.OfType<IItem>().ToList();
                set => Items = ObjectMapper.ParseList<ItemDTO>(value);
            }
        }

        [Flags]
        public enum ABC
        {
            A = 1,
            B = 2,
            C = 4
        }

        class AbcDTO
        {
            public ABC Abc { get; set; }
        }

        class AbcByteDTO
        {
            public byte Abc { get; set; }
        }

        class AbcIntDTO
        {
            public int Abc { get; set; }
        }
    }
}
