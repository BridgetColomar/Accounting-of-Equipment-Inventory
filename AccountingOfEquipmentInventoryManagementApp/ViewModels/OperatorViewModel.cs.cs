using AccountingOfEquipmentInventoryManagementApp.Helpers;
using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountingOfEquipmentInventoryManagementApp.ViewModels
{
    public class OperatorViewModel : INotifyPropertyChanged
    {
        #region Свойства привязки

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set { _searchTerm = value; OnPropertyChanged(nameof(SearchTerm)); }
        }

        private EquipmentCategory _selectedCategoryFilter;
        public EquipmentCategory SelectedCategoryFilter
        {
            get => _selectedCategoryFilter;
            set { _selectedCategoryFilter = value; OnPropertyChanged(nameof(SelectedCategoryFilter)); }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        // Коллекция оборудования для DataGrid
        public ObservableCollection<Equipment> InventoryList { get; set; } = new ObservableCollection<Equipment>();

        // Коллекция категорий для ComboBox
        public ObservableCollection<EquipmentCategory> Categories { get; set; } = new ObservableCollection<EquipmentCategory>();

        // Если необходимо, можно добавить свойство для выбранного оборудования
        private Equipment _selectedEquipment;
        public Equipment SelectedEquipment
        {
            get => _selectedEquipment;
            set { _selectedEquipment = value; OnPropertyChanged(nameof(SelectedEquipment)); }
        }

        #endregion

        #region Команды

        public ICommand SearchCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Конструктор и инициализация

        public OperatorViewModel()
        {
            // Инициализация команд через RelayCommand
            SearchCommand = new RelayCommand(async (o) => await LoadInventoryAsync(SearchTerm, SelectedCategoryFilter?.Name));
            ClearFilterCommand = new RelayCommand(async (o) =>
            {
                SearchTerm = string.Empty;
                SelectedCategoryFilter = null;
                await LoadInventoryAsync();
            });
            RefreshCommand = new RelayCommand(async (o) => await LoadInventoryAsync(SearchTerm, SelectedCategoryFilter?.Name));

            // Одновременно загружаем категории и данные
            _ = LoadCategoryFilterAsync();
            _ = LoadInventoryAsync();
        }

        #endregion

        #region Методы загрузки данных

        // Загрузка списка категорий для фильтра (ComboBox)
        private async Task LoadCategoryFilterAsync()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    var categories = await context.EquipmentCategories.ToListAsync();
                    Categories.Clear();
                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                }
            }
            catch (Exception ex)
            {
                Status = $"Ошибка загрузки категорий: {ex.Message}";
                // Логирование ошибки можно добавить здесь, если необходимо
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

                    // Обновление коллекции
                    InventoryList.Clear();
                    foreach (var eq in equipmentList)
                        InventoryList.Add(eq);

                    Status = $"Загружено {equipmentList.Count} записей.";
                }
            }
            catch (Exception ex)
            {
                Status = $"Ошибка загрузки данных: {ex.Message}";
            }
        }

        #endregion

        #region Реализация INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
