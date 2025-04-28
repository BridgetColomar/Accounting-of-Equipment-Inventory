using AccountingOfEquipmentInventoryManagementLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Services.Abstraction
{

    /// <summary>
    /// Интерфейс для работы с данными сотрудников.
    /// </summary>
    public interface IEmployeeService
    {
        Task<Employee> AuthenticateEmployeeAsync(string username, string password);
        Task AddEmployeeAsync(Employee employee);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id); // Этот метод требует реализации
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int employeeId);
    }
}
