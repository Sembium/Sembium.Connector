using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Services
{
    public interface IProductionService
    {
        bool ProductionOrderExists(int saleBranchNo, int saleNo);
    }
}
