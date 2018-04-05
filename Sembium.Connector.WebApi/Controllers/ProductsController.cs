using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sembium.Connector.Services;
using Microsoft.AspNetCore.Authorization;
using Sembium.Connector.Entities;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Sembium.Connector.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        // GET api/products/GetProductByCode?productCode=123456
        [Route("GetProductByCode")]
        [HttpGet]
        public Product GetProductByCode([FromQuery] int productCode)
        {
            return _productsService.GetProductByCode(productCode);
        }

        // GET api/products/GetProductByNo?productNo=9988776655
        [Route("GetProductByNo")]
        [HttpGet]
        public Product GetProductByNo([FromQuery] long productNo)
        {
            return _productsService.GetProductByNo(productNo);
        }

        // GET api/products/GetProductChildren?productCode=123456
        [Route("GetProductChildren")]
        [HttpGet]
        public IEnumerable<Product> GetProductChildren([FromQuery] int productCode)
        {
            return _productsService.GetProductChildren(productCode);
        }

        // GET api/products/GetNonEmptyLastGroups
        [Route("GetNonEmptyLastGroups")]
        [HttpGet]
        public IEnumerable<Product> GetNonEmptyLastGroups()
        {
            return _productsService.GetNonEmptyLastGroups();
        }

        // GET api/products/GetProductParam?productCode=123456&productParamCode=12
        [Route("GetProductParam")]
        [HttpGet]
        public ProductParam GetProductParam([FromQuery] int productCode, [FromQuery] int productParamCode)
        {
            return _productsService.GetProductParam(productCode, productParamCode);
        }

        // GET api/products/GetProductParams?productCode=123456
        [Route("GetProductParams")]
        [HttpGet]
        public IEnumerable<ProductParam> GetProductParams([FromQuery] int productCode)
        {
            return _productsService.GetProductParams(productCode);
        }
    }
}
