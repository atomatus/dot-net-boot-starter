using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    public partial class ClientTest
    {
        protected void Configure(EntityTypeBuilder<ClientTest> builder)
        {
            builder.ToTable("Clients");

            builder.Property(e => e.Age)
                .HasMaxLength(MAX_AGE_LENGTH)
                .IsRequired();

            builder.Property(e => e.Name)
                .HasMaxLength(MAX_NAME_LENGTH)
                .IsRequired();

            builder.HasData(this.SeedData());
        }

        private IEnumerable<ClientTest> SeedData()
        {
            yield return new ClientTest { Id = 1, Uuid = Guid.NewGuid(), Age = 22, Name = "Dr. Sheldon Lee Cooper", Created = DateTime.Now };
            yield return new ClientTest { Id = 2, Uuid = Guid.NewGuid(), Age = 24, Name = "Dra. Amy Farrah Fowler", Created = DateTime.Now };
                                                  
            yield return new ClientTest { Id = 3, Uuid = Guid.NewGuid(), Age = 24, Name = "Dr. Leonard Leakey Hofstadter", Created = DateTime.Now };
            yield return new ClientTest { Id = 4, Uuid = Guid.NewGuid(), Age = 22, Name = "Penny Hofstadter", Created = DateTime.Now };
                                                  
            yield return new ClientTest { Id = 5, Uuid = Guid.NewGuid(), Age = 24, Name = "Howard Joel Wolowitz", Created = DateTime.Now };
            yield return new ClientTest { Id = 6, Uuid = Guid.NewGuid(), Age = 24, Name = "Dra. Bernadette Rostenkowski-Wolowitz ", Created = DateTime.Now };
                                                  
            yield return new ClientTest { Id = 7, Uuid = Guid.NewGuid(), Age = 25, Name = "Dr. Rajesh Ramayan Koothrappali", Created = DateTime.Now };
        }

    }
}
