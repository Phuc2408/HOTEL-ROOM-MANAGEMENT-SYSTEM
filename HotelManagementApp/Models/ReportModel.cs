using System;

namespace HotelManagementApp.Models
{
    public class ReportModel
    {
        public DateTime Date { get; set; }
        public double Revenue { get; set; }
    }


    public class MonthlyGuestStats
    {
        public string Month { get; set; } = string.Empty;
        public int GuestCount { get; set; }
    }
}
