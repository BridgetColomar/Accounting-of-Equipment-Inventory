using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using AccountingOfEquipmentInventoryManagementLib.Entities;

namespace AccountingOfEquipmentInventoryManagementApp.ViewModels
{
    public class EquipmentViewModel
    {
        public Equipment Model { get; }

        public EquipmentViewModel(Equipment equipment)
        {
            Model = equipment;
        }

        public string Name => Model.Name;
        public string SerialNumber => Model.SerialNumber;
        public string Status => Model.Status.ToString();
        public string CategoryName => Model.Category?.Name;
        public DateTime PurchaseDate => Model.PurchaseDate;
        public string Location => Model.Location;

        public BitmapImage ImageSource
        {
            get
            {
                if (Model.Image == null || Model.Image.Length == 0)
                    return null;

                using (var ms = new MemoryStream(Model.Image))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
        }
    }
}
