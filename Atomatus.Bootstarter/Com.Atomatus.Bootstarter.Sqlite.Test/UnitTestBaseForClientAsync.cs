using Com.Atomatus.Bootstarter.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [TestCaseOrderer("Com.Atomatus.Bootstarter.TestPriorityOrderer", "Com.Atomatus.Bootstarter.Sqlite.Test")]    
    public abstract class UnitTestBaseForClientAsync<TProviderFixture> : IClassFixture<TProviderFixture>
        where TProviderFixture : ProviderFixture<ClientTest, long>
    {
        private readonly TProviderFixture provider;

        public UnitTestBaseForClientAsync(TProviderFixture provider)
        {
            this.provider = provider;
        }

        #region Requires
        [Fact, TestPriorityXHigh]
        [Trait("Operation", "Require")]
        [Trait("Operation", "Async")]
        public void RequireFromProvider_ServiceCrud_NotNull_Async()
        {
            Assert.NotNull(provider.ServiceAsync);
        }
        
        [Fact, TestPriorityXHigh]
        [Trait("Operation", "Require")]
        [Trait("Operation", "Async")]
        public void RequireFromProvider_ServiceCrud_WithId_NotNull_Async()
        {
            Assert.NotNull(provider.ServiceWithIdAsync);
        }
        #endregion

        #region Save
        [Theory, TestPriorityHigh]
        [InlineData(ClientTest.MIN_AGE, "Stuart Bloom")]
        [InlineData(ClientTest.MIN_AGE + 1, "Alex Jensen")]
        [InlineData(ClientTest.MAX_AGE - 1, "Barry Kripke")]
        [InlineData(ClientTest.MAX_AGE, "Dr. Beverly Hofstadter")]
        [Trait("Operation", "Save")]
        [Trait("Operation", "Async")]
        [Trait("Result", "Successfully")]
        public async Task Service_Save_Successfully_Async(int age, string name)
        {
            //[A]rrange
            ClientTest client   = new ClientTest
            {
                Age = age,
                Name = name
            };

            //[A]ct
            var beforeSave      = DateTime.Now;
            ClientTest result   = await provider.ServiceAsync.SaveAsync(client);

            //[A]ssert            
            Assert.Equal(client, result);
            Assert.True(result.Id > 0L, "After save, result does not contains an ID!");
            Assert.True(result.Uuid != Guid.Empty, "After save, result does not contains an Uuid!");
            Assert.True(result.Created > beforeSave, "After save, result does not contains a valid auditable Created date time!");
        }

        [Theory, TestPriorityHigh]
        [InlineData(ClientTest.MIN_AGE, "Dr. Eric Gablehauser")]
        [Trait("Operation", "Save")]
        [Trait("Operation", "Async")]
        [Trait("Result", "Error")]
        [Trait("Result", "Constraint")]
        public async Task Service_Save_Error_UniqueConstraintPrimaryKey_Async(int age, string name)
        {
            //[A]rrange
            ClientTest client = new ClientTest
            {
                Age = age,
                Name = name
            };

            //[A]ct
            ClientTest result = await provider.ServiceAsync.SaveAsync(client);
            var ex = await Assert.ThrowsAnyAsync<DbUpdateException>(() => provider.ServiceAsync.SaveAsync(result));

            //[A]ssert
            Assert.NotNull(ex);
        }

        [Theory, TestPriorityHigh]
        [InlineData(ClientTest.MIN_AGE, null)]
        [Trait("Operation", "Save")]
        [Trait("Operation", "Async")]
        [Trait("Result", "Error")]
        [Trait("Result", "FieldRequired")]
        public async Task Service_Save_Error_FieldRequired_Async(int age, string name)
        {
            //[A]rrange
            ClientTest client = new ClientTest
            {
                Age = age,
                Name = name
            };

            //[A]ct
            var ex = await Assert.ThrowsAnyAsync<AggregateValidationException>(() => provider.ServiceAsync.SaveAsync(client));

            //[A]ssert
            Assert.NotNull(ex);
        }
        #endregion

        #region Update
        [Fact, TestPriorityLow]
        [Trait("Operation", "Update")]
        [Trait("Result", "Successfully")]
        public async Task Service_Update_Without_Id_Successfully_Async()
        {
            //[A]rrange
            ClientTest client = new ClientTest { Age = ClientTest.MAX_AGE, Name = "Arthur Jeffries" };
            //[A]ct
            ClientTest result = await provider.ServiceAsync.SaveAsync(client);
            //[A]ssert            
            Assert.Equal(client, result);

            //[A]rrange
            client.Age--;
            client.Id = 0;

            var beforeSave = DateTime.Now;
            result = await provider.ServiceAsync.UpdateAsync(client);

            //[A]ssert            
            Assert.Equal(client, result);
            Assert.True(result.Id > 0L, "After update, result does not contains an ID!");
            Assert.True(result.Uuid != Guid.Empty, "After update, result does not contains an Uuid!");
            Assert.True(result.Updated > beforeSave, "After update, result does not contains a valid auditable Updated date time!");
        }

        [Fact, TestPriorityLow]
        [Trait("Operation", "Update")]
        [Trait("Result", "Successfully")]
        public async Task Service_Update_Without_Uuid_Successfully_Async()
        {
            //[A]rrange
            ClientTest client = new ClientTest { Age = ClientTest.MAX_AGE, Name = "Dr. Crawley" };
            //[A]ct
            ClientTest result = await provider.ServiceAsync.SaveAsync(client);
            //[A]ssert            
            Assert.Equal(client, result);

            //[A]rrange            
            client.Age--;
            client.Uuid = Guid.Empty;

            //[A]ct            
            var beforeSave = DateTime.Now;
            result = await provider.ServiceAsync.UpdateAsync(client);

            //[A]ssert            
            Assert.Equal(client, result);
            Assert.True(result.Id > 0L, "After update, result does not contains an ID!");
            Assert.True(result.Uuid != Guid.Empty, "After update, result does not contains an Uuid!");
            Assert.True(result.Updated > beforeSave, "After update, result does not contains a valid auditable Updated date time!");
        }

        [Fact, TestPriorityLow]
        [Trait("Operation", "Update")]
        [Trait("Result", "Successfully")]
        public async Task Service_Update_ByRecoveryId_Successfully_Async()
        {
            //[A]rrange
            ClientTest client = new ClientTest { Age = ClientTest.MAX_AGE, Name = "Dr. Crawley" };
            //[A]ct
            ClientTest result = await provider.ServiceAsync.SaveAsync(client);
            //[A]ssert            
            Assert.Equal(client, result);

            //[A]ct
            client = await provider.ServiceWithIdAsync.GetAsync(result.Id);
            //[A]ssert            
            Assert.Equal(client.Id, result.Id);
            Assert.Equal(client.Uuid, result.Uuid);
            Assert.Equal(client.Age, result.Age);
            Assert.Equal(client.Name, result.Name);

            //[A]rrange            
            client.Age--;
            client.Uuid = Guid.Empty;

            //[A]ct            
            var beforeSave = DateTime.Now;
            result = await provider.ServiceAsync.UpdateAsync(client);

            //[A]ssert            
            Assert.Equal(client, result);
            Assert.True(result.Id > 0L, "After update, result does not contains an ID!");
            Assert.True(result.Uuid != Guid.Empty, "After update, result does not contains an Uuid!");
            Assert.True(result.Updated > beforeSave, "After update, result does not contains a valid auditable Updated date time!");
        }

        [Theory, TestPriorityXLow]
        [InlineData(0, 1, ClientTest.MAX_AGE - 1, "Dr. & Sra. Koothrappali")]
        [InlineData(1, 2, ClientTest.MAX_AGE - 2, "Leslie Winkle")]
        [Trait("Operation", "Update")]
        [Trait("Operation", "Async")]
        [Trait("Result", "Successfully")]
        public async Task Service_Update_Successfully_Async(int page, int limit, int age, string name)
        {
            //[A]rrange
            ClientTest client = (await provider.ServiceAsync.PagingAsync(page, limit)).Last();
            client.Age = age;
            client.Name = name;

            //[A]ct            
            var beforeSave = DateTime.Now;
            ClientTest result = await provider.ServiceAsync.UpdateAsync(client);

            //[A]ssert            
            Assert.Equal(client, result);
            Assert.True(result.Id > 0L, "After update, result does not contains an ID!");
            Assert.True(result.Uuid != Guid.Empty, "After update, result does not contains an Uuid!");
            Assert.True(result.Updated > beforeSave, "After update, result does not contains a valid auditable Updated date time!");
        }

        [Fact, TestPriorityXLow]
        [Trait("Operation", "Update")]
        [Trait("Operation", "Async")]
        [Trait("Result", "Error")]
        [Trait("Result", "FieldRequired")]
        public async Task Service_Update_Error_FieldRequired_Async()
        {
            //[A]rrange
            ClientTest client = await provider.ServiceAsync.FirstAsync();
            client.Name = null;//required it.

            //[A]ct
            var ex = await Assert.ThrowsAnyAsync<AggregateValidationException>(() => provider.ServiceAsync.UpdateAsync(client));

            //[A]ssert
            Assert.NotNull(ex);
        }

        [Fact, TestPriorityXLow]
        [Trait("Operation", "Update")]
        [Trait("Operation", "Async")]
        [Trait("Result", "Error")]
        [Trait("Result", "Untrackable")]
        public async Task Service_Update_Error_Untrackable_Async()
        {
            //[A]rrange
            ClientTest client = new ClientTest
            {
                Age = ClientTest.MIN_AGE,
                Name = new string('a', ClientTest.MAX_NAME_LENGTH)
            };

            //[A]ct
            var ex = await Assert.ThrowsAnyAsync<InvalidOperationException>(() => provider.ServiceAsync.UpdateAsync(client));

            //[A]ssert
            Assert.NotNull(ex);
        }
        #endregion

        #region Delete
        [Fact, TestPriorityXXLow]
        [Trait("Operation", "Delete")]
        [Trait("Operation", "Async")]
        [Trait("Result", "Successfully")]
        public async Task Service_Delete_Successfully_Async()
        {
            //[A]rrange
            ClientTest client = await provider.ServiceAsync.FirstAsync();

            //[A]ct            
            bool result = await provider.ServiceAsync.DeleteAsync(client);

            //[A]ssert            
            Assert.True(result, "Does not possible to delete value!");
        }

        [Fact, TestPriorityXXLow]
        [Trait("Operation", "Delete")]
        [Trait("Operation", "Async")]
        [Trait("Result", "Successfully")]
        public async Task Service_Delete_By_Uuid_Successfully_Async()
        {
            //[A]rrange
            ClientTest client = await provider.ServiceAsync.FirstAsync();

            //[A]ct            
            bool result = await provider.ServiceAsync.DeleteByUuidAsync(client.Uuid);

            //[A]ssert            
            Assert.True(result, "Does not possible to delete value!");
        }

        [Fact, TestPriorityXXLow]
        [Trait("Operation", "Delete")]
        [Trait("Operation", "Async")]
        [Trait("Result", "Error")]
        [Trait("Result", "Untrackable")]
        public async Task Service_Delete_Error_Untrackable_Async()
        {
            //[A]rrange
            ClientTest client = new ClientTest
            {
                Age = ClientTest.MIN_AGE,
                Name = new string('a', ClientTest.MAX_NAME_LENGTH)
            };

            //[A]ct
            var ex = await Assert.ThrowsAnyAsync<InvalidOperationException>(() => provider.ServiceAsync.UpdateAsync (client));

            //[A]ssert
            Assert.NotNull(ex);
        }
        #endregion
    }
}
