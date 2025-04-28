using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementLib.Entities
{
    /// <summary>
    /// Сущность "Сотрудник" для системы верификации и разграничения доступа.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Уникальный идентификатор сотрудника.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Полное имя сотрудника.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Имя пользователя (логин) для входа в систему.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Хэш пароля для безопасного хранения учетных данных.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Роль сотрудника, определяющая уровень доступа к информации.
        /// </summary>
        public Role AccessRole { get; set; }

        /// <summary>
        /// Флаг, указывающий, активен ли аккаунт сотрудника.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Дата и время последней успешной авторизации.
        /// </summary>
        public DateTime? LastLoginTime { get; set; }
    }
}
