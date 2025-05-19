using AccountingOfEquipmentInventoryManagementApp.Helpers;
using AccountingOfEquipmentInventoryManagementApp.Views.Windows;
using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace AccountingOfEquipmentInventoryManagementApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IEmployeeService _employeeService;
        private readonly IServiceProvider _serviceProvider;

        // Основной конструктор с зависимостями через DI
        public LoginViewModel(IEmployeeService employeeService, IServiceProvider serviceProvider)
        {
            _employeeService = employeeService;
            _serviceProvider = serviceProvider;

            LoginCommand = new RelayCommand(async o => await ExecuteLoginAsync());
        }

        // Параметрless конструктор для поддержки XAML (например, при использовании StartupUri)
        public LoginViewModel() : this(
            ((App)Application.Current).AppHost.Services.GetRequiredService<IEmployeeService>(),
            ((App)Application.Current).AppHost.Services)
        { }

        // Свойство для ввода имени пользователя
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        // Свойство для ввода пароля
        // Обратите внимание: стандартный PasswordBox не поддерживает двухстороннюю привязку Password.
        // Для полноценной работы можно использовать прикреплённое поведение или передавать значение вручную из code‑behind.
        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        // Свойство для вывода сообщения об ошибке
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        // Команда для входа
        public ICommand LoginCommand { get; }

        // Делегат, через который окно (View) может закрыться по запросу ViewModel
        public Action RequestClose { get; set; }

        // Метод, выполняющий логику входа
        public async Task ExecuteLoginAsync()
        {
            string username = Username?.Trim();
            string password = Password?.Trim();

            // Аутентификация сотрудника через сервис
            var employee = await _employeeService.AuthenticateEmployeeAsync(username, password);
            if (employee != null)
            {
                MessageBox.Show($"Добро пожаловать, {employee.FullName}!",
                    "Аутентификация успешна", MessageBoxButton.OK, MessageBoxImage.Information);

                // Выбираем окно для открытия в зависимости от роли сотрудника
                Window nextWindow = null;
                switch (employee.AccessRole)
                {
                    case Role.Operator:
                        nextWindow = _serviceProvider.GetRequiredService<OperatorWindow>();
                        break;
                    case Role.Manager:
                        nextWindow = _serviceProvider.GetRequiredService<ManagerWindow>();
                        break;
                    case Role.Administrator:
                        nextWindow = _serviceProvider.GetRequiredService<AdministratorWindow>();
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль пользователя.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }
                nextWindow.Show();

                // Запрашиваем закрытие окна аутентификации
                RequestClose?.Invoke();
            }
            else
            {
                // Если аутентификация не удалась — выводим сообщение об ошибке
                ErrorMessage = "Неверное имя пользователя или пароль.";
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
