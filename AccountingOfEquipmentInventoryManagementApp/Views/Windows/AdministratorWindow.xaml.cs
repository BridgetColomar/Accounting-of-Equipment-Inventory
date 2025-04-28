using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AccountingOfEquipmentInventoryManagementApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdministratorWindow.xaml
    /// </summary>
    public partial class AdministratorWindow : Window
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IInventoryService _inventoryService;
        private readonly IEmployeeService _employeeService;

        // Пример коллекций для привязки к DataGrid или ListView
        public ObservableCollection<Equipment> Equipments { get; set; }
        public ObservableCollection<InventoryRecord> InventoryRecords { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }

        public AdministratorWindow(IEquipmentService equipmentService,
                                   IInventoryService inventoryService,
                                   IEmployeeService employeeService)
        {
            InitializeComponent();
            _equipmentService = equipmentService;
            _inventoryService = inventoryService;
            _employeeService = employeeService;

            Equipments = new ObservableCollection<Equipment>();
            InventoryRecords = new ObservableCollection<InventoryRecord>();
            Employees = new ObservableCollection<Employee>();

            DataContext = this; // Для простоты привязки коллекций к элементам управления
            Loaded += AdministratorWindow_Loaded;
        }

        private async void AdministratorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllDataAsync();
        }

        /// <summary>
        /// Загружает данные оборудования, инвентаризации и сотрудников.
        /// </summary>
        private async Task LoadAllDataAsync()
        {
            try
            {
                Equipments.Clear();
                InventoryRecords.Clear();
                Employees.Clear();

                var eqList = await _equipmentService.GetAllEquipmentAsync();
                foreach (var eq in eqList)
                    Equipments.Add(eq);

                var invList = await _inventoryService.GetAllInventoryRecordsAsync();
                foreach (var inv in invList)
                    InventoryRecords.Add(inv);

                var empList = await _employeeService.GetAllEmployeesAsync();
                foreach (var emp in empList)
                    Employees.Add(emp);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Пример обработчика для кнопки "Добавить оборудование"
        private async void BtnAddEquipment_Click(object sender, RoutedEventArgs e)
        {
            // Здесь можно показать диалоговое окно для ввода данных нового оборудования
            // Для простоты создаём оборудование с тестовыми данными
            var newEquipment = new Equipment
            {
                Name = "Новое оборудование",
                SerialNumber = Guid.NewGuid().ToString().Substring(0, 10),
                Status = EquipmentStatus.Active,
                PurchaseDate = DateTime.Now,
                Location = "Склад №1",
                // Image можно оставить пустым или задать базовый массив байтов
            };

            try
            {
                await _equipmentService.AddEquipmentAsync(newEquipment);
                Equipments.Add(newEquipment);
                MessageBox.Show("Оборудование успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Пример обновления оборудования (метод может быть расширен для редактирования выбранного элемента)
        private async void BtnEditEquipment_Click(object sender, RoutedEventArgs e)
        {
            var selectedEquipment = (Equipment)(sender as Button)?.DataContext;
            if (selectedEquipment == null)
            {
                MessageBox.Show("Выберите оборудование для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Например, изменим местоположение и статус (в реальном приложении данные берутся из формы редактирования)
            selectedEquipment.Location = "Отдел контроля";
            selectedEquipment.Status = EquipmentStatus.InMaintenance;

            try
            {
                await _equipmentService.UpdateEquipmentAsync(selectedEquipment);
                MessageBox.Show("Изменения сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Пример удаления оборудования
        private async void BtnDeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            var selectedEquipment = (Equipment)(sender as Button)?.DataContext;
            if (selectedEquipment == null)
            {
                MessageBox.Show("Выберите оборудование для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить выбранное оборудование?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    await _equipmentService.DeleteEquipmentAsync(selectedEquipment.Id);
                    Equipments.Remove(selectedEquipment);
                    MessageBox.Show("Оборудование удалено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}