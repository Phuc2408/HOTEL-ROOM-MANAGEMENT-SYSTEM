using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotelManagementApp.ViewModels
{
    public class CheckInDialogViewModel : INotifyPropertyChanged
    {
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

        private string _roomId;
        public string RoomId
        {
            get => _roomId;
            set => SetProperty(ref _roomId, value);
        }

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
        private bool _isRepairRequested;
        public bool IsRepairRequested
        {
            get => _isRepairRequested;
            set => SetProperty(ref _isRepairRequested, value);
        }
    }
}
