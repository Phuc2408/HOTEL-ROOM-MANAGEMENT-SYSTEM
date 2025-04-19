using System;
using System.Windows.Media;

namespace HotelManagementApp.Models
{
    public class GuestModel
    {
        public string? GuestName { get; set; }
        public string? IdCard { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Country { get; set; }
        public string? Room { get; set; }
        public int PeopleCount { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public SolidColorBrush? StatusColor { get; internal set; }
    }
}
