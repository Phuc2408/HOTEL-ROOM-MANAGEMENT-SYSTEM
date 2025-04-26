using System.ComponentModel.DataAnnotations;

namespace HotelManagementApp.Models
{
    public class Service
    {
        [Key]
        public int SID { get; set; }
        public string SName { get; set; }
        public string SUnit { get; set; }
        public decimal SPrice { get; set; }
    }
}
