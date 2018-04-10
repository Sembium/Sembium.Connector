using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Entities
{
    public class ProductParam
    {
        public int ProductCode { get; set; }
        public int ProductParamCode { get; set; }
        public int OrderNo { get; set; }
        public string Name { get; set; }
        public string Abbrev { get; set; }
        public int ValueType { get; set; }
        public Nullable<int> NomCode { get; set; }
        public Nullable<int> ValueNomItemCode { get; set; }
        public Nullable<Double> ValueFloat { get; set; }
        public string ParamValue { get; set; }
    }
}
