using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AccountingOfEquipmentInventoryManagementApp.ViewModels;
namespace AccountingOfEquipmentInventoryManagementApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdministratorWindow.xaml
    /// </summary>
    public partial class AdministratorWindow : Window
    {
        private void CustomTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        public AdministratorWindow()
        {
            InitializeComponent();
            DataContext = new AdministratorViewModel();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdministratorViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }

}