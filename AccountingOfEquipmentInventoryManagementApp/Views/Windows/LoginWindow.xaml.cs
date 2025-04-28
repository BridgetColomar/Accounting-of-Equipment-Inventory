using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IEmployeeService _employeeService;
        private readonly IServiceProvider _serviceProvider;

        // Конструктор, получающий зависимости через DI
        public LoginWindow(IEmployeeService employeeService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _employeeService = employeeService;
            _serviceProvider = serviceProvider;
        }

        // Параметрless конструктор для поддержки XAML (если используется StartupUri)
        public LoginWindow() : this(
            ((App)Application.Current).AppHost.Services.GetRequiredService<IEmployeeService>(),
            ((App)Application.Current).AppHost.Services)
        { }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string password = PwdPassword.Password.Trim();

            // Аутентифицируем пользователя, данные загружаются напрямую из базы
            var employee = await _employeeService.AuthenticateEmployeeAsync(username, password);

            if (employee != null)
            {
                MessageBox.Show($"Добро пожаловать, {employee.FullName}!",
                    "Аутентификация успешна", MessageBoxButton.OK, MessageBoxImage.Information);

                // Выбираем окно в зависимости от роли, используя локальную переменную loginWindow
                Window loginWindow = null;
                switch (employee.AccessRole)
                {
                    case Role.Administrator:
                        loginWindow = _serviceProvider.GetRequiredService<AdministratorWindow>();
                        break;
                    case Role.Operator:
                        loginWindow = _serviceProvider.GetRequiredService<OperatorWindow>();
                        break;
                    case Role.Manager:
                        loginWindow = _serviceProvider.GetRequiredService<ManagerWindow>();
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль пользователя.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                // Открываем выбранное окно
                loginWindow.Show();

                // Закрываем окно авторизации (LoginWindow)
                this.Close();
            }
            else
            {
                // Если аутентификация не удалась, выводим сообщение об ошибке
                LblError.Content = "Неверное имя пользователя или пароль.";
            }
        }
    }
}
