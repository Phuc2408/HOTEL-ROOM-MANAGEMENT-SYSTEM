using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HotelManagementApp.Models;
using System.Windows.Media;
using HotelManagementApp.Database;
using System;

namespace HotelManagementApp.ViewModels
{
    public class GuestManagementViewModel : INotifyPropertyChanged
    {
        private AppDbContext _context;
        private ObservableCollection<GuestModel> _guests;
        public ObservableCollection<GuestModel> Guests
        {
            get => _guests;
            set
            {
                _guests = value;
                OnPropertyChanged();
            }
        }

        public GuestManagementViewModel()
        {
            _context = new AppDbContext();
            LoadGuests();
        }

        public void LoadGuests()
        {
            var guestList = (from rent in _context.Rent
                             join customer in _context.Customer on rent.CID equals customer.CID
                             join room in _context.Room on rent.RID equals room.RID
                             where rent.CheckInDate == _context.Rent
                                 .Where(r2 => r2.RID == rent.RID)
                                 .Max(r2 => r2.CheckInDate)
                             select new GuestModel
                             {
                                 CID = customer.CID,
                                 GuestName = customer.CName,
                                 IdCard = customer.CPersonalID,
                                 PhoneNumber = customer.CPhone,
                                 Email = customer.CMail,
                                 Country = customer.CCountry,
                                 Room = "Room " + room.RID,
                                 CheckInDate = rent.CheckInDate,
                                 CheckOutDate = rent.CheckOutDate,
                                 StatusColor = (room.RStatus == "in_use") ? Brushes.Green :
                                               (room.RStatus == "checked_out" || room.RStatus == "overdue") ? Brushes.Red :
                                               Brushes.Gray
                             }).ToList();

            Guests = new ObservableCollection<GuestModel>(guestList);
        }

        public GuestModel? SelectedGuest { get; set; }

        public void UpdateGuest(GuestViewModel guest)
        {
            using (var context = new AppDbContext())
            {
                var customer = context.Customer.FirstOrDefault(c => c.CID == guest.CID);
                if (customer != null)
                {
                    customer.CName = guest.CName;
                    customer.CPhone = guest.CPhone;
                    customer.CPersonalID = guest.CPersonalID;
                    context.SaveChanges();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
