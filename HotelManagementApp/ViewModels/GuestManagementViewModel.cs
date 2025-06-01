using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;
using HotelManagementApp.Database;
using HotelManagementApp.Models;

namespace HotelManagementApp.ViewModels
{
    public class GuestManagementViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context = new();

        // Dữ liệu gốc
        public ObservableCollection<GuestModel> AllGuests { get; set; } = new();

        // View lọc để binding lên DataGrid
        public ICollectionView Guests { get; set; }

        private GuestModel? _selectedGuest;
        public GuestModel? SelectedGuest
        {
            get => _selectedGuest;
            set
            {
                _selectedGuest = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                Guests?.Refresh();
            }
        }

        public GuestManagementViewModel()
        {
            // Khởi tạo view lọc
            Guests = CollectionViewSource.GetDefaultView(AllGuests);
            Guests.Filter = FilterGuest;

            LoadGuests();
        }

        public void LoadGuests()
        {
            // Bước 1: Truy vấn EF không dùng switch/Brushes
            var rawData = (from rent in _context.Rent
                           join customer in _context.Customer on rent.CID equals customer.CID
                           join room in _context.Room on rent.RID equals room.RID
                           where rent.CheckInDate == _context.Rent
                               .Where(r2 => r2.RID == rent.RID)
                               .Max(r2 => r2.CheckInDate)
                           select new
                           {
                               customer,
                               room,
                               rent
                           }).ToList(); // ✅ OK với EF

            // Bước 2: Ánh xạ sang GuestModel, dùng C# thường
            var guestList = rawData.Select(data => new GuestModel
            {
                CID = data.customer.CID,
                GuestName = data.customer.CName,
                IdCard = data.customer.CPersonalID,
                PhoneNumber = data.customer.CPhone,
                Email = data.customer.CMail,
                Country = data.customer.CCountry,
                Room = "Room " + data.room.RID,
                CheckInDate = data.rent.CheckInDate,
                CheckOutDate = data.rent.CheckOutDate,
                StatusColor = data.room.RStatus switch
                {
                    "in_use" => Brushes.Green,
                    "checked_out" or "overdue" => Brushes.Red,
                    _ => Brushes.Gray
                }
            }).ToList();

            // Gán vào danh sách
            AllGuests.Clear();
            foreach (var guest in guestList)
                AllGuests.Add(guest);

            Guests.Refresh();
        }

        private bool FilterGuest(object obj)
        {
            if (obj is GuestModel guest)
            {
                if (string.IsNullOrWhiteSpace(SearchText)) return true;

                return guest.GuestName?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || guest.PhoneNumber?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || guest.IdCard?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        public void UpdateGuest(GuestViewModel guest)
        {
            using var context = new AppDbContext();
            var customer = context.Customer.FirstOrDefault(c => c.CID == guest.CID);
            if (customer != null)
            {
                customer.CName = guest.CName;
                customer.CPhone = guest.CPhone;
                customer.CPersonalID = guest.CPersonalID;
                context.SaveChanges();
            }

            LoadGuests(); // Cập nhật lại sau khi sửa
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
