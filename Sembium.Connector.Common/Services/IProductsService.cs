using Sembium.Connector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Services
{
    public interface IProductsService
    {
        Product GetProductByCode(int productCode);
        Product GetProductByNo(long productNo);
        IEnumerable<Product> GetProductChildren(int productCode);
        IEnumerable<Product> GetNonEmptyLastGroups();
        ProductParam GetProductParam(int productCode, int productParamCode);
        IEnumerable<ProductParam> GetProductParams(int productCode);
    }
}
