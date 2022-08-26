using MeterReadings.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadings.Data
{
    public class MeterReadingsDbContext : DbContext
    {
        public MeterReadingsDbContext(DbContextOptions<MeterReadingsDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<MeterReading> MeterReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasData((new []{
                    new Account { Id = 2344, FirstName = "Tommy", LastName = "Test" },
                    new Account { Id = 2233, FirstName = "Barry", LastName = "Test" },
                    new Account { Id = 8766, FirstName = "Sally", LastName = "Test" },
                    new Account { Id = 2345, FirstName = "Jerry", LastName = "Test" },
                    new Account { Id = 2346, FirstName = "Ollie", LastName = "Test" },
                    new Account { Id = 2347, FirstName = "Tara", LastName = "Test" },
                    new Account { Id = 2348, FirstName = "Tammy", LastName = "Test" },
                    new Account { Id = 2349, FirstName = "Simon", LastName = "Test" },
                    new Account { Id = 2350, FirstName = "Colin", LastName = "Test" },
                    new Account { Id = 2351, FirstName = "Gladys", LastName = "Test" },
                    new Account { Id = 2352, FirstName = "Greg", LastName = "Test" },
                    new Account { Id = 2353, FirstName = "Tony", LastName = "Test" },
                    new Account { Id = 2355, FirstName = "Arthur", LastName = "Test" },
                    new Account { Id = 2356, FirstName = "Craig", LastName = "Test" },
                    new Account { Id = 6776, FirstName = "Laura", LastName = "Test" },
                    new Account { Id = 4534, FirstName = "JOSH", LastName = "Test" },
                    new Account { Id = 1234, FirstName = "Freya", LastName = "Test" },
                    new Account { Id = 1239, FirstName = "Noddy", LastName = "Test" },
                    new Account { Id = 1240, FirstName = "Archie", LastName = "Test" },
                    new Account { Id = 1241, FirstName = "Lara", LastName = "Test" },
                    new Account { Id = 1242, FirstName = "Tim", LastName = "Test" },
                    new Account { Id = 1243, FirstName = "Graham", LastName = "Test" },
                    new Account { Id = 1244, FirstName = "Tony", LastName = "Test" },
                    new Account { Id = 1245, FirstName = "Neville", LastName = "Test" },
                    new Account { Id = 1246, FirstName = "Jo", LastName = "Test" },
                    new Account { Id = 1247, FirstName = "Jim", LastName = "Test" },
                    new Account { Id = 1248, FirstName = "Pam", LastName = "Test" }
                    }
                ));
        }
    }
}