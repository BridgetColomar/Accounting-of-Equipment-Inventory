using AccountingOfEquipmentInventoryManagementApp.ViewModels;
using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
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
   
    public partial class OperatorWindow : Window
    {
        private void CustomTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        public OperatorWindow()
        {
            InitializeComponent();
            DataContext = new OperatorViewModel();
        }
    }
}
