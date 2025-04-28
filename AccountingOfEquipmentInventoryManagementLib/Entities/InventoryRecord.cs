using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementLib.Entities
{
    /// <summary>
    /// Класс, представляющий запись инвентаризации оборудования.
    /// </summary>
    public class InventoryRecord
    {
        /// <summary>
        /// Уникальный идентификатор записи инвентаризации.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Оборудование, к которому относится запись.
        /// </summary>
        public Equipment Equipment { get; set; }

        /// <summary>
        /// Дата проведения инвентаризации.
        /// </summary>
        public DateTime RecordDate { get; set; }

        /// <summary>
        /// Статус оборудования зафиксированный в момент инвентаризации.
        /// </summary>
        public EquipmentStatus RecordedStatus { get; set; }

        /// <summary>
        /// Дополнительные примечания (например, замечания по неисправностям или необходимости обслуживания).
        /// </summary>
        public string Note { get; set; }
    }
}
