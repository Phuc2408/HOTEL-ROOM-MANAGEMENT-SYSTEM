using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementApp.Models
{
    [Table("Rent")]
    public class Rent
    {
        [Key]
        public int RelID { get; set; }

        public int RID { get; set; }

        public int CID { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public TimeSpan CheckInTime { get; set; }

        public TimeSpan CheckOutTime { get; set; }

        public int NumberOfPeople { get; set; }

        public bool isDone { get; set; }
    }
}
