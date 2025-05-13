using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Services
{
    public static class ReportService
    {
        public static async Task<List<Equipment>> GenerateEquipmentReportAsync(DateTime? startDate, DateTime? endDate, string categoryFilter)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
            using (var context = new SqliteDbContext(optionsBuilder.Options))
            {
                var query = context.Equipments.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(e => e.PurchaseDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(e => e.PurchaseDate <= endDate.Value);

                if (!string.IsNullOrEmpty(categoryFilter))
                    query = query.Where(e => e.Category.ToString().Contains(categoryFilter)); // Приведение к строке

                return await query.ToListAsync();
            }
        }
    }
}
