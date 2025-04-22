using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HotelManagementApp.Models;

namespace HotelManagementApp.ViewModels
{
    public class InvoiceManagementViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<InvoiceModel> _invoices;

        public ObservableCollection<InvoiceModel> Invoices
        {
            get => _invoices;
            set
            {
                _invoices = value;
                OnPropertyChanged();
            }
        }

        // Constructor
        public InvoiceManagementViewModel()
        {
            // Tạo dữ liệu mẫu cho Invoice
            Invoices = new ObservableCollection<InvoiceModel>
            {
                new InvoiceModel { InvoiceID = "INV001", GuestName = "Nguyễn Văn A", RentID = "RNT123", CheckOutDate = DateTime.Now, Total = 2500000 },
                new InvoiceModel { InvoiceID = "INV002", GuestName = "Trần Thị B", RentID = "RNT124", CheckOutDate = DateTime.Now.AddDays(1), Total = 3000000 }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
