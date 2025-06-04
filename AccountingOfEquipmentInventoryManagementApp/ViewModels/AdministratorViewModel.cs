using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AccountingOfEquipmentInventoryManagementApp.ViewModels
{
    public class AdministratorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Role> Roles { get; } = new ObservableCollection<Role>(Enum.GetValues(typeof(Role)).Cast<Role>());
        private ObservableCollection<Employee> _employees = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set { _employees = value; OnPropertyChanged(nameof(Employees)); }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));

                if (_selectedEmployee != null)
                {
                    FullName = _selectedEmployee.FullName;
                    Username = _selectedEmployee.Username;
                    SelectedRole = _selectedEmployee.AccessRole;
                    Password = string.Empty; // Не раскрываем пароль
                }
            }
        }

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(nameof(FullName)); }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        private Role _selectedRole = Role.Administrator;
        public Role SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(nameof(SelectedRole)); }
        }


        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set { _searchTerm = value; OnPropertyChanged(nameof(SearchTerm)); }
        }

        public ICommand LoadEmployeesCommand => new RelayCommand(async _ => await LoadEmployeesAsync(SearchTerm));
        public ICommand AddEmployeeCommand => new RelayCommand(async _ => await AddEmployeeAsync());
        public ICommand EditEmployeeCommand => new RelayCommand(async _ => await EditEmployeeAsync());
        public ICommand DeleteEmployeesCommand => new RelayCommand(async _ => await DeleteEmployeesAsync());
        public ICommand ExportCommand => new RelayCommand(_ => Export());
        public ICommand EmployeeDoubleClickCommand => new RelayCommand(async _ => await EditEmployeeAsync());

        public AdministratorViewModel()
        {
            _ = LoadEmployeesAsync();
        }

        private DbContextOptions<SqliteDbContext> CreateOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
            optionsBuilder.UseSqlite("Data Source=yourdatabase.db"); // Укажите свой путь к БД
            return optionsBuilder.Options;
        }

        public async Task LoadEmployeesAsync(string search = null)
        {
            try
            {
                using var context = new SqliteDbContext(CreateOptions());
                var query = context.Set<Employee>().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(e => EF.Functions.Like(e.FullName, $"%{search}%") ||
                                             EF.Functions.Like(e.Username, $"%{search}%"));

                var result = await query.ToListAsync();

                Employees.Clear();
                foreach (var emp in result)
                    Employees.Add(emp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки сотрудников: " + ex.Message);
                MessageBox.Show("Ошибка загрузки сотрудников: " + ex.Message);
            }
        }

        public async Task AddEmployeeAsync()
        {
            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            try
            {
                string hash = ComputeSha256Hash(Password);

                var employee = new Employee
                {
                    FullName = FullName,
                    Username = Username,
                    PasswordHash = hash,
                    AccessRole = SelectedRole,
                    IsActive = true
                };

                using var context = new SqliteDbContext(CreateOptions());
                context.Set<Employee>().Add(employee);
                await context.SaveChangesAsync();

                MessageBox.Show("Сотрудник добавлен");
                await LoadEmployeesAsync();

                FullName = Username = Password = string.Empty;
                SelectedRole = Role.Administrator;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка добавления: " + ex.Message);
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        public async Task EditEmployeeAsync()
        {
            if (SelectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника");
                return;
            }

            if (MessageBox.Show("Сохранить изменения?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedEmployee.FullName = FullName;
                    SelectedEmployee.Username = Username;
                    SelectedEmployee.AccessRole = SelectedRole;
                    if (!string.IsNullOrWhiteSpace(Password))
                        SelectedEmployee.PasswordHash = ComputeSha256Hash(Password);

                    using var context = new SqliteDbContext(CreateOptions());
                    context.Set<Employee>().Update(SelectedEmployee);
                    await context.SaveChangesAsync();

                    MessageBox.Show("Сотрудник обновлен");
                    await LoadEmployeesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка обновления: " + ex.Message);
                    MessageBox.Show("Ошибка обновления: " + ex.Message);
                }
            }
        }

        public async Task DeleteEmployeesAsync()
        {
            if (SelectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника");
                return;
            }

            if (MessageBox.Show("Удалить сотрудника?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using var context = new SqliteDbContext(CreateOptions());
                    context.Set<Employee>().Remove(SelectedEmployee);
                    await context.SaveChangesAsync();

                    MessageBox.Show("Удален");
                    await LoadEmployeesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка удаления: " + ex.Message);
                    MessageBox.Show("Ошибка удаления: " + ex.Message);
                }
            }
        }

        public void Export()
        {
            try
            {
                if (!Employees.Any())
                {
                    MessageBox.Show("Нет данных");
                    return;
                }

                var sfd = new SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv|TXT файлы (*.txt)|*.txt|Excel файлы (*.xlsx)|*.xlsx|PDF файлы (*.pdf)|*.pdf",
                    FileName = "EmployeesReport"
                };

                if (sfd.ShowDialog() == true)
                {
                    string ext = Path.GetExtension(sfd.FileName).ToLower();

                    if (ext == ".csv")
                    {
                        using var sw = new StreamWriter(sfd.FileName);
                        sw.WriteLine("Id,FullName,Username,AccessRole,IsActive,LastLoginTime");
                        foreach (var e in Employees)
                            sw.WriteLine($"{e.Id},{EscapeCsv(e.FullName)},{EscapeCsv(e.Username)},{e.AccessRole},{e.IsActive},{e.LastLoginTime:dd.MM.yyyy HH:mm}");
                    }
                    else if (ext == ".txt")
                    {
                        using var sw = new StreamWriter(sfd.FileName);
                        sw.WriteLine("Id\tFullName\tUsername\tAccessRole\tIsActive\tLastLoginTime");
                        foreach (var e in Employees)
                            sw.WriteLine($"{e.Id}\t{e.FullName}\t{e.Username}\t{e.AccessRole}\t{e.IsActive}\t{e.LastLoginTime:dd.MM.yyyy HH:mm}");
                    }
                    else if (ext == ".xlsx")
                    {
                        var wb = new XLWorkbook();
                        var ws = wb.Worksheets.Add("EmployeesReport");
                        ws.Cell(1, 1).Value = "Id";
                        ws.Cell(1, 2).Value = "FullName";
                        ws.Cell(1, 3).Value = "Username";
                        ws.Cell(1, 4).Value = "AccessRole";
                        ws.Cell(1, 5).Value = "IsActive";
                        ws.Cell(1, 6).Value = "LastLoginTime";
                        int row = 2;
                        foreach (var e in Employees)
                        {
                            ws.Cell(row, 1).Value = e.Id;
                            ws.Cell(row, 2).Value = e.FullName;
                            ws.Cell(row, 3).Value = e.Username;
                            ws.Cell(row, 4).Value = e.AccessRole.ToString();
                            ws.Cell(row, 5).Value = e.IsActive;
                            ws.Cell(row, 6).Value = e.LastLoginTime?.ToString("dd.MM.yyyy HH:mm");
                            row++;
                        }
                        wb.SaveAs(sfd.FileName);
                    }
                    else if (ext == ".pdf")
                    {
                        var doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                        doc.Open();
                        var table = new PdfPTable(6);
                        table.AddCell("Id");
                        table.AddCell("FullName");
                        table.AddCell("Username");
                        table.AddCell("AccessRole");
                        table.AddCell("IsActive");
                        table.AddCell("LastLoginTime");
                        foreach (var e in Employees)
                        {
                            table.AddCell(e.Id.ToString());
                            table.AddCell(e.FullName);
                            table.AddCell(e.Username);
                            table.AddCell(e.AccessRole.ToString());
                            table.AddCell(e.IsActive.ToString());
                            table.AddCell(e.LastLoginTime?.ToString("dd.MM.yyyy HH:mm") ?? "");
                        }
                        doc.Add(table);
                        doc.Close();
                    }

                    MessageBox.Show("Экспорт завершен");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка экспорта: " + ex.Message);
                MessageBox.Show("Ошибка экспорта: " + ex.Message);
            }
        }

        private string EscapeCsv(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            if (val.Contains(',') || val.Contains('"') || val.Contains('\n'))
                return $"\"{val.Replace("\"", "\"\"")}\"";
            return val;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
