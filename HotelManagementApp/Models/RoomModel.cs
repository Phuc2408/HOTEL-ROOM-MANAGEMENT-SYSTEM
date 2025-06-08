using System;
using System.ComponentModel;

namespace HotelManagementApp.Models
{
    public class RoomModel : INotifyPropertyChanged
    {
        // Thuộc tính gốc khớp với DB
        private int _rid;
        public int RID
        {
            get => _rid;
            set { _rid = value; OnPropertyChanged(nameof(RID)); OnPropertyChanged(nameof(RoomId)); }
        }

        private int _rfloor;
        public int RFloor
        {
            get => _rfloor;
            set { _rfloor = value; OnPropertyChanged(nameof(RFloor)); OnPropertyChanged(nameof(Floor)); }
        }

        private string _rtype;
        public string RType
        {
            get => _rtype;
            set { _rtype = value; OnPropertyChanged(nameof(RType)); OnPropertyChanged(nameof(RoomType)); }
        }

        private string _rstatus = "empty";
        public string RStatus
        {
            get => _rstatus;
            set { _rstatus = value; OnPropertyChanged(nameof(RStatus)); OnPropertyChanged(nameof(Status)); }
        }

        private string _cname;
        public string CName
        {
            get => _cname;
            set { _cname = value; OnPropertyChanged(nameof(CName)); OnPropertyChanged(nameof(GuestName)); }
        }

        private DateTime? _checkInDate;
        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set { _checkInDate = value; OnPropertyChanged(nameof(CheckInDate)); }
        }

        // Alias cho ViewModel cũ / XAML binding
        public int RoomId
        {
            get => RID;
            set => RID = value;
        }

        public int Floor
        {
            get => RFloor;
            set => RFloor = value;
        }

        public string RoomType
        {
            get => RType;
            set => RType = value;
        }

        public string Status
        {
            get => RStatus;
            set => RStatus = value;
        }

        public string GuestName
        {
            get => CName;
            set => CName = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
