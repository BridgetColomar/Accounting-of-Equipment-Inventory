using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementLib.Entities
{
    /// <summary>
    /// Сущность "Оборудование" для учета в системе.
    /// </summary>
    public class Equipment
    {
        /// <summary>
        /// Уникальный идентификатор оборудования.
        /// </summary>
        public int Id  { get; set; }

        /// <summary>
        /// Наименование оборудования.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Уникальный серийный номер или идентификатор.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Статус оборудования.
        /// </summary>
        public EquipmentStatus Status { get; set; }

        /// <summary>
        /// Категория оборудования.
        /// </summary>
        public EquipmentCategory Category { get; set; }

        /// <summary>
        /// Дата покупки или поступления оборудования.
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Местоположение оборудования (например, склад, отдел или рабочее место).
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Изображение оборудования (хранится в виде массива байтов).
        /// </summary>
        public byte[] Image { get; set; }
    }
}
