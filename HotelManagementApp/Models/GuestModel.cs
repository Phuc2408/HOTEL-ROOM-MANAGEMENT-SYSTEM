using System;
using System.Windows.Media;

namespace HotelManagementApp.Models
{
    public class GuestModel
    {
        public int CID { get; set; } // <- Bổ sung để đồng bộ với ViewModel và DB
        public string GuestName { get; set; }
        public string IdCard { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Room { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public Brush StatusColor { get; set; }
    }
}