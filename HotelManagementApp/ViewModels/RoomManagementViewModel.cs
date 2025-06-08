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
                    // Giai đoạn 1: Lấy tất cả các phòng. Đối với mỗi phòng, nếu nó đang "Occupied" (hoặc "in_use"),
                    // tìm bản ghi Rent được coi là "active" (isDone = false).
                    var roomsAndPotentialRent = context.Room
                        .Select(roomEntity => new // Tạo một đối tượng tạm cho mỗi roomEntity từ bảng Room
                        {
                            RoomData = roomEntity, // Thông tin cơ bản của phòng
                                                   // Tìm bản ghi Rent "active" (isDone = false) cho phòng này.
                                                   // Nếu có nhiều (không nên xảy ra cho phòng Occupied), lấy bản ghi có CheckInDate mới nhất.
                            ActiveRentData = (roomEntity.RStatus == "in_use")
                                ? context.Rent
                                    .Where(rent => rent.RID == roomEntity.RID && rent.isDone == false) 
                                    .OrderByDescending(rent => rent.CheckInDate)
                                    .FirstOrDefault() // Lấy 1 hoặc null
                                : null // Nếu phòng không Occupied/in_use, không có ActiveRentData
                        })
                        .ToList(); // Thực thi truy vấn, lấy danh sách các phòng và ActiveRentData (nếu có)

                    // Giai đoạn 2: Thu thập Customer ID từ các ActiveRentData (nếu có)
                    var customerIdsToFetch = roomsAndPotentialRent
                        .Where(rwp => rwp.ActiveRentData != null)
                        .Select(rwp => rwp.ActiveRentData.CID)
                        .Distinct()
                        .ToList();

                    // Giai đoạn 3: Lấy thông tin tất cả Customer cần thiết trong một lượt
                    var customersDictionary = context.Customer
                        .Where(c => customerIdsToFetch.Contains(c.CID))
                        .ToDictionary(c => c.CID);

                    // Giai đoạn 4: Tạo danh sách RoomModel cuối cùng để hiển thị
                    var finalRoomModels = roomsAndPotentialRent.Select(temp =>
                    {
                        Customer customerData = null;
                        if (temp.ActiveRentData != null && customersDictionary.TryGetValue(temp.ActiveRentData.CID, out var cust))
                        {
                            customerData = cust;
                        }

                        return new RoomModel
                        {
                            RID = temp.RoomData.RID,
                            RFloor = temp.RoomData.RFloor,
                            RType = temp.RoomData.RType,
                            RStatus = temp.RoomData.RStatus,

                            CName = (temp.RoomData.RStatus == "in_use") && customerData != null
                                        ? customerData.CName
                                        : null,
                            CheckInDate = ((temp.RoomData.RStatus == "in_use") && temp.ActiveRentData != null)
                                        ? temp.ActiveRentData.CheckInDate
                                        : (DateTime?)null
                        };
                    }).ToList();

                    Rooms = new ObservableCollection<RoomModel>(finalRoomModels);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load RoomModel: {ex.Message}\n{ex.StackTrace}", "Lỗi dữ liệu");
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
