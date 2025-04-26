using HotelManagementApp.Models;
using HotelManagementApp.Database;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Globalization;
using System.Windows;

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
            try
            {
                using (var context = new AppDbContext())
                {
                    var roomsFromDb = (from room in context.Room
                                       join rent in context.Rent on room.RID equals rent.RID into rentGroup
                                       from rent in rentGroup.DefaultIfEmpty()
                                       join cust in context.Customer on rent.CID equals cust.CID into custGroup
                                       from cust in custGroup.DefaultIfEmpty()
                                       select new RoomModel
                                       {
                                           RID = room.RID,
                                           RFloor = room.RFloor,
                                           RType = room.RType,
                                           RStatus = room.RStatus,
                                           CName = cust.CName,
                                           CheckInDate = rent.CheckInDate
                                       }).ToList();

                    Rooms = new ObservableCollection<RoomModel>(roomsFromDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load RoomModel: {ex.Message}", "Lỗi dữ liệu");
                Rooms = new ObservableCollection<RoomModel>();
            }

            ChangeFloorCommand = new RelayCommand<int>(floor => SelectedFloor = floor);
            UpdateFilteredRooms();
        }

        private void UpdateFilteredRooms()
        {
            FilteredRooms.Clear();
            if (Rooms == null) return;

            foreach (var room in Rooms.Where(r => r.RFloor == SelectedFloor))
            {
                FilteredRooms.Add(room);
            }
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
