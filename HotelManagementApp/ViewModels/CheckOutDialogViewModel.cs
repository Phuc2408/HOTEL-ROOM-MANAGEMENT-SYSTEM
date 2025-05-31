using HotelManagementApp.Database;
using HotelManagementApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace HotelManagementApp.ViewModels
{
    public class CheckOutDialogViewModel : INotifyPropertyChanged
    {
        // ============================
        // 1. Các property hiển thị lên dialog
        // ============================

        private string _guestName;
        public string GuestName
        {
            get => _guestName;
            set
            {
                if (_guestName != value)
                {
                    _guestName = value;
                    OnPropertyChanged(nameof(GuestName));
                }
            }
        }

        private string _idCard;
        public string IdCard
        {
            get => _idCard;
            set
            {
                if (_idCard != value)
                {
                    _idCard = value;
                    OnPropertyChanged(nameof(IdCard));
                }
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        private string _selectedCountry;
        public string SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                if (_selectedCountry != value)
                {
                    _selectedCountry = value;
                    OnPropertyChanged(nameof(SelectedCountry));
                }
            }
        }

        private string _roomNumber;
        public string RoomNumber
        {
            get => _roomNumber;
            set
            {
                if (_roomNumber != value)
                {
                    _roomNumber = value;
                    OnPropertyChanged(nameof(RoomNumber));
                }
            }
        }

        private int _peopleCount;
        public int PeopleCount
        {
            get => _peopleCount;
            set
            {
                if (_peopleCount != value)
                {
                    _peopleCount = value;
                    OnPropertyChanged(nameof(PeopleCount));
                }
            }
        }

        private DateTime? _checkInDate;
        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (_checkInDate != value)
                {
                    _checkInDate = value;
                    OnPropertyChanged(nameof(CheckInDate));
                }
            }
        }

        private DateTime? _checkOutDate;
        public DateTime? CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                if (_checkOutDate != value)
                {
                    _checkOutDate = value;
                    OnPropertyChanged(nameof(CheckOutDate));
                }
            }
        }

        // ============================
        // 2. Danh sách hết dịch vụ + room
        // ============================

        public ObservableCollection<ServiceModel> Services { get; set; }

        private ICollectionView filteredServices;
        public ICollectionView FilteredServices
        {
            get => filteredServices;
            set
            {
                filteredServices = value;
                OnPropertyChanged(nameof(FilteredServices));
            }
        }

        private decimal totalPrice;
        public decimal TotalPrice
        {
            get => totalPrice;
            set
            {
                if (totalPrice != value)
                {
                    totalPrice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        // ============================
        // 3. Các biến luồng dữ liệu và trạng thái nội bộ
        // ============================

        public int CurrentCustomerId { get; set; }
        public int CurrentInvoiceId { get; set; }
        public int CurrentRoomId { get; set; }

        private DispatcherTimer saveDelayTimer;
        private ServiceModel lastChangedService;

        private decimal roomPrice;
        private string roomType;
        private ServiceModel roomItemModel;

        // Trường để lưu RelID (bản ghi Rent chưa có Invoice) 
        private int activeRelID_forCheckout;

        private bool _canPerformCheckout;
        public bool CanPerformCheckout
        {
            get => _canPerformCheckout;
            private set
            {
                if (_canPerformCheckout != value)
                {
                    _canPerformCheckout = value;
                    OnPropertyChanged(nameof(CanPerformCheckout));
                }
            }
        }

        // ============================
        // Constructor
        // ============================
        public CheckOutDialogViewModel()
        {
            saveDelayTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(600)
            };
            saveDelayTimer.Tick += SaveDelayTimer_Tick;

            Services = new ObservableCollection<ServiceModel>();
        }

        /// <summary>
        /// Phương thức khởi tạo và load dữ liệu dựa trên roomId được truyền vào
        /// </summary>
        /// <param name="roomId"></param>
        public void InitializeByRoomId(int roomId)
        {
            this.CurrentRoomId = roomId;
            this.Services.Clear();
            this.activeRelID_forCheckout = 0;
            this.CurrentCustomerId = 0;
            this.CurrentInvoiceId = 0;
            this.TotalPrice = 0;
            this.roomItemModel = null;
            this.CanPerformCheckout = false;

            using (var db = new AppDbContext())
            {
                // 1. Lấy thông tin phòng
                var roomDetails = db.Room.FirstOrDefault(r => r.RID == roomId);
                if (roomDetails == null)
                {
                    Debug.WriteLine($"[CheckOut Init] Lỗi: Không tìm thấy thông tin phòng RID: {roomId}.");
                    MessageBox.Show("Lỗi: Không tìm thấy thông tin phòng để checkout.", "Lỗi Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                    OnPropertyChanged(nameof(CanPerformCheckout));
                    return;
                }

                this.RoomNumber = roomDetails.RID.ToString();
                OnPropertyChanged(nameof(RoomNumber));

                this.roomPrice = roomDetails.RPrice;
                this.roomType = roomDetails.RType;

                // 2. Tìm Rent đang active (chưa có Invoice) cho phòng này
                var currentActiveRent = db.Rent
                    .Where(r => r.RID == roomId && !db.Invoice.Any(inv => inv.RelID == r.RelID))
                    .OrderByDescending(r => r.CheckInDate)
                    .FirstOrDefault();

                if (currentActiveRent == null)
                {
                    Debug.WriteLine($"[CheckOut Init] KHÔNG TÌM THẤY RENT ACTIVE cho Room ID: {roomId}.");
                    MessageBox.Show("Phòng này không có lượt thuê đang hoạt động (chưa thanh toán) để checkout.", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Vẫn load Services (chỉ có roomItemModel với Quantity = 0)
                    LoadServicesFromDatabase();
                    FilterServices();
                    CalculateTotalPrice();
                    OnPropertyChanged(nameof(CanPerformCheckout));
                    return;
                }

                // Lưu RelID và CustomerId của Rent đó
                this.activeRelID_forCheckout = currentActiveRent.RelID;
                this.CurrentCustomerId = currentActiveRent.CID;

                // 3. Load thông tin Customer (khách hàng) từ bảng Customer
                var customer = db.Customer.FirstOrDefault(c => c.CID == currentActiveRent.CID);
                if (customer != null)
                {
                    this.GuestName = customer.CName;  
                    this.IdCard = customer.CPersonalID;
                    this.PhoneNumber = customer.CPhone;
                    this.SelectedCountry = customer.CCountry;
                }
                else
                {
                    Debug.WriteLine($"[CheckOut Init] Lỗi: Không tìm thấy Customer CID: {currentActiveRent.CID}");
                    MessageBox.Show("Lỗi: Không tìm thấy thông tin khách hàng.", "Lỗi Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                OnPropertyChanged(nameof(GuestName));
                OnPropertyChanged(nameof(IdCard));
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(SelectedCountry));

                // 4. Gán thông tin ngày và số người
                this.PeopleCount = currentActiveRent.NumberOfPeople;      // Số lượng người
                this.CheckInDate = currentActiveRent.CheckInDate;      // Ngày Check-in
                this.CheckOutDate = currentActiveRent.CheckOutDate;    // Ngày Check-out (có thể null nếu chưa có)

                OnPropertyChanged(nameof(PeopleCount));
                OnPropertyChanged(nameof(CheckInDate));
                OnPropertyChanged(nameof(CheckOutDate));

                // 5. Kiểm tra Invoice đã tồn tại chưa
                var invoiceRecord = db.Invoice.FirstOrDefault(i => i.RelID == this.activeRelID_forCheckout);
                if (invoiceRecord == null)
                {
                    // CHƯA CÓ HÓA ĐƠN → TẠO HÓA ĐƠN NHÁP
                    Debug.WriteLine($"[CheckOut Init] Tạo Invoice nháp cho RelID: {this.activeRelID_forCheckout}");
                    invoiceRecord = new Invoice
                    {
                        CID = this.CurrentCustomerId,
                        RelID = this.activeRelID_forCheckout,
                        IDate = DateTime.Now.Date,
                        RoomTotal = this.roomPrice,
                        ServiceTotal = 0,
                        Total = this.roomPrice,
                        // Nếu có thêm field Status thì gán ở đây (ví dụ: Status = "Draft")
                    };
                    db.Invoice.Add(invoiceRecord);
                    try
                    {
                        db.SaveChanges();
                        this.CurrentInvoiceId = invoiceRecord.IID;
                        Debug.WriteLine($"[CheckOut Init] Invoice nháp (IID={this.CurrentInvoiceId}) đã được tạo.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[CheckOut Init] Lỗi khi tạo Invoice nháp: {ex.Message}");
                        MessageBox.Show("Lỗi khi khởi tạo thông tin thanh toán. Vui lòng thử lại.", "Lỗi Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
                        OnPropertyChanged(nameof(CanPerformCheckout));
                        return;
                    }
                }
                else
                {
                    // ĐÃ CÓ HÓA ĐƠN từ trước → Load lại ServiceUsage
                    Debug.WriteLine($"[CheckOut Init] Tải Invoice (IID={invoiceRecord.IID}) đã tồn tại cho RelID: {this.activeRelID_forCheckout}");
                    this.CurrentInvoiceId = invoiceRecord.IID;
                    LoadExistingServiceUsages();
                }
            } // Kết thúc using AppDbContext()

            // 6. Load toàn bộ các dịch vụ và thêm roomItemModel
            LoadServicesFromDatabase();

            // 7. Lọc ra các dịch vụ đã có quantity > 0 (bao gồm cả roomItemModel nếu quantity > 0)
            FilterServices();

            // 8. Tính tổng giá 
            CalculateTotalPrice();

            // 9. Cho phép nút Confirm chỉ khi Rent đang active (activeRelID_forCheckout != 0)
            this.CanPerformCheckout = (this.activeRelID_forCheckout != 0);
            OnPropertyChanged(nameof(CanPerformCheckout));
        }

        /// <summary>
        /// Thực hiện lưu thông tin thanh toán khi người dùng bấm Confirm
        /// (gọi trực tiếp từ code‐behind hoặc command)
        /// </summary>
        /// <returns></returns>
        public bool PerformCheckout()
        {
            if (!CanPerformCheckout || this.activeRelID_forCheckout == 0 || this.CurrentInvoiceId == 0)
            {
                Debug.WriteLine("[PerformCheckout] Thiếu dữ liệu để checkout.");
                MessageBox.Show("Không thể thực hiện checkout. Thông tin không đầy đủ.", "Lỗi Checkout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Nếu timer vẫn đang chạy → dừng và lưu service cuối cùng
            if (saveDelayTimer.IsEnabled)
            {
                saveDelayTimer.Stop();
                if (lastChangedService != null)
                    SaveOrUpdateServiceUsage(lastChangedService);
            }
            CalculateTotalPrice();

            using (var db = new AppDbContext())
            {
                // 1. Tìm Invoice từ CSDL
                var invoiceToFinalize = db.Invoice.FirstOrDefault(i => i.IID == this.CurrentInvoiceId);
                if (invoiceToFinalize == null)
                {
                    Debug.WriteLine($"[PerformCheckout] Invoice (IID={this.CurrentInvoiceId}) không tồn tại.");
                    MessageBox.Show("Lỗi: Không tìm thấy hóa đơn để hoàn tất checkout.", "Lỗi Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // 2. Cập nhật tổng tiền
                invoiceToFinalize.RoomTotal = this.roomItemModel?.TotalAmount ?? 0;
                invoiceToFinalize.ServiceTotal = Math.Max(0, this.TotalPrice - (this.roomItemModel?.TotalAmount ?? 0));
                invoiceToFinalize.Total = this.TotalPrice;
                // Nếu có trường Status trong Invoice, ví dụ: Status = "Finalized";

                // 3. Cập nhật rent → đặt CheckOutDate + CheckOutTime
                var rentToUpdate = db.Rent.FirstOrDefault(r => r.RelID == invoiceToFinalize.RelID);
                if (rentToUpdate != null)
                {
                    rentToUpdate.CheckOutDate = DateTime.Now.Date;
                    rentToUpdate.CheckOutTime = DateTime.Now.TimeOfDay;
                }

                // 4. Cập nhật trạng thái phòng thành Cleaning
                var roomToUpdate = db.Room.FirstOrDefault(r => r.RID == this.CurrentRoomId);
                if (roomToUpdate != null)
                {
                    roomToUpdate.RStatus = "Cleaning";
                }

                try
                {
                    db.SaveChanges();
                    Debug.WriteLine($"[PerformCheckout] Checkout thành công cho IID={invoiceToFinalize.IID}.");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[PerformCheckout] Lỗi khi lưu CSDL: {ex.Message}");
                    MessageBox.Show($"Lỗi khi lưu thông tin checkout: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        /// <summary>
        /// Nếu muốn đổi trạng thái phòng thành "Cleaning" riêng lẻ (không chạy PerformCheckout),
        /// bạn có thể gọi phương thức này.
        /// </summary>
        /// <returns></returns>
        public bool SetRoomStatusToCleaning()
        {
            if (this.CurrentRoomId == 0)
            {
                Debug.WriteLine("[SetRoomStatus] CurrentRoomId = 0.");
                return false;
            }

            using (var db = new AppDbContext())
            {
                var room = db.Room.FirstOrDefault(r => r.RID == this.CurrentRoomId);
                if (room == null)
                {
                    Debug.WriteLine($"[SetRoomStatus] Không tìm thấy Room RID={this.CurrentRoomId}");
                    return false;
                }
                room.RStatus = "Cleaning";
                try
                {
                    db.SaveChanges();
                    Debug.WriteLine($"[SetRoomStatus] Cập nhật RStatus=Cleaning cho RID={this.CurrentRoomId}.");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SetRoomStatus] Lỗi lưu CSDL: {ex.Message}");
                    return false;
                }
            }
        }

        // ============================
        // 4. Các hàm nội bộ để load & lưu ServiceUsage
        // ============================

        private void SaveDelayTimer_Tick(object sender, EventArgs e)
        {
            saveDelayTimer.Stop();
            if (lastChangedService != null)
                SaveOrUpdateServiceUsage(lastChangedService);
        }

        private void LoadServicesFromDatabase()
        {
            using (var db = new AppDbContext())
            {
                var serviceEntities = db.Service.ToList();

                foreach (var s in serviceEntities)
                {
                    var item = new ServiceModel
                    {
                        ServiceID = s.SID,
                        ServiceName = s.SName,
                        Unit = s.SUnit,
                        UnitPrice = s.SPrice,
                        Quantity = 0
                    };

                    item.PropertyChanged += Service_PropertyChanged;
                    Services.Add(item);
                }

                // Tạo roomItemModel (ServiceModel đại diện cho phí phòng)
                roomItemModel = new ServiceModel
                {
                    ServiceID = -999,
                    ServiceName = $"Room Type: {roomType}",
                    Unit = "fixed",
                    UnitPrice = roomPrice,
                    Quantity = 1
                };
                roomItemModel.PropertyChanged += Service_PropertyChanged;
                Services.Insert(0, roomItemModel);
            }
        }

        private void LoadExistingServiceUsages()
        {
            using (var db = new AppDbContext())
            {
                var usages = db.ServiceUsage
                    .Where(s => s.CID == CurrentCustomerId && s.IID == CurrentInvoiceId)
                    .ToList();

                foreach (var usage in usages)
                {
                    var matched = Services.FirstOrDefault(s => s.ServiceID == usage.SID);
                    if (matched != null)
                    {
                        matched.Quantity = usage.Quantity;
                    }
                }
            }
        }

        private void Service_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ServiceModel.Quantity))
            {
                lastChangedService = sender as ServiceModel;
                saveDelayTimer.Stop();
                saveDelayTimer.Start();

                FilterServices();
                CalculateTotalPrice();
            }
        }

        private void SaveOrUpdateServiceUsage(ServiceModel service)
        {
            if (service.ServiceID == -999) return;

            if (CurrentCustomerId == 0 || CurrentInvoiceId == 0)
            {
                Debug.WriteLine("[Save] Bỏ qua vì CID hoặc IID = 0");
                return;
            }

            Debug.WriteLine($"[Save] {service.ServiceName}: Qty={service.Quantity}, CID={CurrentCustomerId}, IID={CurrentInvoiceId}");

            using (var db = new AppDbContext())
            {
                var existing = db.ServiceUsage.FirstOrDefault(s =>
                    s.CID == CurrentCustomerId &&
                    s.IID == CurrentInvoiceId &&
                    s.SID == service.ServiceID);

                if (existing != null)
                {
                    if (service.Quantity == 0)
                        db.ServiceUsage.Remove(existing);
                    else
                    {
                        existing.Quantity = service.Quantity;
                        existing.ServiceTotal = service.TotalAmount;
                        db.ServiceUsage.Update(existing);
                    }
                }
                else if (service.Quantity > 0)
                {
                    db.ServiceUsage.Add(new ServiceUsage
                    {
                        SID = service.ServiceID,
                        CID = CurrentCustomerId,
                        IID = CurrentInvoiceId,
                        Quantity = service.Quantity,
                        ServiceTotal = service.TotalAmount
                    });
                }

                db.SaveChanges();
            }
            CalculateTotalPrice();
        }

        public void FilterServices()
        {
            var cvs = new CollectionViewSource { Source = Services };
            cvs.Filter += (s, e) =>
            {
                if (e.Item is ServiceModel service)
                    e.Accepted = service.Quantity > 0 || service.ServiceID == -999;
            };
            FilteredServices = cvs.View;
        }

        public void CalculateTotalPrice()
        {
            decimal total = Services
                .Where(s => s.Quantity > 0 || s.ServiceID == -999)
                .Sum(s => s.TotalAmount);

            TotalPrice = total;
        }

        // ============================
        // 5. INotifyPropertyChanged
        // ============================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
