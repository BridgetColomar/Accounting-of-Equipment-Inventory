using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Context.Connections
{
    public class SqliteDbContext : AppDbContext
    {
        public SqliteDbContext(DbContextOptions<SqliteDbContext> options)
            : base(options)   // передаём опции в базовый конструктор
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Использование SQLite в качестве базы данных
                optionsBuilder.UseSqlite("Data Source=EquipmentInventory.db");
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}

