using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementLib.Entities
{
    /// <summary>
    /// Класс для управления операциями инвентаризации.
    /// </summary>
    public class InventoryManager
    {
        private readonly List<Equipment> _equipmentList;
        private readonly List<InventoryRecord> _inventoryRecords;

        public InventoryManager()
        {
            _equipmentList = new List<Equipment>();
            _inventoryRecords = new List<InventoryRecord>();
        }

        /// <summary>
        /// Добавление нового оборудования в систему учета.
        /// </summary>
        /// <param name="equipment">Экземпляр Equipment для добавления.</param>
        public void AddEquipment(Equipment equipment)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            _equipmentList.Add(equipment);
        }

        /// <summary>
        /// Создание записи инвентаризации для указанного оборудования.
        /// </summary>
        /// <param name="equipmentId">Идентификатор оборудования.</param>
        /// <param name="recordedStatus">Фактический статус оборудования на момент инвентаризации.</param>
        /// <param name="note">Дополнительные заметки (опционально).</param>
        public void RecordInventory(int equipmentId, EquipmentStatus recordedStatus, string note = null)
        {
            // Поиск оборудования по идентификатору
            var equipment = _equipmentList.Find(e => e.Id == equipmentId);
            if (equipment == null)
            {
                throw new ArgumentException("Оборудование с указанным идентификатором не найдено.", nameof(equipmentId));
            }

            var record = new InventoryRecord
            {
                Id = _inventoryRecords.Count + 1,
                Equipment = equipment,
                RecordDate = DateTime.Now,
                RecordedStatus = recordedStatus,
                Note = note
            };

            _inventoryRecords.Add(record);
        }

        /// <summary>
        /// Получение списка всех записей инвентаризации.
        /// </summary>
        /// <returns>Перечисление записей инвентаризации.</returns>
        public IEnumerable<InventoryRecord> GetInventoryRecords()
        {
            return _inventoryRecords;
        }

        /// <summary>
        /// Получение списка всего оборудования, зарегистрированного в системе.
        /// </summary>
        /// <returns>Перечисление оборудования.</returns>
        public IEnumerable<Equipment> GetEquipment()
        {
            return _equipmentList;
        }
    }
}

