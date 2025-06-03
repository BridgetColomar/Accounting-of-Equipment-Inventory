using AccountingOfEquipmentInventoryManagementApp.Helpers;
using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementDbContext.Services;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
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
using AccountingOfEquipmentInventoryManagementDbContext.Context;
using DocumentFormat.OpenXml.InkML;
using System.Windows.Media.Imaging;
using AccountingOfEquipmentInventoryManagementApp.Views.Windows;

namespace AccountingOfEquipmentInventoryManagementApp.ViewModels
{
    public class ManagerViewModel : INotifyPropertyChanged
    {

        private readonly DbController _dbController;

        public ObservableCollection<Equipment> EquipmentList { get; set; } = new();
        private Equipment _selectedEquipment;
        public Equipment SelectedEquipment
        {
            get => _selectedEquipment;
            set
            {
                _selectedEquipment = value;
                OnPropertyChanged();
            }
        }
        // Коллекции
        public ObservableCollection<EquipmentCategory> EquipmentCategories { get; set; } = new();
        public ObservableCollection<string> Locations { get; set; } = new();
        public ObservableCollection<Equipment> EquipmentReport { get; set; } = new();

        public Array EquipmentStatuses => Enum.GetValues(typeof(EquipmentStatus));
        private string _newCategoryName;

