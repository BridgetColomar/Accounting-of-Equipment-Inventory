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
        // Папка, в которой расположен контекст
        private const string _contextFolder = "AccountingOfEquipmentInventoryManagementDbContext";
        // Имя файла базы данных
        private const string _fileName = "EquipmentInventory.db";
        // Строка подключения, которая будет сформирована динамически
        private string _connectionString = "Data Source=EquipmentInventory.db";
        public SqliteDbContext(DbContextOptions<SqliteDbContext> options)
            : base(options)   // передаём опции в базовый конструктор
        {
            // Получаем базовую директорию решения (от bin\Debug\netX)
            var solutionPath = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.ToString();

            // Формируем путь к папке контекста внутри решения
            var dbDirectory = Path.Combine(solutionPath, _contextFolder);

            // Если директории не существует, создаем её
            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
                Debug.WriteLine($"Directory '{dbDirectory}' was created.");
            }

            // Формируем полный путь к файлу базы данных
            var fullPath = Path.Combine(dbDirectory, _fileName);
            _connectionString = $"Data Source={fullPath}";

            // Если файл базы данных отсутствует, создаём базу
            if (!File.Exists(fullPath))
            {
                Database.EnsureCreated();
                Debug.WriteLine($"{this.GetType().Name} was created at {fullPath}.");
            }
            else
            {
                Debug.WriteLine($"{this.GetType().Name} connected to {fullPath}.");
            }
        }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentCategory> EquipmentCategories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Настраиваем использование SQLite с полученной строкой подключения
            optionsBuilder.UseSqlite(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}

