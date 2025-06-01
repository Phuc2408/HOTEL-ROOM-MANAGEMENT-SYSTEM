using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using HotelManagementApp.Models;
using HotelManagementApp.Database;

namespace HotelManagementApp.ViewModels
{
    public class InvoiceManagementViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context = new();

        // Dữ liệu gốc
        public ObservableCollection<InvoiceDisplayModel> AllInvoices { get; set; } = new();

        // View đã lọc cho DataGrid
        public ICollectionView Invoices { get; set; }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                Invoices?.Refresh();
            }
        }

        public InvoiceManagementViewModel()
        {
            Invoices = CollectionViewSource.GetDefaultView(AllInvoices);
            Invoices.Filter = FilterInvoice;

            LoadInvoicesFromDatabase();
        }

        private void LoadInvoicesFromDatabase()
        {
            var invoiceList = (from invoice in _context.Invoice
                               join customer in _context.Customer on invoice.CID equals customer.CID
                               join rent in _context.Rent on invoice.RelID equals rent.RelID
                               join room in _context.Room on rent.RID equals room.RID
                               select new InvoiceDisplayModel
                               {
                                   IID = invoice.IID,
                                   InvoiceID = "INV" + invoice.IID.ToString("D3"),
                                   GuestName = customer.CName,
                                   RentID = "RNT" + rent.RelID.ToString(),
                                   CheckOutDate = rent.CheckOutDate,
                                   Total = invoice.Total,
                                   RoomType = room.RType,
                                   RoomPrice = room.RPrice
                               }).ToList();

            AllInvoices.Clear();
            foreach (var item in invoiceList)
                AllInvoices.Add(item);

            Invoices.Refresh();
        }

        private bool FilterInvoice(object obj)
        {
            if (obj is InvoiceDisplayModel invoice)
            {
                if (string.IsNullOrWhiteSpace(SearchText)) return true;

                return invoice.InvoiceID?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || invoice.GuestName?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || invoice.RentID?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || invoice.RoomType?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
