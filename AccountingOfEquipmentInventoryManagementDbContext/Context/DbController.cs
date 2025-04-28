using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Context
{
    public class DbController
    {
        private readonly AppDbContext _context;

        public DbController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Применение всех миграций к базе данных.
        /// </summary>
        public void MigrateDatabase()
        {
            try
            {
                _context.Database.Migrate();
                Console.WriteLine("Миграция базы данных выполнена успешно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при миграции базы данных: {ex.Message}");
            }
        }
    }
}
