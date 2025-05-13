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
        public ManagerWindow()
        {
            InitializeComponent();
            // Загружаем список категорий в оба ComboBox'а: для фильтра и для добавления оборудования
            _ = LoadEquipmentCategoriesAsync();
            // Загружаем список оборудования
            _ = LoadEquipmentReportAsync();
        }

        // Метод загрузки всех категорий оборудования из БД для ComboBox'а
        private async Task LoadEquipmentCategoriesAsync()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    // Загружаем категории для выборки оборудования и для добавления нового оборудования
                    var categories = await context.EquipmentCategories.ToListAsync();
                    // Если у вас в окне менеджера есть ComboBox для выбора категории при добавлении оборудования:
                    cbEquipmentCategory.ItemsSource = categories;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки категорий: " + ex.Message);
                MessageBox.Show("Ошибка загрузки категорий: " + ex.Message, "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод загрузки списка оборудования (с возможной фильтрацией)
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
                MessageBox.Show("Ошибка загрузки оборудования: " + ex.Message, "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                var newCategory = new EquipmentCategory
                {
                    Name = categoryName
                };

                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    context.EquipmentCategories.Add(newCategory);
                    await context.SaveChangesAsync();
                }
                MessageBox.Show("Категория успешно создана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                tbNewCategoryName.Clear();
                // Обновляем ComboBox, если он используется при добавлении оборудования
                await LoadEquipmentCategoriesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка создания категории: " + ex.Message);
                MessageBox.Show("Ошибка создания категории: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик добавления оборудования в БД
        private async void btnAddEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = tbEquipmentName.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Введите название оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedCategory = cbEquipmentCategory.SelectedItem as EquipmentCategory;
                if (selectedCategory == null)
                {
                    MessageBox.Show("Выберите категорию оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newEquipment = new Equipment
                {
                    Name = name,
                    // Присваиваем созданную категорию
                    Category = selectedCategory,
                    PurchaseDate = DateTime.Now
                };

                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    context.Equipments.Add(newEquipment);
                    await context.SaveChangesAsync();
                }

                MessageBox.Show("Оборудование успешно добавлено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                // Очистка полей
                tbEquipmentName.Clear();
                cbEquipmentCategory.SelectedIndex = -1;
                await LoadEquipmentReportAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка добавления оборудования: " + ex.Message);
                MessageBox.Show("Ошибка добавления оборудования: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик формирования отчёта по оборудованию
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

        // Обработчик экспорта отчёта в CSV
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
                        // Заголовки CSV
                        sw.WriteLine("Id,Name,Category,DateAdded");
                        foreach (var item in reportData)
                        {
                            sw.WriteLine($"{item.Id},{EscapeCsv(item.Name)},{EscapeCsv(item.Category.Name)},{item.PurchaseDate:yyyy-MM-dd HH:mm:ss}");
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

        // Метод для экранирования текстовых значений для CSV-экспорта
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