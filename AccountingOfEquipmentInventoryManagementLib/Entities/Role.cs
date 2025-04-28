using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementLib.Entities
{
    /// <summary>
    /// Перечисление ролей (уровней доступа) для сотрудников.
    /// </summary>
    public enum Role
    {
        Operator,       // Базовый уровень доступа, например, для операторов
        Manager,        // Расширенный доступ для управления информацией
        Administrator   // Полный доступ ко всем функциям системы
    }
}
