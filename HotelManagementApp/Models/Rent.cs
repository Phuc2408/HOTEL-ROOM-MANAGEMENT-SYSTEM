using HotelManagementApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Rent")]
public class Rent
{
    [Key]
    public int ReID { get; set; } // ✅ Đúng với tên cột trong SQL

    public int RID { get; set; }

    public int CID { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public TimeSpan CheckInTime { get; set; }

    public TimeSpan CheckOutTime { get; set; }

    public int NumberOfPeople { get; set; }

    public bool isDone { get; set; }

    // 🔁 Navigation property nếu bạn muốn liên kết với Room và Customer
    [ForeignKey("RID")]
    public virtual Room Room { get; set; }

    [ForeignKey("CID")]
    public virtual Customer Customer { get; set; }
}
