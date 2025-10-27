using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    /// <summary>
    /// Контекст базы данных приложения.
    /// Отвечает за доступ к сущностям и управление подключением к БД.
    /// </summary>
    /// <remarks>
    /// Использует Entity Framework Core с SQLite.  
    /// </remarks>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Создаёт новый экземпляр контекста приложения.
        /// </summary>
        /// <param name="options">
        /// Параметры конфигурации контекста, включая строку подключения и параметры EF.
        /// </param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Таблица записей.  
        /// Представляет набор объектов в базе данных.
        /// </summary>
        public DbSet<Record> Records => Set<Record>();
    }
}
