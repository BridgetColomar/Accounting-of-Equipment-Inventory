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
using Microsoft.Extensions.Options;

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
            // Заполнение ComboBox с значениями из enum EquipmentStatus
            LoadEquipmentStatusItems();
            // Загрузка категорий для выбора
            _ = LoadEquipmentCategoriesAsync();
            // Загрузка отчёта оборудования
            _ = LoadEquipmentReportAsync();
            // При загрузке окна обновляем поле ID оборудования
            this.Loaded += async (s, e) => await UpdateNextEquipmentIdAsync();
        }

        // Заполнение ComboBox для статуса оборудования значениями из перечисления EquipmentStatus
        private void LoadEquipmentStatusItems()
        {
            cbEquipmentStatus.ItemsSource = Enum.GetValues(typeof(EquipmentStatus));
            cbEquipmentStatus.SelectedIndex = 0;
        }

        // Асинхронная загрузка категорий оборудования из БД для ComboBox
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

        // Асинхронная загрузка отчёта оборудования (с фильтрами, если заданы параметры)
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

        // Метод вычисления следующего уникального идентификатора оборудования (тип int)
        private async Task<int> GetNextEquipmentIdAsync()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    int? maxId = await context.Equipments.MaxAsync(e => (int?)e.Id);
                    return (maxId ?? 0) + 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка получения следующего ID: " + ex.Message);
                return 1;
            }
        }

        // Обновление поля ID оборудования (в текстовом поле) до следующего значения
        private async Task UpdateNextEquipmentIdAsync()
        {
            int nextId = await GetNextEquipmentIdAsync();
            tbEquipmentId.Text = nextId.ToString();
        }

        // Обработчик создания новой категории оборудования
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

        // Обработчик выбора изображения оборудования
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

        // Обработчик добавления нового оборудования в БД
        private async void btnAddEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Считывание данных с формы
                string name = tbEquipmentName.Text.Trim();
                string serialNumber = tbSerialNumber.Text.Trim();
                // Дополнительные проверки и считывание других значений

                // Создание объекта нового оборудования (без заполнения поля Category)
                Equipment newEquipment = new Equipment
                {
                    // Если поле Id автоинкрементное, его не нужно заполнять
                    Name = name,
                    SerialNumber = serialNumber,
                    Status = (EquipmentStatus)cbEquipmentStatus.SelectedItem,
                    // Category будет назначена позже
                    PurchaseDate = dpPurchaseDate.SelectedDate.Value,
                    Location = tbLocation.Text.Trim(),
                    Image = selectedImageBytes
                };

                // Создание нового контекста БД и корректная привязка выбранной категории
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    var selectedCategory = cbEquipmentCategory.SelectedItem as EquipmentCategory;
                    if (selectedCategory == null)
                    {
                        MessageBox.Show("Выберите категорию оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    // Получаем категорию из текущего контекста
                    var attachedCategory = context.EquipmentCategories.FirstOrDefault(c => c.Id == selectedCategory.Id);
                    if (attachedCategory == null)
                    {
                        MessageBox.Show("Выбранная категория не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    newEquipment.Category = attachedCategory;

                    // Добавляем оборудование и сохраняем изменения в базе данных
                    context.Equipments.Add(newEquipment);
                    await context.SaveChangesAsync();
                }

                MessageBox.Show("Оборудование успешно добавлено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                // Очистка формы и обновление UI
            }
            catch (Exception ex)
            {
                string errorDetails = ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString();
                Debug.WriteLine("Ошибка добавления оборудования: " + errorDetails);
                MessageBox.Show("Ошибка добавления оборудования: " + errorDetails, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик формирования отчёта по оборудованию (с фильтрами)
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
                Debug.WriteLine("Ошибка формирования отчёта: " + ex.Message);
                MessageBox.Show("Ошибка формирования отчёта: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик экспорта отчёта в CSV-файл
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

        // Метод экранирования строк для экспорта в CSV
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