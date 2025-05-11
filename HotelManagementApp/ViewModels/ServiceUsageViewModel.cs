using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApp.ViewModels
{
    public class ServiceUsageViewModel
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
        public bool Overdue { get; set; } = false;
        public bool TotalRow { get; set; } = false;
    }
}
