using AccountingOfEquipmentInventoryManagementApp.Views.Windows;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementDbContext.Services;
using AccountingOfEquipmentInventoryManagementDbContext.Context;
using AccountingOfEquipmentInventoryManagementLib.Entities;


namespace AccountingOfEquipmentInventoryManagementApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public IHost AppHost { get;  set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppHost = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
           {
               // Регистрация контекста базы данных
               services.AddDbContext<AppDbContext, SqliteDbContext>(options =>
                   options.UseSqlite("Data Source=EquipmentInventory.db"));

               // Регистрация сервисов
               services.AddTransient<IEmployeeService, EmployeeService>();
               services.AddTransient<IEquipmentService, EquipmentService>();  // Если требуется
               services.AddTransient<IInventoryService, InventoryService>();  // Регистрация IInventoryService

               // Регистрация окон приложения
               services.AddTransient<LoginWindow>();
               services.AddTransient<AdministratorWindow>();
               services.AddTransient<ManagerWindow>();
               services.AddTransient<OperatorWindow>();
           }).Build();

            await AppHost.StartAsync();

            // Загрузка окна авторизации
            var loginWindow = AppHost.Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            AppHost.Dispose();
            base.OnExit(e);
        }
    }
}

//23.04 не работает Менеджер 