using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotelManagementApp.ViewModels  // 🔴 Bắt buộc!
{
    public class CheckInDialogViewModel : INotifyPropertyChanged
    {
        // Thông tin khách
        private string _guestName;
        public string GuestName
        {
            get => _guestName;
            set => SetProperty(ref _guestName, value);
        }
        private string _idCard;
        public string IdCard
        {
            get => _idCard;
            set => SetProperty(ref _idCard, value);
        }
        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }
        private string _selectedCountry;
        public string SelectedCountry
        {
            get => _selectedCountry;
            set => SetProperty(ref _selectedCountry, value);
        }

        // Thông tin phòng & đặt phòng
        public string RoomId { get; set; }       

        private string _peopleCount;
        public string PeopleCount
        {
            get => _peopleCount;
            set => SetProperty(ref _peopleCount, value);
        }

        private DateTime? _checkInDate = DateTime.Today;
        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set => SetProperty(ref _checkInDate, value);
        }
        private DateTime? _checkOutDate = DateTime.Today.AddDays(1);
        public DateTime? CheckOutDate
        {
            get => _checkOutDate;
            set => SetProperty(ref _checkOutDate, value);
        }

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

        
        
    }

}
