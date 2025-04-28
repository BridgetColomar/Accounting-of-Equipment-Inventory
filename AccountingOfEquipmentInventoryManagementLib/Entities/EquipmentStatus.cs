using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementLib.Entities
{
    /// <summary>
    /// Перечисление возможных статусов оборудования.
    /// </summary>
    public enum EquipmentStatus
    {
        Active,         // Оборудование активно используется
        InMaintenance,  // Оборудование находится на обслуживании
        Decommissioned, // Оборудование выведено из эксплуатации
        Unknown         // Статус не определён
    }
}
