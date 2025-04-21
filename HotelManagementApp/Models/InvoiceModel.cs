using System.Collections.ObjectModel;

namespace HotelManagementApp.Models
{
    public class InvoiceModel
    {
        public string InvoiceID { get; set; }   
        public string GuestName { get; set; }
        public string RentID { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal Total { get; set; }
        public ObservableCollection<ServiceModel> Services { get; set; }

        public InvoiceModel()
        {
            Services = new ObservableCollection<ServiceModel>();
        }
    }
}
