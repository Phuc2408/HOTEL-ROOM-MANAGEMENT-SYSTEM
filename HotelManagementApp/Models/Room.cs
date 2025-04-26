using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementApp.Models
{
    [Table("Room")]
    public class Room
    {
        [Key]
        public int RID { get; set; }

        public string RType { get; set; }

        public string RStatus { get; set; }

        public decimal RPrice { get; set; }

        public int RFloor { get; set; }
    }
}
