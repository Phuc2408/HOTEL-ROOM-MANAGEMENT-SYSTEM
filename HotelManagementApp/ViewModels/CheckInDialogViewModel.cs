using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotelManagementApp.ViewModels  // 🔴 Bắt buộc!
{
    public class CheckInDialogViewModel : INotifyPropertyChanged
    {
        // Thông tin khách
        public string GuestName { get; set; }
        public string IdCard { get; set; }
        public string PhoneNumber { get; set; }
        public string SelectedCountry { get; set; }

        // Thông tin phòng & đặt phòng
        public string RoomId { get; set; }          // Dạng string từ UI
        public string PeopleCount { get; set; }     // Dạng string từ UI

        public DateTime? CheckInDate { get; set; } = DateTime.Today;
        public DateTime? CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        // Giá trị convert sẵn để gọi Service
        public int RoomIdInt => int.TryParse(RoomId, out int r) ? r : 0;
        public int PeopleCountInt => int.TryParse(PeopleCount, out int p) ? p : 1;
        public DateTime CheckInDateValue => CheckInDate ?? DateTime.Today;
        public DateTime CheckOutDateValue => CheckOutDate ?? DateTime.Today.AddDays(1);

        // Yêu cầu sửa phòng nếu có
        public bool IsRepairRequested { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public CheckInDialogViewModel()
        {
            // No need for countries list anymore
        }
        
    }

}
