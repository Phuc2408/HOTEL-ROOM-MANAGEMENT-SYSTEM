using HotelManagementApp.Database;
using HotelManagementApp.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HotelManagementApp.ViewModels
{
    public class InvoiceDetailViewModel : INotifyPropertyChanged
    {
        private AppDbContext _context;
        private InvoiceViewModel _invoice;

        public InvoiceViewModel Invoice
        {
            get => _invoice;
            set
            {
                _invoice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        public decimal TotalAmount => Invoice?.Services?.Sum(s => s.Quantity * s.Price) ?? 0;

        public InvoiceDetailViewModel(Invoice invoice)
        {
            _context = new AppDbContext();
            LoadInvoiceDetail(invoice);
        }

        private void LoadInvoiceDetail(Invoice invoice)
        {
            if (invoice == null) return;
            int invoiceId = invoice.IID;

            var invoiceDb = _context.Invoice.FirstOrDefault(i => i.IID == invoiceId);
            if (invoiceDb == null) return;

            var rent = _context.Rent.FirstOrDefault(r => r.ReID == invoiceDb.ReID);
            var customer = rent != null ? _context.Customer.FirstOrDefault(c => c.CID == rent.CID) : null;
            var room = rent != null ? _context.Room.FirstOrDefault(r => r.RID == rent.RID) : null;

            var usages = _context.ServiceUsage
                .Where(u => u.ReID == invoiceDb.ReID)
                .ToList();

            var services = usages.Select(u =>
            {
                var service = _context.Service.FirstOrDefault(s => s.SID == u.SID);
                return new ServiceUsageViewModel
                {
                    Name = service?.SName,
                    Unit = service?.SUnit,
                    Price = service?.SPrice ?? 0,
                    Quantity = u.Quantity
                };
            }).ToList();

            // === Tính số ngày thuê ===
            int numberOfDays = 1;
            if (rent != null)
            {
                numberOfDays = (invoiceDb.IDate.Date - rent.CheckInDate.Date).Days;
                if (numberOfDays <= 0) numberOfDays = 1;

                var delay = invoiceDb.IDate - (invoiceDb.IDate.Date + new TimeSpan(12, 0, 0));
                if (delay.TotalHours > 6)
                {
                    numberOfDays += 1;
                }
            }

            // === Dòng phí phòng tính đúng số ngày ===
            if (room != null && numberOfDays > 0)
            {
                decimal unitPrice = invoiceDb.RoomTotal / numberOfDays;
                var roomLine = new ServiceUsageViewModel
                {
                    Name = $"Room Type: {room.RType}",
                    Unit = "day",
                    Price = unitPrice,
                    Quantity = numberOfDays
                };
                services.Insert(0, roomLine);
            }

            Invoice = new InvoiceViewModel
            {
                InvoiceID = invoiceDb.IID,
                InvoiceDate = invoiceDb.IDate,
                RoomTotal = invoiceDb.RoomTotal,
                ServiceTotal = invoiceDb.ServiceTotal,
                GuestName = customer?.CName,
                IdCard = customer?.CPersonalID,
                PhoneNumber = customer?.CPhone,
                Email = customer?.CMail,
                SelectedCountry = customer?.CCountry,
                RoomId = rent?.RID ?? 0,
                PeopleCount = rent?.NumberOfPeople ?? 0,
                CheckInDate = rent?.CheckInDate ?? DateTime.MinValue,
                CheckOutDate = rent?.CheckOutDate ?? DateTime.MinValue,
                Services = services
            };

            OnPropertyChanged(nameof(Invoice));
            OnPropertyChanged(nameof(TotalAmount));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
