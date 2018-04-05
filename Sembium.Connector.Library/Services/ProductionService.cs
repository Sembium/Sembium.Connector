using Sembium.Connector.Data;
using Sembium.Connector.Data.Connection;
using Sembium.Connector.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Services
{
    public class ProductionService : IProductionService
    {
        private const int ProductionOrdersCoveringSalesUserActivityCode = 1921;

        private readonly IDataConnection _dataConnection;
        private readonly IAuthorization _authorization;

        public ProductionService(IDataConnection dataConnection, IAuthorization authorization)
        {
            _dataConnection = dataConnection;
            _authorization = authorization;
        }

        public bool ProductionOrderExists(int saleBranchNo, int saleNo)
        {
            _authorization.CheckUserActivity(ProductionOrdersCoveringSalesUserActivityCode);

            return _dataConnection.GetItem(
                (r) => (r.GetInt32(0) != 0),
                "select Sign(Count(*)) from SALES s, DEPTS d where (s.SALE_BRANCH_CODE = d.DEPT_CODE) and (d.CUSTOM_CODE = :SALE_BRANCH_NO) and (s.SALE_NO = :SALE_NO)",
                new SqlDataParameter("SALE_BRANCH_NO", saleBranchNo),
                new SqlDataParameter("SALE_NO", saleNo));
        }
    }
}
