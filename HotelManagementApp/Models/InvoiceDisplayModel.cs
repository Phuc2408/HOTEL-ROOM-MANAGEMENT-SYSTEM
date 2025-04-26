namespace HotelManagementApp.Models
{
    public class InvoiceDisplayModel
    {
        public string InvoiceID { get; set; }
        public string GuestName { get; set; }
        public string RentID { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal Total { get; set; }
    }
}
