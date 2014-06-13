using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.models
{
    public class LineItem
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductSKU { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TaxRate { get; set; }
        public int TaxAmount { get; set; }
        public string CommodityCode { get; set; }
    }
}
