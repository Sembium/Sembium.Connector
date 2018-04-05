using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sembium.Connector.Services;
using Microsoft.AspNetCore.Authorization;

namespace Sembium.Connector.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProductionController : Controller
    {
        private readonly IProductionService _productionService;

        public ProductionController(IProductionService productionService)
        {
            _productionService = productionService;
        }

        // GET api/production/ProductionOrderExists?saleBranchNo=2&saleNo=20158
        [Route("ProductionOrderExists")]
        [HttpGet]
        public bool ProductionOrderExists([FromQuery] int saleBranchNo, [FromQuery] int saleNo)
        {
            return _productionService.ProductionOrderExists(saleBranchNo, saleNo);
        }
    }
}
