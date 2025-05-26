using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Context.Connections
{
    public class SqliteDbContextFactory : IDesignTimeDbContextFactory<SqliteDbContext>
    {
        // Папка, в которой находится модуль (контекст)
        private const string _contextFolder = "AccountingOfEquipmentInventoryManagementDbContext";
        // Имя файла базы данных
        private const string _fileName = "EquipmentInventory.db";

        public SqliteDbContext CreateDbContext(string[] args)
        {
            // Определяем базовую директорию решения (поднимаясь от bin\Debug\netX)
            var solutionPath = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.ToString();

            // Формируем путь к папке контекста внутри решения (без использования названия корневой папки проекта)
            var dbDirectory = Path.Combine(solutionPath, _contextFolder);

            // Формируем полный путь к файлу базы данных
            var fullPath = Path.Combine(dbDirectory, _fileName);

            // Формирование строки подключения на основе полного пути
            var connectionString = $"Data Source={fullPath}";

            var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
            optionsBuilder.UseSqlite(connectionString);
            return new SqliteDbContext(optionsBuilder.Options);

        }
        public SqliteDbContext CreateDbContext()
        {
            return CreateDbContext(Array.Empty<string>());
        }
    }
}
