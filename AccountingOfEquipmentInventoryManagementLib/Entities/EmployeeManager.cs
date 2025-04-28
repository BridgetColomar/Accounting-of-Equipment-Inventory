using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementLib.Entities
{
    /// <summary>
    /// Класс для управления сотрудниками, включая верификацию и аутентификацию.
    /// </summary>
    public class EmployeeManager
    {
        private readonly List<Employee> _employees;

        public EmployeeManager()
        {
            _employees = new List<Employee>();
        }

        /// <summary>
        /// Добавление нового сотрудника в систему.
        /// </summary>
        /// <param name="employee">Экземпляр Employee для добавления.</param>
        public void AddEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            _employees.Add(employee);
        }

        /// <summary>
        /// Метод аутентификации сотрудника по логину и паролю.
        /// Реализована простая проверка. Для реальной системы следует использовать криптографические методы хэширования.
        /// </summary>
        /// <param name="username">Имя пользователя (логин) сотрудника.</param>
        /// <param name="password">Введённый пароль для проверки.</param>
        /// <returns>Если аутентификация проходит успешно, возвращается объект Employee; иначе – null.</returns>
        public Employee AuthenticateEmployee(string username, string password)
        {
            // Для демонстрационных целей используется простая проверка,
            // предполагая, что входной пароль уже является хэшем.
            foreach (var emp in _employees)
            {
                if (emp.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase) &&
                    VerifyPassword(password, emp.PasswordHash))
                {
                    emp.LastLoginTime = DateTime.Now;
                    return emp;
                }
            }
            return null;
        }

        /// <summary>
        /// Пример метода проверки пароля.
        /// Здесь необходимо заменить простую проверку на надежный алгоритм (например, bcrypt или PBKDF2) в реальном применении.
        /// </summary>
        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            // Пример: сравнение входного значения с хранилищем.
            return inputPassword == storedHash;
        }

        /// <summary>
        /// Получение списка сотрудников.
        /// </summary>
        /// <returns>Перечисление зарегистрированных сотрудников.</returns>
        public IEnumerable<Employee> GetEmployees()
        {
            return _employees;
        }
    }
}
