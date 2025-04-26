using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApp.Models
{
    public class ServiceUsage
    {
        [Key] 
        public int UID { get; set; }
        public int SID { get; set; }
        public int CID { get; set; }
        public int IID { get; set; }
        public int Quantity { get; set; }
        public decimal ServiceTotal { get; set; }
    }
}
