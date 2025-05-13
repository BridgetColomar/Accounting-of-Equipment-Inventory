using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using System.Security.Cryptography;

namespace AccountingOfEquipmentInventoryManagementApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdministratorWindow.xaml
    /// </summary>
    public partial class AdministratorWindow : Window
    {
        public AdministratorWindow()
        {
            InitializeComponent();
            // Заполнение ComboBox для ролей (предполагается, что Role – enum)
            cbAccessRole.ItemsSource = Enum.GetValues(typeof(Role)).Cast<Role>();
            // Установка значения по умолчанию, например, Администратор
            cbAccessRole.SelectedItem = Role.Administrator;

            _ = LoadEmployeesAsync();
        }

        // Загрузка сотрудников из БД с необязательным поисковым выражением
        private async Task LoadEmployeesAsync(string searchTerm = null)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    IQueryable<Employee> query = context.Set<Employee>();

                    if (!string.IsNullOrEmpty(searchTerm))
                        query = query.Where(e => EF.Functions.Like(e.FullName, $"%{searchTerm}%") ||
                                                 EF.Functions.Like(e.Username, $"%{searchTerm}%"));

                    var employees = await query.ToListAsync();
                    EmployeesDataGrid.ItemsSource = employees;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки сотрудников: " + ex.Message);
                MessageBox.Show("Ошибка загрузки сотрудников: " + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Добавление нового сотрудника
        private async void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fullName = tbFullName.Text.Trim();
                string username = tbUsername.Text.Trim();
                string password = pbPassword.Password;
                Role accessRole = (Role)cbAccessRole.SelectedItem;

                if (string.IsNullOrEmpty(fullName) ||
                    string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Заполните все обязательные поля (ФИО, имя пользователя, пароль).",
                                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Вычисляем хэш пароля (например, SHA256)
                string passwordHash = ComputeSha256Hash(password);

                var newEmployee = new Employee
                {
                    FullName = fullName,
                    Username = username,
                    PasswordHash = passwordHash,
                    AccessRole = accessRole,
                    IsActive = true,
                    LastLoginTime = null
                };

                var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                using (var context = new SqliteDbContext(optionsBuilder.Options))
                {
                    context.Set<Employee>().Add(newEmployee);
                    await context.SaveChangesAsync();
                }
                MessageBox.Show("Сотрудник успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                // Очистка полей ввода
                tbFullName.Clear();
                tbUsername.Clear();
                pbPassword.Clear();
                cbAccessRole.SelectedItem = Role.Administrator;
                await LoadEmployeesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка добавления сотрудника: " + ex.Message);
                MessageBox.Show("Ошибка добавления сотрудника: " + ex.Message, "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Редактирование выбранного сотрудника
        private async void btnEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is not Employee selectedEmployee)
            {
                MessageBox.Show("Выберите сотрудника для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Заполнение полей текущими данными выбранного сотрудника
            tbFullName.Text = selectedEmployee.FullName;
            tbUsername.Text = selectedEmployee.Username;
            // Пароль не заполняется по соображениям безопасности – его можно изменить отдельно.
            cbAccessRole.SelectedItem = selectedEmployee.AccessRole;

            // Для обновления используем диалог подтверждения
            if (MessageBox.Show("Сохранить изменения для выбранного сотрудника?", "Подтверждение",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    selectedEmployee.FullName = tbFullName.Text.Trim();
                    selectedEmployee.Username = tbUsername.Text.Trim();
                    // Если администратор ввёл новый пароль, обновляем хэш; иначе – оставляем существующий.
                    if (!string.IsNullOrEmpty(pbPassword.Password))
                        selectedEmployee.PasswordHash = ComputeSha256Hash(pbPassword.Password);
                    selectedEmployee.AccessRole = (Role)cbAccessRole.SelectedItem;

                    var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                    using (var context = new SqliteDbContext(optionsBuilder.Options))
                    {
                        context.Set<Employee>().Update(selectedEmployee);
                        await context.SaveChangesAsync();
                    }
                    MessageBox.Show("Сотрудник успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadEmployeesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка редактирования сотрудника: " + ex.Message);
                    MessageBox.Show("Ошибка редактирования сотрудника: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обработчик удаления выбранных сотрудников
        private async void btnDeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEmployees = EmployeesDataGrid.SelectedItems.Cast<Employee>().ToList();
                if (!selectedEmployees.Any())
                {
                    MessageBox.Show("Выберите одного или нескольких сотрудников для удаления.", "Внимание",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (MessageBox.Show($"Удалить выбранных сотрудников ({selectedEmployees.Count})?",
                                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
                    using (var context = new SqliteDbContext(optionsBuilder.Options))
                    {
                        context.Set<Employee>().RemoveRange(selectedEmployees);
                        await context.SaveChangesAsync();
                    }
                    MessageBox.Show("Сотрудники удалены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadEmployeesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка удаления сотрудников: " + ex.Message);
                MessageBox.Show("Ошибка удаления сотрудников: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Поиск сотрудников по ФИО или имени пользователя
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = tbSearch.Text.Trim();
            await LoadEmployeesAsync(searchTerm);
        }

        // Экспорт списка сотрудников в CSV
        private void btnExportCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var employees = EmployeesDataGrid.ItemsSource as IEnumerable<Employee>;
                if (employees == null || !employees.Any())
                {
                    MessageBox.Show("Нет данных для экспорта.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var sfd = new SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                    FileName = "EmployeesReport.csv"
                };

                if (sfd.ShowDialog() == true)
                {
                    using (var sw = new StreamWriter(sfd.FileName))
                    {
                        sw.WriteLine("Id,FullName,Username,AccessRole,IsActive,LastLoginTime");
                        foreach (var emp in employees)
                        {
                            // Если LastLoginTime равен null, можно экспортировать пустую строку
                            string lastLogin = emp.LastLoginTime.HasValue ? emp.LastLoginTime.Value.ToString("yyyy-MM-dd HH:mm") : "";
                            sw.WriteLine($"{emp.Id},{EscapeCsv(emp.FullName)},{EscapeCsv(emp.Username)},{emp.AccessRole},{emp.IsActive},{lastLogin}");
                        }
                    }
                    MessageBox.Show("Экспорт завершен успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка экспорта CSV: " + ex.Message);
                MessageBox.Show("Ошибка экспорта CSV: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик двойного клика по DataGrid для редактирования
        private void EmployeesDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnEditEmployee_Click(sender, e);
        }

        // Метод для экранирования спецсимволов при экспорте в CSV
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }

        // Метод для вычисления SHA256-хэша строки (например, для пароля)
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Вычисляем хэш-байты
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                // Преобразуем байты в строку
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}