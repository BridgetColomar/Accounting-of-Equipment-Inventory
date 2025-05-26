using AccountingOfEquipmentInventoryManagementDbContext.Context;
using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementApp.ViewModels
{
    public class EquipmentCardsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Equipment> EquipmentList { get; set; }

        public EquipmentCardsViewModel()
        {
            EquipmentList = new ObservableCollection<Equipment>();

            using (var db = new SqliteDbContextFactory().CreateDbContext())
            {
                var equipmentFromDb = db.Equipments.Include(e => e.Category).ToList(); 
                foreach (var item in equipmentFromDb)
                {
                    EquipmentList.Add(item);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
