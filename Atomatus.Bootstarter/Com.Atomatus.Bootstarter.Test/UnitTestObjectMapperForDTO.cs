using Com.Atomatus.Bootstarter.Util;

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
    }
}
