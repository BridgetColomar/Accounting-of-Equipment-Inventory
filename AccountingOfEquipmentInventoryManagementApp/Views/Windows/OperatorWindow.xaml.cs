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
    /// Логика взаимодействия для OperatorWindow.xaml
    /// </summary>
    public partial class OperatorWindow : Window
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IInventoryService _inventoryService;

        public ObservableCollection<Equipment> Equipments { get; set; }

        public OperatorWindow(IEquipmentService equipmentService, IInventoryService inventoryService)
        {
            InitializeComponent();
            _equipmentService = equipmentService;
            _inventoryService = inventoryService;
            Equipments = new ObservableCollection<Equipment>();
            DataContext = this;
            Loaded += OperatorWindow_Loaded;
        }

        private async void OperatorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadEquipmentsAsync();
        }

        private async Task LoadEquipmentsAsync()
        {
            try
            {
                Equipments.Clear();
                var equipments = await _equipmentService.GetAllEquipmentAsync();
                foreach (var eq in equipments)
                {
                    Equipments.Add(eq);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик для сохранения результатов инвентаризации оператора
        private async void BtnRecordInventory_Click(object sender, RoutedEventArgs e)
        {
            // Предполагается, что в XAML имеется:
            // • ComboBox для выбора оборудования (Name="CmbEquipment")
            // • ComboBox для выбора нового состояния (Name="CmbStatus")
            // • TextBox для ввода комментария (Name="TxtNote")
            if (CmbEquipment.SelectedItem is Equipment selectedEquipment &&
                CmbStatus.SelectedItem is ComboBoxItem statusItem &&
                Enum.TryParse(statusItem.Tag.ToString(), out EquipmentStatus newStatus))
            {
                string note = TxtNote.Text.Trim();
                try
                {
                    await _inventoryService.AddInventoryRecordAsync(new InventoryRecord
                    {
                        Equipment = selectedEquipment, // или можно передать Id
                        RecordDate = DateTime.Now,
                        RecordedStatus = newStatus,
                        Note = note
                    });
                    MessageBox.Show("Запись инвентаризации успешно создана", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите оборудование и новый статус", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
