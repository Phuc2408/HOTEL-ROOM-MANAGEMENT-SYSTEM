using HotelManagementApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Globalization;


namespace HotelManagementApp.ViewModels
{
    public class RoomManagementViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RoomModel> Rooms { get; set; }
        public ObservableCollection<RoomModel> FilteredRooms { get; set; } = new ObservableCollection<RoomModel>();
        public ObservableCollection<int> Floors { get; set; } = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

        private int _selectedFloor = 1;
        public int SelectedFloor
        {
            get => _selectedFloor;
            set
            {
                if (_selectedFloor != value)
                {
                    _selectedFloor = value;
                    OnPropertyChanged(nameof(SelectedFloor));
                    UpdateFilteredRooms();
                }
            }
        }

        public ICommand ChangeFloorCommand { get; }


        public RoomManagementViewModel()
        {
            // Không cần GenerateSampleRooms nữa
            Rooms = new ObservableCollection<RoomModel>
    {
        new RoomModel { RoomId = 101, Floor = 1, Status = "in_use", GuestName = "Anh", CheckInDate = DateTime.Today },
    new RoomModel { RoomId = 102, Floor = 1,RoomType = "Deluxe", Status = "in_use", GuestName = "Bình", CheckInDate = DateTime.Today.AddDays(-1) },
    new RoomModel { RoomId = 103, Floor = 1,RoomType = "Suit", Status = "cleaning" },
    new RoomModel { RoomId = 104, Floor = 1,RoomType = "Double Bed", Status = "repairing" },
    new RoomModel { RoomId = 105, Floor = 1,RoomType = "Deluxe", Status = "overdue", GuestName = "Chị Lan", CheckInDate = DateTime.Today.AddDays(-3) },
    new RoomModel { RoomId = 106, Floor = 1,RoomType = "Deluxe", Status = "empty" },
    new RoomModel { RoomId = 107, Floor = 1,RoomType = "Deluxe", Status = "empty" },
    new RoomModel { RoomId = 108, Floor = 1, Status = "in_use", GuestName = "Đạt", CheckInDate = DateTime.Today },
    new RoomModel { RoomId = 109, Floor = 1, Status = "cleaning" },
    new RoomModel { RoomId = 110, Floor = 1, Status = "empty" },
    new RoomModel { RoomId = 111, Floor = 1, Status = "repairing" },
    new RoomModel { RoomId = 112, Floor = 1, Status = "empty" },
    new RoomModel { RoomId = 113, Floor = 1, Status = "empty" },
    new RoomModel { RoomId = 114, Floor = 1, Status = "overdue", GuestName = "Hải", CheckInDate = DateTime.Today.AddDays(-2) },
    new RoomModel { RoomId = 115, Floor = 1, Status = "in_use", GuestName = "Kim", CheckInDate = DateTime.Today },
    new RoomModel { RoomId = 116, Floor = 1, Status = "empty" },
    new RoomModel { RoomId = 117, Floor = 1, Status = "empty" },
    new RoomModel { RoomId = 118, Floor = 1, Status = "cleaning" },
    new RoomModel { RoomId = 119, Floor = 1, Status = "repairing" },
    new RoomModel { RoomId = 120, Floor = 1, Status = "in_use", GuestName = "Linh", CheckInDate = DateTime.Today },
    new RoomModel { RoomId = 121, Floor = 1, Status = "empty" },
    new RoomModel { RoomId = 122, Floor = 1, Status = "overdue", GuestName = "Mai", CheckInDate = DateTime.Today.AddDays(-4) },
    new RoomModel { RoomId = 123, Floor = 1, Status = "empty" },
    new RoomModel { RoomId = 124, Floor = 1, Status = "in_use", GuestName = "Nam", CheckInDate = DateTime.Today },
    new RoomModel { RoomId = 125, Floor = 1, Status = "empty" },
    new RoomModel { RoomId = 126, Floor = 1, Status = "cleaning" },
    new RoomModel { RoomId = 127, Floor = 1, Status = "repairing" },
    new RoomModel { RoomId = 128, Floor = 1, Status = "empty" },
    };

            ChangeFloorCommand = new RelayCommand<int>(floor => SelectedFloor = floor);
            UpdateFilteredRooms();
        }

        private void UpdateFilteredRooms()
        {
            FilteredRooms.Clear();
            foreach (var room in Rooms.Where(r => r.Floor == SelectedFloor))
                FilteredRooms.Add(room);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private DateTime _currentDate = DateTime.Today;
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));
                OnPropertyChanged(nameof(CurrentDateString));
            }
        }

        public string CurrentDateString =>
            CurrentDate.ToString("dddd, dd/MM/yyyy", new CultureInfo("vi-VN"));
    }
}
