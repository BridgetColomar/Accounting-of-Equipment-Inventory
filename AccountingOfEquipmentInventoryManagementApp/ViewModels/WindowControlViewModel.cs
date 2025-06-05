using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace AccountingOfEquipmentInventoryManagementApp.ViewModels
{
    public class WindowControlViewModel
    {
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }

        public WindowControlViewModel()
        {
            MinimizeCommand = new RelayCommand(w =>
            {
                var window = w as Window;
                if (window != null)
                    window.WindowState = WindowState.Minimized;
            });

            MaximizeCommand = new RelayCommand(w =>
            {
                var window = w as Window;
                if (window != null)
                {
                    window.WindowState = window.WindowState == WindowState.Maximized
                        ? WindowState.Normal
                        : WindowState.Maximized;
                }
            });

            CloseCommand = new RelayCommand(w =>
            {
                var window = w as Window;
                if (window != null)
                    window.Close();
            });

           
        }
        
    }
}
