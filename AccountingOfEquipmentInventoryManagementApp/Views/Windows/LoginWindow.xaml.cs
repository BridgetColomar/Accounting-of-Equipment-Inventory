using AccountingOfEquipmentInventoryManagementApp.ViewModels;
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
        public LoginWindow()
        {
            InitializeComponent();

            // Если окно создаётся через DI, то DataContext будет задан из DI-контейнера
            var vm = new LoginViewModel();
            vm.RequestClose = () => this.Close();
            DataContext = vm;
        }
    }
}
