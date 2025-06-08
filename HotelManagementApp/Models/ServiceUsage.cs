using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementApp.Models
{
    public class ServiceUsage
    {
        [Key]
        public int UID { get; set; }

        public int SID { get; set; }

        public int ReID { get; set; } // ✅ đúng theo cột khóa ngoại trong DB

        public int Quantity { get; set; }

        [Column("TotalPerService")]
        public decimal ServiceTotal { get; set; } // ✅ đổi tên cho khớp DB hoặc map lại
    }
}
