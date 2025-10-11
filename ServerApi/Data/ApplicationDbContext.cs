using Microsoft.EntityFrameworkCore;
using Server.Api.Data.Models;

namespace Server.Api.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Таблица в БД для сущности Record
        public DbSet<Record> Records => Set<Record>();
    }
}
