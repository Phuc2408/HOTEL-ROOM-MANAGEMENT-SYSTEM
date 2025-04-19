using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HotelManagementApp.Models;
using System.Windows.Media;

namespace HotelManagementApp.ViewModels
{
    public class GuestManagementViewModel : INotifyPropertyChanged
    {
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
            // Dữ liệu mẫu
            var guestList = new List<GuestModel>
            {
                new GuestModel { GuestName = "Nguyễn Văn A", IdCard = "KA0963", PhoneNumber = "(225) 555-0118", Email = "nguyenvana@example.com", Country = "Vietnam", Room = "Room 101", CheckInDate = new DateTime(2025, 4, 15), CheckOutDate = new DateTime(2025, 4, 20), StatusColor = Brushes.Green },
                new GuestModel { GuestName = "Trần Thị B", IdCard = "KA0964", PhoneNumber = "(225) 555-0120", Email = "tranb@example.com", Country = "Vietnam", Room = "Room 102", CheckInDate = new DateTime(2025, 4, 14), CheckOutDate = new DateTime(2025, 4, 18), StatusColor = Brushes.Red }
            };

            // Tạo danh sách và thêm STT tự động
            Guests = new ObservableCollection<GuestModel>(
                guestList.Select((guest, index) => new GuestModel
                {
                    GuestName = guest.GuestName,
                    IdCard = guest.IdCard,
                    PhoneNumber = guest.PhoneNumber,
                    Email = guest.Email,
                    Country = guest.Country,
                    Room = guest.Room,
                    CheckInDate = guest.CheckInDate,
                    CheckOutDate = guest.CheckOutDate,
                    StatusColor = guest.StatusColor
                }));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
