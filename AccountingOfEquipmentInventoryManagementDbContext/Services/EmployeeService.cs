using AccountingOfEquipmentInventoryManagementDbContext.Context;
using AccountingOfEquipmentInventoryManagementDbContext.Context.Connections;
using AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction;
using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly SqliteDbContext _context;

        public EmployeeService(SqliteDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Аутентифицирует сотрудника, извлекая данные напрямую из базы данных.
        /// Если аутентификация успешна, обновляет дату последнего входа.
        /// </summary>
        public async Task<Employee> AuthenticateEmployeeAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Логин или пароль не могут быть пустыми.");
                return null;
            }

            // Приводим логин к нижнему регистру для неявного сравнения без учета регистра
            string normalizedUsername = username.Trim().ToLower();

            // Запрашиваем сотрудника из базы данных напрямую
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username.ToLower() == normalizedUsername);

            if (employee == null)
            {
                Console.WriteLine($"Сотрудник с логином '{username}' не найден в базе.");
                return null;
            }

            // Прямое сравнение "пароля" для демонстрации. В реальном приложении необходимо сравнивать хэшированный пароль.
            if (employee.PasswordHash != password)
            {
                Console.WriteLine("Неверный пароль.");
                return null;
            }

            // Успешная аутентификация — обновляем дату последнего входа
            employee.LastLoginTime = DateTime.Now;
            await _context.SaveChangesAsync();

            Console.WriteLine($"Аутентификация прошла успешно для пользователя '{username}'.");
            return employee;
        }

        // --- Остальные методы IEmployeeService, например:

        public async Task AddEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                throw new InvalidOperationException("Сотрудник не найден.");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}