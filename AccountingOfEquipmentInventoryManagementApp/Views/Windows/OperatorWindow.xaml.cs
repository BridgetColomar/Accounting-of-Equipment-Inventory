using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
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
        public OperatorWindow()
        {
            InitializeComponent();
            _ = LoadCategoryFilterAsync();
            _ = LoadInventoryAsync();
        }

        // Загрузка списка категорий для фильтра (ComboBox)
        private async Task LoadCategoryFilterAsync()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    var categories = await context.EquipmentCategories.ToListAsync();
                    cbCategoryFilter.ItemsSource = categories;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки категорий: " + ex.Message);
                MessageBox.Show("Ошибка загрузки категорий: " + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод загрузки данных об оборудовании с фильтрами
        private async Task LoadInventoryAsync(string searchTerm = null, string categoryName = null)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    // Подгружаем данные об оборудовании с включением категории
                    IQueryable<Equipment> query = context.Equipments.Include(e => e.Category);

                    // Если задан поисковый запрос, фильтруем по наименованию оборудования
                    if (!string.IsNullOrEmpty(searchTerm))
                        query = query.Where(e => EF.Functions.Like(e.Name, $"%{searchTerm}%"));

                    // Если выбрана категория – фильтруем по точному совпадению названия категории
                    if (!string.IsNullOrEmpty(categoryName))
                        query = query.Where(e => e.Category.Name == categoryName);

                    var equipmentList = await query.ToListAsync();
                    InventoryDataGrid.ItemsSource = equipmentList;
                    tbStatus.Text = $"Загружено {equipmentList.Count} записей.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки данных: " + ex.Message);
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик кнопки "Обновить данные"
        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadInventoryAsync(tbSearch.Text, (cbCategoryFilter.SelectedItem as EquipmentCategory)?.Name);
        }

        // Обработчик кнопки "Поиск"
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            await LoadInventoryAsync(tbSearch.Text, (cbCategoryFilter.SelectedItem as EquipmentCategory)?.Name);
        }

        // Обработчик кнопки "Сбросить"
        private async void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Clear();
            cbCategoryFilter.SelectedIndex = -1;
            await LoadInventoryAsync();
        }
    }
}
