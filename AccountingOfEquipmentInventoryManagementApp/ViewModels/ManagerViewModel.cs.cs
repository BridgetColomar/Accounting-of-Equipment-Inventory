using AccountingOfEquipmentInventoryManagementApp.Helpers;
using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AccountingOfEquipmentInventoryManagementApp.ViewModels
{
    public class ManagerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<EquipmentCategory> EquipmentCategories { get; set; } = new();
        public ObservableCollection<string> Locations { get; set; } = new();
        public ObservableCollection<Equipment> EquipmentReport { get; set; } = new();
        public Array EquipmentStatuses => Enum.GetValues(typeof(EquipmentStatus));
        public EquipmentCategory SelectedCategory { get; set; }
        public string SelectedLocation { get; set; }
        public EquipmentStatus SelectedStatus { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public string EquipmentName { get; set; }
        public string SerialNumber { get; set; }
        public int EquipmentId { get; set; }

        public byte[] SelectedImageBytes { get; set; }

        public ICommand AddEquipmentCommand { get; }
        public ICommand LoadCategoriesCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand DeleteEquipmentCommand { get; }
        public ICommand ExportCommand { get; }

        public ManagerViewModel()
        {
            AddEquipmentCommand = new RelayCommand(async _ => await AddEquipmentAsync());
            LoadCategoriesCommand = new RelayCommand(async _ => await LoadEquipmentCategoriesAsync());
            SelectImageCommand = new RelayCommand(_ => SelectImage());
            AddCategoryCommand = new RelayCommand(async _ => await AddCategoryAsync());
            GenerateReportCommand = new RelayCommand(async _ => await LoadEquipmentReportAsync());
            DeleteEquipmentCommand = new RelayCommand(async _ => await DeleteEquipmentAsync());
            ExportCommand = new RelayCommand(_ => ExportToFile());

            LoadDefaults();
        }

        private void LoadDefaults()
        {
            LoadLocations();
            _ = LoadEquipmentCategoriesAsync();
            _ = UpdateNextEquipmentIdAsync();
            _ = LoadEquipmentReportAsync();
        }

        private void LoadLocations()
        {
            Locations.Clear();
            var predefined = new[] { "Головной офис", "Офис 101", "Офис 102", "Склад", "Филиал" };
            foreach (var loc in predefined)
                Locations.Add(loc);
            SelectedLocation = Locations.FirstOrDefault();
        }

        private async Task LoadEquipmentCategoriesAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
            using var context = new SqliteDbContext(optionsBuilder.Options);
            var categories = await context.EquipmentCategories.ToListAsync();
            EquipmentCategories.Clear();
            foreach (var category in categories)
                EquipmentCategories.Add(category);
        }

        private async Task AddEquipmentAsync()
        {
            if (string.IsNullOrWhiteSpace(EquipmentName) || string.IsNullOrWhiteSpace(SerialNumber))
            {
                MessageBox.Show("Введите наименование и серийный номер оборудования.");
                return;
            }

            if (SelectedCategory == null || !PurchaseDate.HasValue || string.IsNullOrEmpty(SelectedLocation))
            {
                MessageBox.Show("Заполните все обязательные поля.");
                return;
            }

            var equipment = new Equipment
            {
                Id = EquipmentId,
                Name = EquipmentName,
                SerialNumber = SerialNumber,
                Status = SelectedStatus,
                Category = SelectedCategory,
                PurchaseDate = PurchaseDate.Value,
                Location = SelectedLocation,
                Image = SelectedImageBytes
            };

            var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
            using var context = new SqliteDbContext(optionsBuilder.Options);
            context.Equipments.Add(equipment);
            await context.SaveChangesAsync();

            MessageBox.Show("Оборудование добавлено.");
            await UpdateNextEquipmentIdAsync();
            EquipmentName = SerialNumber = string.Empty;
            PurchaseDate = null;
            SelectedImageBytes = null;
            await LoadEquipmentReportAsync();
        }

        private async Task UpdateNextEquipmentIdAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
            using var context = new SqliteDbContext(optionsBuilder.Options);
            EquipmentId = (await context.Equipments.MaxAsync(e => (int?)e.Id) ?? 0) + 1;
        }

        private async Task LoadEquipmentReportAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
            using var context = new SqliteDbContext(optionsBuilder.Options);
            var report = await context.Equipments.Include(e => e.Category).ToListAsync();
            EquipmentReport.Clear();
            foreach (var equipment in report)
                EquipmentReport.Add(equipment);
        }

        private void SelectImage()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
                SelectedImageBytes = File.ReadAllBytes(dialog.FileName);
        }

        private async Task AddCategoryAsync()
        {
            // Логика добавления новой категории (ввод через диалог и сохранение в БД)
        }

        private async Task DeleteEquipmentAsync()
        {
            // Логика удаления выбранного оборудования (по ID или другому критерию)
        }

        private void ExportToFile()
        {
            // Логика экспорта отчета в Excel, CSV или другой формат
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
