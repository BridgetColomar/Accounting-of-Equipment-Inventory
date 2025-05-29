using DocumentFormat.OpenXml.Office2010.CustomUI;
using System;
using System.Collections.Generic;
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

namespace AccountingOfEquipmentInventoryManagementApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        // Конструктор принимает BitmapImage для показа.
        public ImageWindow(BitmapImage image)
        {
            InitializeComponent();
            ImageControl.Source = image;
        }

        // Можно также добавить конструктор, принимающий путь к изображению или byte[],
        // если это требуется по логике приложения.
        public ImageWindow(byte[] imageData)
        {
            InitializeComponent();
            if (imageData != null && imageData.Length > 0)
            {
                using (var ms = new MemoryStream(imageData))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    ImageControl.Source = bitmap;
                }
            }
        }
    }
}
