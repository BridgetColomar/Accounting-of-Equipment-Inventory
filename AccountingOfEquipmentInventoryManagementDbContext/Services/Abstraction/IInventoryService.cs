using AccountingOfEquipmentInventoryManagementLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryRecord>> GetAllInventoryRecordsAsync();
        Task<IEnumerable<InventoryRecord>> GetInventoryRecordsByEquipmentIdAsync(int equipmentId);
        Task AddInventoryRecordAsync(InventoryRecord record);
    }
}
