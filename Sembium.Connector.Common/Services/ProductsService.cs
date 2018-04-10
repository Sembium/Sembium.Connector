using Sembium.Connector.Data;
using Sembium.Connector.Data.Connection;
using Sembium.Connector.Data.Sql;
using Sembium.Connector.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Services
{
    public class ProductsService : IProductsService
    {
        private const int PoductsUserActivityCode = 1201;

        private readonly IDataConnection _dataConnection;
        private readonly IAuthorization _authorization;

        public ProductsService(IDataConnection dataConnection, IAuthorization authorization)
        {
            _dataConnection = dataConnection;
            _authorization = authorization;
        }

        private Product ReadProduct(DbDataReader reader)
        {
            return new Product()
            {
                ProductCode = (int)reader.GetDouble(0),
                Name = reader.GetString(1),
                ProductNo = reader.GetInt64(2),
                PartnerProductNames = (reader.IsDBNull(3) ? null : reader.GetString(3))
            };
        }

        private ProductParam ReadProductParam(DbDataReader reader)
        {
            return new ProductParam()
            {
                ProductCode = (int)reader.GetDouble(0),
                ProductParamCode = (int)reader.GetDouble(1),
                OrderNo = (int)reader.GetDouble(2),
                Name = (string)reader.GetString(3),
                Abbrev = (string)(reader.IsDBNull(4) ? null : reader.GetString(4)),
                ValueType = (int)reader.GetDouble(5),
                NomCode = (reader.IsDBNull(6) ? null : (int?)reader.GetDouble(6)),
                ValueNomItemCode = (reader.IsDBNull(7) ? null : (int?)reader.GetDouble(7)),
                ValueFloat = (reader.IsDBNull(8) ? null : (double?)reader.GetDecimal(8)),
                ParamValue = (reader.IsDBNull(9) ? null : reader.GetString(9))
            };
        }

        private Product GetProduct(string sql, params SqlDataParameter[] parameters)
        {
            return _dataConnection.GetItem(ReadProduct, sql, parameters);
        }

        private IEnumerable<Product> GetProducts(string sql, params SqlDataParameter[] parameters)
        {
            return _dataConnection.GetItems(ReadProduct, sql, parameters);
        }

        private IEnumerable<ProductParam> GetProductParams(string sql, params SqlDataParameter[] parameters)
        {
            return _dataConnection.GetItems(ReadProductParam, sql, parameters);
        }

        public Product GetProductByCode(int productCode)
        {
            _authorization.CheckUserActivity(PoductsUserActivityCode);

            return GetProduct(
                "select p.PRODUCT_CODE, p.NAME, p.CUSTOM_CODE, ProductUtils.ProductCompanyProductNames(p.PRODUCT_CODE) as PRODUCT_PARTNER_NAMES from PRODUCTS p where (p.PRODUCT_CODE = :PRODUCT_CODE) and (p.CUSTOM_CODE is not null)",
                new SqlDataParameter("PRODUCT_CODE", productCode));
        }

        public Product GetProductByNo(long productNo)
        {
            _authorization.CheckUserActivity(PoductsUserActivityCode);

            return GetProduct(
                "select p.PRODUCT_CODE, p.NAME, p.CUSTOM_CODE, ProductUtils.ProductCompanyProductNames(p.PRODUCT_CODE) as PRODUCT_PARTNER_NAMES from PRODUCTS p where (p.CUSTOM_CODE = :CUSTOM_CODE)",
                new SqlDataParameter("CUSTOM_CODE", productNo));
        }

        public IEnumerable<Product> GetProductChildren(int productCode)
        {
            _authorization.CheckUserActivity(PoductsUserActivityCode);

            return GetProducts(
                "select p.PRODUCT_CODE, p.NAME, p.CUSTOM_CODE, ProductUtils.ProductCompanyProductNames(p.PRODUCT_CODE) as PRODUCT_PARTNER_NAMES from PRODUCTS p where (p.IS_INACTIVE = 0) and (p.IS_GROUP = 0) and (p.PARENT_CODE = :PRODUCT_CODE) order by p.CUSTOM_CODE",
                new SqlDataParameter("PRODUCT_CODE", productCode));
        }

        public IEnumerable<Product> GetNonEmptyLastGroups()
        {
            _authorization.CheckUserActivity(PoductsUserActivityCode);

            return GetProducts(
                "select" + Environment.NewLine +
                "  p.PRODUCT_CODE," + Environment.NewLine +
                "  p.NAME," + Environment.NewLine +
                "  p.CUSTOM_CODE," + Environment.NewLine +
                "  ProductUtils.ProductCompanyProductNames(p.PRODUCT_CODE) as PRODUCT_PARTNER_NAMES" + Environment.NewLine +
                "from" + Environment.NewLine +
                "  PRODUCTS p" + Environment.NewLine +
                "where" + Environment.NewLine +
                "  (p.IS_GROUP = 1) and" + Environment.NewLine +
                "  (p.IS_INACTIVE = 0) and" + Environment.NewLine +
                "  ( exists" + Environment.NewLine +
                "    ( select" + Environment.NewLine +
                "        1" + Environment.NewLine +
                "      from" + Environment.NewLine +
                "        PRODUCTS p2" + Environment.NewLine +
                "      where" + Environment.NewLine +
                "        (p2.IS_GROUP = 0) and" + Environment.NewLine +
                "        (p2.IS_INACTIVE = 0) and" + Environment.NewLine +
                "        (p2.PARENT_CODE = p.PRODUCT_CODE)" + Environment.NewLine +
                "    )" + Environment.NewLine +
                "  )" + Environment.NewLine +
                "order by" + Environment.NewLine +
                "  p.NAME");
        }

        private IEnumerable<ProductParam> InternalGetProductParams(int productCode, int productParamCode)
        {
            return GetProductParams(
                "select" + Environment.NewLine +
                "  pp.PRODUCT_CODE," + Environment.NewLine +
                "  pp.PRODUCT_PARAM_CODE," + Environment.NewLine +
                "  pp.PRODUCT_PARAM_ORDER_NO," + Environment.NewLine +
                "  pp.PRODUCT_PARAM_NAME," + Environment.NewLine +
                "  pp.ABBREV," + Environment.NewLine +
                "  pp.VALUE_TYPE," + Environment.NewLine +
                "  pp.NOM_CODE," + Environment.NewLine +
                "  pp.VALUE_NOM_ITEM_CODE," + Environment.NewLine +
                "  pp.VALUE_FLOAT," + Environment.NewLine +
                "  pp.PARAM_VALUE" + Environment.NewLine +
                "from" + Environment.NewLine +
                "  PRODUCT_PARAMS pp" + Environment.NewLine +
                "where" + Environment.NewLine +
                "  (pp.PRODUCT_CODE = :PRODUCT_CODE) and" + Environment.NewLine +
                "  ( (:PRODUCT_PARAM_CODE is null) or" + Environment.NewLine +
                "    (:PRODUCT_PARAM_CODE = 0) or" + Environment.NewLine +
                "    (pp.PRODUCT_PARAM_CODE = :PRODUCT_PARAM_CODE) )" + Environment.NewLine +
                "order by" + Environment.NewLine +
                "  pp.PRODUCT_CODE," + Environment.NewLine +
                "  pp.PRODUCT_PARAM_CODE",
                new SqlDataParameter("PRODUCT_CODE", productCode),
                new SqlDataParameter("PRODUCT_PARAM_CODE", productParamCode));
        }

        public ProductParam GetProductParam(int productCode, int productParamCode)
        {
            _authorization.CheckUserActivity(PoductsUserActivityCode);

            if (productParamCode <= 0)
            {
                return null;
            }

            return InternalGetProductParams(productCode, productParamCode).FirstOrDefault();
        }

        public IEnumerable<ProductParam> GetProductParams(int productCode)
        {
            _authorization.CheckUserActivity(PoductsUserActivityCode);

            return InternalGetProductParams(productCode, 0);
        }
    }
}
