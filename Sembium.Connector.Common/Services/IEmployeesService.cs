using Sembium.Connector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Services
{
    public interface IEmployeesService
    {
        void AddEmployeeMovement(int employeeNo, int inOut, DateTime movementDateTime);
        void AddEmployeeMovements(IEnumerable<EmployeeMovement> employeeMovements);
    }
}
