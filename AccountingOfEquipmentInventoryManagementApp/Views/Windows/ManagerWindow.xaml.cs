using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementDbContext.Context;
using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
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
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using AccountingOfEquipmentInventoryManagementDbContext.Services;

namespace AccountingOfEquipmentInventoryManagementApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        // Переменная для хранения выбранного изображения в виде массива байтов
        private byte[] selectedImageBytes = null;

        public ManagerWindow()
        {
            InitializeComponent();
            // Заполнение ComboBox с значениям enum EquipmentStatus
            LoadEquipmentStatusItems();
            // Загрузка категорий для выбора (ComboBox)
            _ = LoadEquipmentCategoriesAsync();
            // Загрузка списка оборудования (отчёт)
            _ = LoadEquipmentReportAsync();
        }

        // Заполняем ComboBox для статуса оборудования значениями из перечисления EquipmentStatus
        private void LoadEquipmentStatusItems()
        {
            cbEquipmentStatus.ItemsSource = Enum.GetValues(typeof(EquipmentStatus));
            cbEquipmentStatus.SelectedIndex = 0;
        }

        // Асинхронно загружаем категории оборудования из БД для ComboBox
        private async Task LoadEquipmentCategoriesAsync()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    var categories = await context.EquipmentCategories.ToListAsync();
                    cbEquipmentCategory.ItemsSource = categories;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки категорий: " + ex.Message);
                MessageBox.Show("Ошибка загрузки категорий: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Асинхронно загружаем список оборудования (с фильтрами, если переданы параметры)
        private async Task LoadEquipmentReportAsync(string searchCategory = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    IQueryable<Equipment> query = context.Equipments.Include(e => e.Category);
                    if (startDate.HasValue)
                        query = query.Where(e => e.PurchaseDate >= startDate.Value);
                    if (endDate.HasValue)
                        query = query.Where(e => e.PurchaseDate <= endDate.Value);
                    if (!string.IsNullOrEmpty(searchCategory))
                        query = query.Where(e => e.Category.Name == searchCategory);

                    var equipmentList = await query.ToListAsync();
                    ReportDataGrid.ItemsSource = equipmentList;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки оборудования: " + ex.Message);
                MessageBox.Show("Ошибка загрузки оборудования: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик для создания новой категории оборудования
        private async void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = tbNewCategoryName.Text.Trim();
            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Введите название категории.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newCategory = new EquipmentCategory { Name = categoryName };

                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    context.EquipmentCategories.Add(newCategory);
                    await context.SaveChangesAsync();
                }
                MessageBox.Show("Категория успешно создана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                tbNewCategoryName.Clear();
                await LoadEquipmentCategoriesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка создания категории: " + ex.Message);
                MessageBox.Show("Ошибка создания категории: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик для выбора изображения оборудования
        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    selectedImageBytes = File.ReadAllBytes(ofd.FileName);
                    MessageBox.Show("Изображение успешно загружено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка при загрузке изображения: " + ex.Message);
                    MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обработчик для добавления нового оборудования в БД
        private async void btnAddEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = tbEquipmentName.Text.Trim();
                string serialNumber = tbSerialNumber.Text.Trim();
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(serialNumber))
                {
                    MessageBox.Show("Введите наименование и серийный номер оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cbEquipmentStatus.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                EquipmentStatus status = (EquipmentStatus)cbEquipmentStatus.SelectedItem;

                if (cbEquipmentCategory.SelectedItem == null)
                {
                    MessageBox.Show("Выберите категорию оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                EquipmentCategory category = (EquipmentCategory)cbEquipmentCategory.SelectedItem;

                DateTime? purchaseDate = dpPurchaseDate.SelectedDate;
                if (!purchaseDate.HasValue)
                {
                    MessageBox.Show("Выберите дату покупки оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string location = tbLocation.Text.Trim();
                if (string.IsNullOrEmpty(location))
                {
                    MessageBox.Show("Введите местоположение оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Equipment newEquipment = new Equipment
                {
                    Name = name,
                    SerialNumber = serialNumber,
                    Status = status,
                    Category = category,
                    PurchaseDate = purchaseDate.Value,
                    Location = location,
                    Image = selectedImageBytes
                };

                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    context.Equipments.Add(newEquipment);
                    await context.SaveChangesAsync();
                }

                MessageBox.Show("Оборудование успешно добавлено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                tbEquipmentName.Clear();
                tbSerialNumber.Clear();
                tbLocation.Clear();
                dpPurchaseDate.SelectedDate = null;
                cbEquipmentStatus.SelectedIndex = 0;
                cbEquipmentCategory.SelectedIndex = -1;
                selectedImageBytes = null;
                await LoadEquipmentReportAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка добавления оборудования: " + ex.Message);
                MessageBox.Show("Ошибка добавления оборудования: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик формирования отчета по оборудованию (с фильтрами)
        private async void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime? startDate = dpStartDate.SelectedDate;
                DateTime? endDate = dpEndDate.SelectedDate;
                string categoryFilter = tbCategoryFilter.Text.Trim();

                await LoadEquipmentReportAsync(categoryFilter, startDate, endDate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка формирования отчета: " + ex.Message);
                MessageBox.Show("Ошибка формирования отчета: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик экспорта отчета в CSV-файл
        private void btnExportCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportData = ReportDataGrid.ItemsSource as IEnumerable<Equipment>;
                if (reportData == null || !reportData.Any())
                {
                    MessageBox.Show("Нет данных для экспорта.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var sfd = new SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                    FileName = "EquipmentReport.csv"
                };

                if (sfd.ShowDialog() == true)
                {
                    using (var sw = new StreamWriter(sfd.FileName))
                    {
                        sw.WriteLine("Id,Name,SerialNumber,Status,Category,PurchaseDate,Location");
                        foreach (var item in reportData)
                        {
                            sw.WriteLine($"{item.Id},{EscapeCsv(item.Name)},{EscapeCsv(item.SerialNumber)},{item.Status},{EscapeCsv(item.Category.Name)},{item.PurchaseDate:yyyy-MM-dd HH:mm:ss},{EscapeCsv(item.Location)}");
                        }
                    }
                    MessageBox.Show("Экспорт завершён успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка экспорта CSV: " + ex.Message);
                MessageBox.Show("Ошибка экспорта CSV: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод экранирования строк для экспорта CSV
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}