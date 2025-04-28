using AccountingOfEquipmentInventoryManagementDbContext.Context;
using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryRecord>> GetAllInventoryRecordsAsync()
        {
            return await _context.InventoryRecords
                                 .Include(ir => ir.Equipment)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<InventoryRecord>> GetInventoryRecordsByEquipmentIdAsync(int equipmentId)
        {
            return await _context.InventoryRecords
                                 .Where(ir => ir.Equipment.Id == equipmentId)
                                 .Include(ir => ir.Equipment)
                                 .ToListAsync();
        }

        public async Task AddInventoryRecordAsync(InventoryRecord record)
        {
            await _context.InventoryRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }
    }
}
