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
        // Абсолютный путь к файлу базы данных 
        private const string AbsoluteDbPath = @"D:\C#\Accounting-of-Equipment-Inventory\AccountingOfEquipmentInventoryManagementDbContext\EquipmentInventory.db";
        // Строка подключения, указывающая на этот файл
        private readonly string _connectionString = $"Data Source={AbsoluteDbPath}";
        public SqliteDbContext(DbContextOptions<SqliteDbContext> options)
            : base(options)   // передаём опции в базовый конструктор
        {
            // Если файл базы данных не существует, создаем его
            if (!File.Exists(AbsoluteDbPath))
            {
                Database.EnsureCreated();
                Debug.WriteLine($"{this.GetType().Name} was created at {AbsoluteDbPath}.");
            }
            else
            {
                Debug.WriteLine($"{this.GetType().Name} connected to {AbsoluteDbPath}.");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Используем SQLite с заданной строкой подключения
            optionsBuilder.UseSqlite(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}

