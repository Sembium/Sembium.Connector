using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Entities
{
    public class Product
    {
        public int ProductCode { get; set; }
        public string Name { get; set; }
        public long ProductNo { get; set; }
        public string PartnerProductNames { get; set; }
    }
}