        public string NewCategoryName
        {
            get => _newCategoryName;
            set
            {
                _newCategoryName = value;
                OnPropertyChanged(nameof(NewCategoryName));
            }
        }
        // Выбранные элементы и фильтры
        private EquipmentCategory _selectedCategory;
        public EquipmentCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedLocation;
        public string SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                if (_selectedLocation != value)
                {
                    _selectedLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        private EquipmentStatus _selectedStatus;
        public EquipmentStatus SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private EquipmentCategory _selectedReportCategory;
        public EquipmentCategory SelectedReportCategory
        {
            get => _selectedReportCategory;
            set
            {
                if (_selectedReportCategory != value)
                {
                    _selectedReportCategory = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _reportStartDate;
        public DateTime? ReportStartDate
        {
            get => _reportStartDate;
            set
            {
                if (_reportStartDate != value)
                {
                    _reportStartDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _reportEndDate;
        public DateTime? ReportEndDate
        {
            get => _reportEndDate;
            set
            {
                if (_reportEndDate != value)
                {
                    _reportEndDate = value;
                    OnPropertyChanged();
                }
            }
        }

        // Параметры для добавления оборудования
        private string _equipmentName;
        public string EquipmentName
        {
            get => _equipmentName;
            set
            {
                if (_equipmentName != value)
                {
                    _equipmentName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get => _serialNumber;
            set
            {
                if (_serialNumber != value)
                {
                    _serialNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _equipmentId;
        public int EquipmentId
        {
            get => _equipmentId;
            set
            {
                if (_equipmentId != value)
                {
                    _equipmentId = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _purchaseDate;
        public DateTime? PurchaseDate
        {
            get => _purchaseDate;
            set
            {
                if (_purchaseDate != value)
                {
                    _purchaseDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte[] _selectedImageBytes;
        public byte[] SelectedImageBytes
        {
            get => _selectedImageBytes;
            set
            {
                if (_selectedImageBytes != value)
                {
                    _selectedImageBytes = value;
                    OnPropertyChanged();
                }
            }
        }

        // Команды
        public ICommand AddEquipmentCommand { get; }
        public ICommand LoadCategoriesCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand DeleteEquipmentCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ViewImageCommand { get; }

        public ManagerViewModel()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
            var context = new SqliteDbContext(optionsBuilder.Options);
            _dbController = new DbController(context);

            AddEquipmentCommand = new RelayCommand(async _ => await AddEquipmentAsync());
            LoadCategoriesCommand = new RelayCommand(async _ => await LoadEquipmentCategoriesAsync());
            SelectImageCommand = new RelayCommand(_ => SelectImage());
            AddCategoryCommand = new RelayCommand(async _ => await AddCategoryAsync());
            GenerateReportCommand = new RelayCommand(async _ => await LoadEquipmentReportAsync());
            DeleteEquipmentCommand = new RelayCommand(async _ => await DeleteEquipmentAsync());
            ExportCommand = new RelayCommand(async _ =>
            {
                await ReportExporter.ExportToFileAsync(ReportStartDate, ReportEndDate, SelectedReportCategory?.Name);
            });
            ViewImageCommand = new RelayCommand(OnViewImage, param => param is byte[]);
            LoadDefaults();
        }

        private void LoadDefaults()
        {
            LoadLocations();
            _ = LoadEquipmentCategoriesAsync();
            _ = UpdateNextEquipmentIdAsync();
            _ = LoadEquipmentReportAsync();
        }
        private void OnViewImage(object parameter)
        {
            if (parameter is byte[] imageData && imageData.Length > 0)
            {
                // Открываем окно с изображением, передаем байтовый массив в конструктор.
                var imageWindow = new ImageWindow(imageData);
                imageWindow.ShowDialog();
            }
        }
        private void LoadLocations()
        {
            Locations.Clear();
            var predefined = new[] { "Головной офис", "Офис 101", "Офис 102", "Склад", "Филиал" };
            foreach (var loc in predefined)
                Locations.Add(loc);
            SelectedLocation = Locations.Count > 0 ? Locations[0] : null;
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

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using var context = new SqliteDbContext(optionsBuilder.Options);

                // Получение категории из базы
                var categoryFromDb = await context.EquipmentCategories.FirstOrDefaultAsync(c => c.Id == SelectedCategory.Id);

                if (categoryFromDb == null)
                {
                    MessageBox.Show("Выбранная категория не найдена в базе данных.");
                    return;
                }

                var equipment = new Equipment
                {
                    Name = EquipmentName,
                    SerialNumber = SerialNumber,
                    Status = SelectedStatus,
                    PurchaseDate = PurchaseDate.Value,
                    Location = SelectedLocation,
                    Image = SelectedImageBytes,
                    Category = categoryFromDb
                };

                context.Equipments.Add(equipment);
                await context.SaveChangesAsync();

                MessageBox.Show("Оборудование добавлено.");

                await UpdateNextEquipmentIdAsync();

                // Очистка полей после добавления
                EquipmentName = SerialNumber = string.Empty;
                PurchaseDate = null;
                SelectedImageBytes = null;

                await LoadEquipmentReportAsync();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении оборудования: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
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

            var query = context.Equipments.Include(e => e.Category).AsQueryable();

            if (ReportStartDate.HasValue)
                query = query.Where(e => e.PurchaseDate >= ReportStartDate.Value);

            if (ReportEndDate.HasValue)
                query = query.Where(e => e.PurchaseDate <= ReportEndDate.Value);

            if (SelectedReportCategory != null)
                query = query.Where(e => e.Category.Id == SelectedReportCategory.Id);

            var filteredReport = await query.ToListAsync();

            EquipmentReport.Clear();
            foreach (var equipment in filteredReport)
                EquipmentReport.Add(equipment);
        }


        private void SelectImage()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                SelectedImageBytes = File.ReadAllBytes(dialog.FileName);
        }
        private void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
        private async Task AddCategoryAsync()
        {
            try
            {
                string name = NewCategoryName?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    ShowMessage("Введите название категории.");
                    return;
                }

                if (EquipmentCategories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    ShowMessage("Такая категория уже существует.");
                    return;
                }

                var newCategory = new EquipmentCategory { Name = name };
                await _dbController.AddCategoryAsync(newCategory);

                EquipmentCategories.Add(newCategory); // Обновление UI
                NewCategoryName = string.Empty;

                ShowMessage("Категория успешно добавлена.");
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при добавлении категории: {ex.Message}");
            }
        }
        private async Task DeleteEquipmentAsync()
        {
            try
            {
                if (SelectedEquipment == null)
                {
                    ShowMessage("Выберите оборудование для удаления.");
                    return;
                }

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить \"{SelectedEquipment.Name}\"?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result != MessageBoxResult.Yes)
                    return;

                await _dbController.DeleteEquipmentAsync(SelectedEquipment.Id);

                EquipmentReport.Remove(SelectedEquipment);
                EquipmentList.Remove(SelectedEquipment);
                SelectedEquipment = null;

                ShowMessage("Оборудование удалено.");
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при удалении оборудования: {ex.Message}");
            }
        }
        public static class ReportExporter
        {
            public static async Task ExportToFileAsync(DateTime? startDate, DateTime? endDate, string categoryFilter)
            {
                var equipments = await ReportService.GenerateEquipmentReportAsync(startDate, endDate, categoryFilter);

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx|PDF files (*.pdf)|*.pdf|CSV files (*.csv)|*.csv",
                    FileName = "EquipmentReport"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                string filePath = saveFileDialog.FileName;
                string extension = Path.GetExtension(filePath).ToLower();

                try
                {
                    switch (extension)
                    {
                        case ".xlsx":
                            ExportToExcel(equipments, filePath);
                            break;

                        case ".pdf":
                            ExportToPdf(equipments, filePath);
                            break;

                        case ".csv":
                            ExportToCsv(equipments, filePath);
                            break;

                        default:
                            throw new NotSupportedException("Неподдерживаемый формат файла.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при экспорте: " + ex.Message);
                }
            }

            private static void ExportToExcel(System.Collections.Generic.List<Equipment> equipments, string filePath)
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Отчет");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Наименование";
                worksheet.Cell(1, 3).Value = "Серийный номер";
                worksheet.Cell(1, 4).Value = "Статус";
                worksheet.Cell(1, 5).Value = "Категория";
                worksheet.Cell(1, 6).Value = "Дата покупки";
                worksheet.Cell(1, 7).Value = "Местоположение";

                int row = 2;
                foreach (var eq in equipments)
                {
                    worksheet.Cell(row, 1).Value = eq.Id;
                    worksheet.Cell(row, 2).Value = eq.Name ?? "";
                    worksheet.Cell(row, 3).Value = eq.SerialNumber ?? "";
                    worksheet.Cell(row, 4).Value = eq.Status.ToString();
                    worksheet.Cell(row, 5).Value = eq.Category?.Name ?? "";
                    worksheet.Cell(row, 6).Value = eq.PurchaseDate.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 7).Value = eq.Location ?? "";
                    row++;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);
            }

            private static void ExportToPdf(System.Collections.Generic.List<Equipment> equipments, string filePath)
            {
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                using var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter.GetInstance(document, fs);
                document.Open();

                var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                var title = new Paragraph("Отчет по оборудованию", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                document.Add(title);

                var table = new PdfPTable(7) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 5f, 20f, 15f, 10f, 15f, 15f, 15f });

                var headerFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                string[] headers = { "ID", "Наименование", "Серийный номер", "Статус", "Категория", "Дата покупки", "Местоположение" };
                foreach (var header in headers)
                {
                    var cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                var dataFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                foreach (var eq in equipments)
                {
                    table.AddCell(new PdfPCell(new Phrase(eq.Id.ToString(), dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(eq.Name ?? "", dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(eq.SerialNumber ?? "", dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(eq.Status.ToString(), dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(eq.Category?.Name ?? "", dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(eq.PurchaseDate.ToString("dd.MM.yyyy"), dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(eq.Location ?? "", dataFont)) { Padding = 5 });
                }

                document.Add(table);
                document.Close();
            }

            private static void ExportToCsv(System.Collections.Generic.List<Equipment> equipments, string filePath)
            {
                var sb = new StringBuilder();
                sb.AppendLine("ID,Наименование,Серийный номер,Статус,Категория,Дата покупки,Местоположение");

                foreach (var eq in equipments)
                {
                    sb.AppendLine($"{eq.Id}," +
                                  $"\"{EscapeCsv(eq.Name)}\"," +
                                  $"\"{EscapeCsv(eq.SerialNumber)}\"," +
                                  $"{eq.Status}," +
                                  $"\"{EscapeCsv(eq.Category?.Name)}\"," +
                                  $"{eq.PurchaseDate:dd.MM.yyyy}," +
                                  $"\"{EscapeCsv(eq.Location)}\"");
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            }

            private static string EscapeCsv(string? s)
            {
                if (string.IsNullOrEmpty(s)) return "";
                return s.Replace("\"", "\"\"");
            }
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
