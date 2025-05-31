using HotelManagementApp.Database;
using HotelManagementApp.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace HotelManagementApp.ViewModels
{
    public class DashBoardViewModel : INotifyPropertyChanged
    {
        // ============================
        // 1. Các property cho đồng hồ
        // ============================
        private string _currentTime;
        public string CurrentTime
        {
            get => _currentTime;
            set { _currentTime = value; OnPropertyChanged(nameof(CurrentTime)); }
        }

        private double _hourAngle;
        public double HourAngle
        {
            get => _hourAngle;
            set { _hourAngle = value; OnPropertyChanged(nameof(HourAngle)); }
        }

        private double _minuteAngle;
        public double MinuteAngle
        {
            get => _minuteAngle;
            set { _minuteAngle = value; OnPropertyChanged(nameof(MinuteAngle)); }
        }

        private double _secondAngle;
        public double SecondAngle
        {
            get => _secondAngle;
            set { _secondAngle = value; OnPropertyChanged(nameof(SecondAngle)); }
        }

        // ============================
        // 2. Constructor và StartClock() (giữ nguyên)
        // ============================
        public DashBoardViewModel()
        {
            StartClock();
            // Sau khi khởi động đồng hồ, load dữ liệu Dashboard
            LoadDashboardData();
        }

        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                var now = DateTime.Now;
                CurrentTime = now.ToString("hh:mm:ss tt");
                HourAngle = (now.Hour % 12 + now.Minute / 60.0) * 30;
                MinuteAngle = (now.Minute + now.Second / 60.0) * 6;
                SecondAngle = now.Second * 6;
            };
            timer.Start();
        }

        // ============================
        // 3. Các property cho Dashboard Cards
        // ============================

        private int _availableRooms;
        /// <summary>
        /// Số phòng trống (Available) – tương đương Count(RStatus="empty")
        /// </summary>
        public int AvailableRooms
        {
            get => _availableRooms;
            set
            {
                if (_availableRooms != value)
                {
                    _availableRooms = value;
                    OnPropertyChanged(nameof(AvailableRooms));
                }
            }
        }

        private int _checkIn;
        /// <summary>
        /// Số phòng check-in trong ngày SelectedDate
        /// </summary>
        public int CheckIn
        {
            get => _checkIn;
            set
            {
                if (_checkIn != value)
                {
                    _checkIn = value;
                    OnPropertyChanged(nameof(CheckIn));
                }
            }
        }

        private int _checkOut;
        /// <summary>
        /// Số phòng check-out trong ngày SelectedDate
        /// </summary>
        public int CheckOut
        {
            get => _checkOut;
            set
            {
                if (_checkOut != value)
                {
                    _checkOut = value;
                    OnPropertyChanged(nameof(CheckOut));
                }
            }
        }

        // ============================
        // 4. Các property cho Status Table
        // ============================

        private int _availableRoomsStatus;
        /// <summary>
        /// Số phòng RStatus = "empty"
        /// </summary>
        public int AvailableRoomsStatus
        {
            get => _availableRoomsStatus;
            set
            {
                if (_availableRoomsStatus != value)
                {
                    _availableRoomsStatus = value;
                    OnPropertyChanged(nameof(AvailableRoomsStatus));
                }
            }
        }

        private int _untidyRoomsStatus;
        /// <summary>
        /// Số phòng RStatus = "cleaning" (tức Untidy)
        /// </summary>
        public int UntidyRoomsStatus
        {
            get => _untidyRoomsStatus;
            set
            {
                if (_untidyRoomsStatus != value)
                {
                    _untidyRoomsStatus = value;
                    OnPropertyChanged(nameof(UntidyRoomsStatus));
                }
            }
        }

        private int _repairRoomsStatus;
        /// <summary>
        /// Số phòng RStatus = "repairing"
        /// </summary>
        public int RepairRoomsStatus
        {
            get => _repairRoomsStatus;
            set
            {
                if (_repairRoomsStatus != value)
                {
                    _repairRoomsStatus = value;
                    OnPropertyChanged(nameof(RepairRoomsStatus));
                }
            }
        }

        private int _inUseRoomsStatus;
        /// <summary>
        /// Số phòng RStatus = "in use"
        /// </summary>
        public int InUseRoomsStatus
        {
            get => _inUseRoomsStatus;
            set
            {
                if (_inUseRoomsStatus != value)
                {
                    _inUseRoomsStatus = value;
                    OnPropertyChanged(nameof(InUseRoomsStatus));
                }
            }
        }

        // ============================
        // 5. Các property cho Room Type Available
        // ============================

        private int _singleRoomsCount;
        public int SingleRoomsCount
        {
            get => _singleRoomsCount;
            set
            {
                if (_singleRoomsCount != value)
                {
                    _singleRoomsCount = value;
                    OnPropertyChanged(nameof(SingleRoomsCount));
                }
            }
        }

        private int _twinRoomsCount;
        public int TwinRoomsCount
        {
            get => _twinRoomsCount;
            set
            {
                if (_twinRoomsCount != value)
                {
                    _twinRoomsCount = value;
                    OnPropertyChanged(nameof(TwinRoomsCount));
                }
            }
        }

        private int _doubleRoomsCount;
        public int DoubleRoomsCount
        {
            get => _doubleRoomsCount;
            set
            {
                if (_doubleRoomsCount != value)
                {
                    _doubleRoomsCount = value;
                    OnPropertyChanged(nameof(DoubleRoomsCount));
                }
            }
        }

        private int _vipRoomsCount;
        public int VIPRoomsCount
        {
            get => _vipRoomsCount;
            set
            {
                if (_vipRoomsCount != value)
                {
                    _vipRoomsCount = value;
                    OnPropertyChanged(nameof(VIPRoomsCount));
                }
            }
        }

        // ============================
        // 6. Property SelectedDate cho Calendar
        // ============================
        private DateTime _selectedDate = DateTime.Today;
        /// <summary>
        /// Ngày đang chọn trên Calendar. Mặc định Today.
        /// Khi thay đổi, sẽ gọi lại LoadDashboardData().
        /// </summary>
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                // Chỉ load data lại nếu ngày thực sự thay đổi
                if (_selectedDate.Date != value.Date)
                {
                    _selectedDate = value.Date;
                    OnPropertyChanged(nameof(SelectedDate));
                    LoadDashboardData();
                }
            }
        }

        // ============================
        // 7. Phương thức LoadDashboardData()
        //    – Đếm các giá trị từ database
        // ============================
        private void LoadDashboardData()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    DateTime today = SelectedDate.Date;

                    // 1. Đếm số phòng check-in trong ngày today (Rent.CheckInDate)
                    CheckIn = db.Rent
                        .Where(r => r.CheckInDate.Date == today)
                        .Count();

                    // 2. Đếm số phòng check-out trong ngày today (Rent.CheckOutDate)
                    CheckOut = db.Rent

                        .Where(r => r.CheckOutDate != null && r.CheckOutDate.Date == today)
                        .Count();

                    // 3. Đếm số phòng theo trạng thái Room.RStatus
                    //    Giả sử RStatus lưu: "available", "in use", "cleaning", "repairing"
                    AvailableRoomsStatus = db.Room
                        .Where(r => r.RStatus.ToLower() == "available")
                        .Count();

                    InUseRoomsStatus = db.Room
                        .Where(r => r.RStatus.ToLower() == "in_use" || r.RStatus.ToLower() == "inuse")
                        .Count();

                    UntidyRoomsStatus = db.Room
                        .Where(r => r.RStatus.ToLower() == "cleaning")
                        .Count();

                    RepairRoomsStatus = db.Room
                        .Where(r => r.RStatus.ToLower() == "repairing")
                        .Count();

                    // 4. Số phòng trống (cùng giá trị với AvailableRoomsStatus)
                    AvailableRooms = AvailableRoomsStatus;

                    // 5. Đếm phòng trống theo loại RType khi RStatus = "available"
                    SingleRoomsCount = db.Room
                        .Where(r => r.RStatus.ToLower() == "available" && r.RType.ToLower() == "single")
                        .Count();

                    TwinRoomsCount = db.Room
                        .Where(r => r.RStatus.ToLower() == "available" && r.RType.ToLower() == "twin")
                        .Count();

                    DoubleRoomsCount = db.Room
                        .Where(r => r.RStatus.ToLower() == "available" && r.RType.ToLower() == "double")
                        .Count();

                    VIPRoomsCount = db.Room
                        .Where(r => r.RStatus.ToLower() == "available" && r.RType.ToLower() == "vip")
                        .Count();
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi khi truy vấn, hiển thị thông báo
                // (bạn có thể log thêm hoặc bắt lỗi chi tiết hơn tùy nhu cầu)
                System.Windows.MessageBox.Show(
                    $"Lỗi khi load dữ liệu Dashboard:\n{ex.Message}",
                    "Lỗi Database",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        // ============================
        // 8. INotifyPropertyChanged
        // ============================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
