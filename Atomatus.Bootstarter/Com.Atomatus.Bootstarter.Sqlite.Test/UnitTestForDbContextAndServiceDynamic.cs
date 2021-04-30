using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    [TestCaseOrderer("Com.Atomatus.Bootstarter.TestPriorityOrderer", "Com.Atomatus.Bootstarter.Sqlite.Test")]
    [Collection("DynamicContextProvider")]
    public class UnitTestForDbContextAndServiceDynamic : IClassFixture<DynamicContextProviderFixture>
    {
        private readonly DynamicContextProviderFixture provider;

        public UnitTestForDbContextAndServiceDynamic(DynamicContextProviderFixture provider)
        {
            this.provider = provider;
        }

        #region Requires
        [Fact, TestPriorityXHigh]
        [Trait("Operation", "Require")]
        public void RequireFromProvider_ServiceCrud_NotNull()
        {
            Assert.NotNull(provider.ClientService);
        }
        
        [Fact, TestPriorityXHigh]
        [Trait("Operation", "Require")]
        public void RequireFromProvider_ServiceCrud_WithId_NotNull()
        {
            Assert.NotNull(provider.ClientServiceWithId);
        }
        #endregion

        #region Save
        [Theory, TestPriorityHigh]
        [InlineData(ClientTest.MIN_AGE, "Lucy")]
        [InlineData(ClientTest.MIN_AGE + 1, "Wil Wheaton")]
        [InlineData(ClientTest.MAX_AGE - 1, "Priya Koothrappali")]
        [InlineData(ClientTest.MAX_AGE, "Mary Cooper")]
        [Trait("Operation", "Save")]
        [Trait("Result", "Successfully")]
        public void Service_Save_Successfully(int age, string name)
        {
            //[A]rrange
            ClientTest client   = new ClientTest
            {
                Age = age,
                Name = name
            };

            //[A]ct
            var beforeSave      = DateTime.Now;
            ClientTest result   = provider.ClientService.Save(client);

            //[A]ssert            
            Assert.Equal(client, result);
            Assert.True(result.Id > 0L, "After save, result does not contains an ID!");
            Assert.True(result.Uuid != Guid.Empty, "After save, result does not contains an Uuid!");
            Assert.True(result.Created > beforeSave, "After save, result does not contains a valid auditable Created date time!");
        }

        [Theory, TestPriorityHigh]
        [InlineData(ClientTest.MIN_AGE, "Sra. Wolowitz")]
        [Trait("Operation", "Save")]
        [Trait("Result", "Error")]
        [Trait("Result", "Constraint")]
        public void Service_Save_Error_UniqueConstraintPrimaryKey(int age, string name)
        {
            //[A]rrange
            ClientTest client = new ClientTest
            {
                Age = age,
                Name = name
            };

            //[A]ct
            ClientTest result = provider.ClientService.Save(client);
            var ex = Assert.ThrowsAny<DbUpdateException>(() => provider.ClientService.Save(result));

            //[A]ssert
            Assert.NotNull(ex);
        }

        [Theory, TestPriorityHigh]
        [InlineData(ClientTest.MIN_AGE, null)]
        [Trait("Operation", "Save")]
        [Trait("Result", "Error")]
        [Trait("Result", "FieldRequired")]
        public void Service_Save_Error_FieldRequired(int age, string name)
        {
            //[A]rrange
            ClientTest client = new ClientTest
            {
                Age = age,
                Name = name
            };

            //[A]ct
            var ex = Assert.ThrowsAny<DbUpdateException>(() => provider.ClientService.Save(client));

            //[A]ssert
            Assert.NotNull(ex);
        }
        #endregion

        #region Update        
        [Fact, TestPriorityLow]
        [Trait("Operation", "Update")]
        [Trait("Result", "Successfully")]
        public void Service_Update_Without_Id_Successfully()
        {
            //[A]rrange
            ClientTest client = new ClientTest { Age = ClientTest.MAX_AGE, Name = "Arthur Jeffries" };
            //[A]ct
            ClientTest result = provider.ClientService.Save(client);
            //[A]ssert            
            Assert.Equal(client, result);

            //[A]rrange
            client.Age--;
            client.Id = 0;

            var beforeSave = DateTime.Now;
            result = provider.ClientService.Update(client);

            //[A]ssert            
            Assert.Equal(client, result);
            Assert.True(result.Id > 0L, "After update, result does not contains an ID!");
            Assert.True(result.Uuid != Guid.Empty, "After update, result does not contains an Uuid!");
            Assert.True(result.Updated > beforeSave, "After update, result does not contains a valid auditable Updated date time!");
        }

        [Fact, TestPriorityLow]
        [Trait("Operation", "Update")]
        [Trait("Result", "Successfully")]
        public void Service_Update_Without_Uuid_Successfully()
        {
            //[A]rrange
            ClientTest client = new ClientTest { Age = ClientTest.MAX_AGE, Name = "Dr. Crawley" };
            //[A]ct
            ClientTest result = provider.ClientService.Save(client);
            //[A]ssert            
            Assert.Equal(client, result);

            //[A]rrange            
            client.Age--;
            client.Uuid = Guid.Empty;

            //[A]ct            
            var beforeSave = DateTime.Now;
            result = provider.ClientService.Update(client);

            //[A]ssert            
            Assert.Equal(client, result);
            Assert.True(result.Id > 0L, "After update, result does not contains an ID!");
            Assert.True(result.Uuid != Guid.Empty, "After update, result does not contains an Uuid!");
            Assert.True(result.Updated > beforeSave, "After update, result does not contains a valid auditable Updated date time!");
        }

        [Theory, TestPriorityLow]
        [InlineData(0, 1, ClientTest.MAX_AGE - 1, "Sra. Wolowitz")]
        [InlineData(1, 2, ClientTest.MAX_AGE - 2, "Zack")]
        [Trait("Operation", "Update")]
        [Trait("Result", "Successfully")]
        public void Service_Update_Successfully(int page, int limit, int age, string name)
        {
            //[A]rrange
            ClientTest client = provider.ClientService.Paging(page, limit).Last();
            client.Age = age;
            client.Name = name;

            //[A]ct            
            var beforeSave = DateTime.Now;
            ClientTest result = provider.ClientService.Update(client);

            //[A]ssert            
            Assert.Equal(client, result);
            Assert.True(result.Id > 0L, "After update, result does not contains an ID!");
            Assert.True(result.Uuid != Guid.Empty, "After update, result does not contains an Uuid!");
            Assert.True(result.Updated > beforeSave, "After update, result does not contains a valid auditable Updated date time!");
        }

        [Fact, TestPriorityLow]
        [Trait("Operation", "Update")]
        [Trait("Result", "Error")]
        [Trait("Result", "FieldRequired")]
        public void Service_Update_Error_FieldRequired()
        {
            //[A]rrange
            ClientTest client = provider.ClientService.First();
            client.Name = null;//required it.

            //[A]ct
            var ex = Assert.ThrowsAny<DbUpdateException>(() => provider.ClientService.Update(client));

            //[A]ssert
            Assert.NotNull(ex);
        }

        [Fact, TestPriorityLow]
        [Trait("Operation", "Update")]
        [Trait("Result", "Error")]
        [Trait("Result", "Untrackable")]
        public void Service_Update_Error_Untrackable()
        {
            //[A]rrange
            ClientTest client = new ClientTest
            {
                Age = ClientTest.MIN_AGE,
                Name = new string('a', ClientTest.MAX_NAME_LENGTH)
            };

            //[A]ct
            var ex = Assert.ThrowsAny<InvalidOperationException>(() => provider.ClientService.Update(client));

            //[A]ssert
            Assert.NotNull(ex);
        }
        #endregion

        #region Delete
        [Fact, TestPriorityXXLow]
        [Trait("Operation", "Delete")]
        [Trait("Result", "Successfully")]
        public void Service_Delete_Successfully()
        {
            //[A]rrange
            ClientTest client = provider.ClientService.First();

            //[A]ct            
            bool result = provider.ClientService.Delete(client);

            //[A]ssert            
            Assert.True(result, "Does not possible to delete value!");
        }

        [Fact, TestPriorityXXLow]
        [Trait("Operation", "Delete")]
        [Trait("Result", "Successfully")]
        public void Service_Delete_By_Uuid_Successfully()
        {
            //[A]rrange
            ClientTest client = provider.ClientService.First();

            //[A]ct            
            bool result = provider.ClientService.DeleteByUuid(client.Uuid);

            //[A]ssert            
            Assert.True(result, "Does not possible to delete value!");
        }

        [Fact, TestPriorityXXLow]
        [Trait("Operation", "Delete")]
        [Trait("Result", "Error")]
        [Trait("Result", "Untrackable")]
        public void Service_Delete_Error_Untrackable()
        {
            //[A]rrange
            ClientTest client = new ClientTest
            {
                Age = ClientTest.MIN_AGE,
                Name = new string('a', ClientTest.MAX_NAME_LENGTH)
            };

            //[A]ct
            var ex = Assert.ThrowsAny<InvalidOperationException>(() => provider.ClientService.Delete(client));

            //[A]ssert
            Assert.NotNull(ex);
        }
        #endregion
    }
}
