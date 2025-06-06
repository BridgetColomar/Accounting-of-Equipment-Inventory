using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementDbContext.Services;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Context
{
    public class DbController
    {
        private readonly SqliteDbContext _context;
        private readonly EquipmentService _equipmentService;

        public DbController(SqliteDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _equipmentService = new EquipmentService(_context);
        }

        /// <summary>
        /// Применение всех миграций к базе данных.
        /// </summary>
        public void MigrateDatabase()
        {
            try
            {
                _context.Database.Migrate();
                Console.WriteLine("Миграция базы данных выполнена успешно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при миграции базы данных: {ex.Message}");
            }
        }

        /// <summary>
        /// Добавляет новую категорию оборудования и сохраняет изменения в БД.
        /// </summary>
        public async Task AddCategoryAsync(EquipmentCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            await _context.EquipmentCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет оборудование по его ID и сохраняет изменения в БД.
        /// </summary>
        public async Task DeleteEquipmentAsync(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null)
                return;

            _context.Equipments.Remove(equipment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Добавляет новое оборудование с учетом бизнес-логики и сохраняет в БД.
        /// </summary>
        public async Task AddEquipmentAsync(Equipment equipment)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            // При необходимости: дополнительная валидация или иная бизнес-логика
            await _context.Equipments.AddAsync(equipment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Обновляет данные существующего оборудования и сохраняет изменения в БД.
        /// </summary>
        public async Task UpdateEquipmentAsync(Equipment equipment)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            _context.Equipments.Update(equipment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Возвращает список всех категорий оборудования.
        /// </summary>
        public async Task<List<EquipmentCategory>> GetAllCategoriesAsync()
        {
            return await _context.EquipmentCategories.ToListAsync();
        }

        /// <summary>
        /// Возвращает список всего оборудования.
        /// </summary>
        public async Task<List<Equipment>> GetAllEquipmentsAsync()
        {
            return await _context.Equipments
                .Include(e => e.Category) // подключаем категорию, если нужна привязка
                .ToListAsync();
        }

    }

}
