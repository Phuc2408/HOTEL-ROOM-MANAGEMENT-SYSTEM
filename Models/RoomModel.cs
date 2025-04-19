using System;
using System.ComponentModel;

namespace HotelManagementApp.Models
{
    public class RoomModel : INotifyPropertyChanged
    {
        public int RoomId { get; set; }
        public int Floor { get; set; }
        public string RoomType { get; set; }
        private string _status = "empty";
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        private string _guestName;
        public string GuestName
        {
            get => _guestName;
            set { _guestName = value; OnPropertyChanged(nameof(GuestName)); }
        }

        private DateTime? _checkInDate;
        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set { _checkInDate = value; OnPropertyChanged(nameof(CheckInDate)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
