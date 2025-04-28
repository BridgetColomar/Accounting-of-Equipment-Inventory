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

namespace AccountingOfEquipmentInventoryManagementApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        // Создаём контекст базы данных вручную с указанной строкой подключения
        private readonly SqliteDbContext _dbContext;

        public ManagerWindow()
        {
            InitializeComponent();

            // Создаём DbContextOptions для SqliteDbContext
            var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
            optionsBuilder.UseSqlite("Data Source=EquipmentInventory.db");

            _dbContext = new SqliteDbContext(optionsBuilder.Options);

            // Загружаем список сотрудников при запуске окна
            LoadEmployees();
        }

        /// <summary>
        /// Асинхронно загружает сотрудников из базы и отображает их в DataGrid.
        /// </summary>
        private async void LoadEmployees()
        {
            try
            {
                List<Employee> employees = await _dbContext.Employees.ToListAsync();
                dgEmployees.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновляет список сотрудников.
        /// </summary>
        private void BtnRefreshEmployees_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }

        /// <summary>
        /// Генерирует отчёт по сотрудникам и выводит его в текстовом поле.
        /// </summary>
        private async void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Employee> employees = await _dbContext.Employees.ToListAsync();
                int totalEmployees = employees.Count;
                int activeEmployees = employees.Count(emp => emp.IsActive);
                int inactiveEmployees = totalEmployees - activeEmployees;
                int admins = employees.Count(emp => emp.AccessRole == Role.Administrator);
                int managers = employees.Count(emp => emp.AccessRole == Role.Manager);
                int operators = employees.Count(emp => emp.AccessRole == Role.Operator);

                txtReport.Text = $"Отчёт по сотрудникам:\n" +
                                 $"Общее количество сотрудников: {totalEmployees}\n" +
                                 $"Активных: {activeEmployees} | Неактивных: {inactiveEmployees}\n\n" +
                                 $"По ролям:\n" +
                                 $"Администраторы: {admins}\n" +
                                 $"Менеджеры: {managers}\n" +
                                 $"Операторы: {operators}\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации отчёта: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}