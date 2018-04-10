using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Entities
{
    public class EmployeeMovement
    {
        public int EmployeeNo { get; set; }
        public int InOut { get; set; }
        public DateTime MovementDateTime { get; set; }
    }
}
