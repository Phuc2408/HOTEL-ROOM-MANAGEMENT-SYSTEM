using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using HotelManagementApp.Models;
using HotelManagementApp.Database; // DbContext của bạn

namespace HotelManagementApp.ViewModels
{
    public class InvoiceManagementViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<InvoiceDisplayModel> _invoices;
        public ObservableCollection<InvoiceDisplayModel> Invoices
        {
            get => _invoices;
            set
            {
                _invoices = value;
                OnPropertyChanged();
            }
        }

        public InvoiceManagementViewModel()
        {
            LoadInvoicesFromDatabase();
        }

        private void LoadInvoicesFromDatabase()
        {
            using (var context = new AppDbContext())
            {
                var invoiceList = (from invoice in context.Invoice
                                   join customer in context.Customer on invoice.CID equals customer.CID
                                   join rent in context.Rent on invoice.RelID equals rent.RelID
                                   select new InvoiceDisplayModel
                                   {
                                       InvoiceID = "INV" + invoice.IID.ToString("D3"),
                                       GuestName = customer.CName,
                                       RentID = "RNT" + rent.RelID.ToString(),
                                       CheckOutDate = rent.CheckOutDate,
                                       Total = invoice.Total
                                   }).ToList();

                Invoices = new ObservableCollection<InvoiceDisplayModel>(invoiceList);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
