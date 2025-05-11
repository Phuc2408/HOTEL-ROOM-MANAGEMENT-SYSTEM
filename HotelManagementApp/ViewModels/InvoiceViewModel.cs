using System;
using System.Collections.Generic;

namespace HotelManagementApp.ViewModels
{
    public class InvoiceViewModel
    {
        public int InvoiceID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal RoomTotal { get; set; }
        public decimal ServiceTotal { get; set; }
        public decimal Total => RoomTotal + ServiceTotal;

        public string GuestName { get; set; }
        public string IdCard { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string SelectedCountry { get; set; }

        public int RoomId { get; set; }
        public int PeopleCount { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public List<ServiceUsageViewModel> Services { get; set; } = new();
    }
}
