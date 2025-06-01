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
                // 1. Lấy thông tin Room (riêng biệt)
                var roomDetails = db.Room.FirstOrDefault(r => r.RID == roomId);
                if (roomDetails == null)
                {
                    Debug.WriteLine($"[CheckOut Init] Lỗi: Không tìm thấy thông tin phòng RID: {roomId}.");
                    MessageBox.Show("Lỗi: Không tìm thấy thông tin phòng để checkout.", "Lỗi Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                this.RoomNumber = roomDetails.RID.ToString();
                OnPropertyChanged(nameof(RoomNumber));

                this.roomPrice = roomDetails.RPrice;
                this.roomType = roomDetails.RType;

                // 2. Tìm Rent active (isDone = false)
                var currentActiveRent = db.Rent
                    .Where(r => r.RID == roomId && r.isDone == false)
                    .OrderByDescending(r => r.CheckInDate)
                    .FirstOrDefault();

                if (currentActiveRent == null)
                {
                    Debug.WriteLine($"[CheckOut Init] KHÔNG TÌM THẤY RENT ACTIVE cho Room ID: {roomId}.");
                    MessageBox.Show("Phòng này không có lượt thuê đang hoạt động (chưa thanh toán) để checkout.",
                                    "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Chỉ load dịch vụ (roomItemModel + services, Q=0)
                    LoadServicesFromDatabase();
                    FilterServices();
                    CalculateTotalPrice();
                    return;
                }

                // Lưu RelID và CustomerId để dùng về sau
                this.activeRelID_forCheckout = currentActiveRent.RelID;
                this.CurrentCustomerId = currentActiveRent.CID;

                // 3. Load thông tin Customer (để hiển thị GuestName, IdCard, Phone, Country)
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

                // 4. Gán ngàу và số người
                this.PeopleCount = currentActiveRent.NumberOfPeople;
                this.CheckInDate = currentActiveRent.CheckInDate;
                this.CheckOutDate = currentActiveRent.CheckOutDate;
                OnPropertyChanged(nameof(PeopleCount));
                OnPropertyChanged(nameof(CheckInDate));
                OnPropertyChanged(nameof(CheckOutDate));

                // 5. Kiểm tra Invoice đã tồn tại chưa
                var invoiceRecord = db.Invoice.FirstOrDefault(i => i.RelID == this.activeRelID_forCheckout);
                if (invoiceRecord == null)
                {
                    Debug.WriteLine($"[CheckOut Init] Tạo Invoice nháp cho RelID: {this.activeRelID_forCheckout}");
                    invoiceRecord = new Invoice
                    {
                        CID = this.CurrentCustomerId,
                        RelID = this.activeRelID_forCheckout,
                        IDate = DateTime.Now.Date,
                        RoomTotal = this.roomPrice,
                        ServiceTotal = 0,
                        Total = this.roomPrice,
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
                        MessageBox.Show("Lỗi khi khởi tạo thông tin thanh toán. Vui lòng thử lại.",
                                        "Lỗi Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    Debug.WriteLine($"[CheckOut Init] Tải Invoice (IID={invoiceRecord.IID}) đã tồn tại cho RelID: {this.activeRelID_forCheckout}");
                    this.CurrentInvoiceId = invoiceRecord.IID;
                    // Lưu ý: Không gọi LoadExistingServiceUsages ở đây vì Services chưa được tạo
                }
            } // Kết thúc using AppDbContext()

            // 6. BƯỚC QUAN TRỌNG: Load toàn bộ ServiceModel (roomItemModel + dịch vụ)
            LoadServicesFromDatabase();

            // 7. Nếu đã tồn tại Invoice (CurrentInvoiceId != 0), gán Quantity cho các dịch vụ cũ
            if (this.CurrentInvoiceId != 0)
            {
                LoadExistingServiceUsages();
            }

            // 8. Lọc ra những ServiceModel có Quantity > 0 (và luôn giữ roomItemModel)
            FilterServices();

            // 9. Tính tổng giá
            CalculateTotalPrice();

            // 10. Cho phép nút Confirm khi Rent active (activeRelID_forCheckout != 0)
            this.CanPerformCheckout = (this.activeRelID_forCheckout != 0);
            OnPropertyChanged(nameof(CanPerformCheckout));
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
                // 1. Tìm và cập nhật Room
                var room = db.Room.FirstOrDefault(r => r.RID == this.CurrentRoomId);
                if (room == null)
                {
                    Debug.WriteLine($"[SetRoomStatus] Không tìm thấy Room RID={this.CurrentRoomId}");
                    return false;
                }
                room.RStatus = "cleaning";

                // 2. Tìm Rent active (isDone == false) gần nhất cho phòng này
                var activeRent = db.Rent
                    .Where(r => r.RID == this.CurrentRoomId && r.isDone == false)
                    .OrderByDescending(r => r.CheckInDate)
                    .FirstOrDefault();

                if (activeRent != null)
                {
                    activeRent.isDone = true;
                    activeRent.CheckOutDate = DateTime.Now.Date; // nếu muốn lưu ngày checkout
                }
                else
                {
                    Debug.WriteLine($"[SetRoomStatus] Không tìm thấy Rent active cho RID={this.CurrentRoomId}.");
                }

                try
                {
                    db.SaveChanges();
                    Debug.WriteLine($"[SetRoomStatus] Cập nhật RStatus='cleaning' và Rent.isDone=true cho RID={this.CurrentRoomId}.");
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

                // ✅ Cập nhật lại Invoice.ServiceTotal và Invoice.Total
                var invoice = db.Invoice.FirstOrDefault(i => i.IID == CurrentInvoiceId);
                if (invoice != null)
                {
                    invoice.ServiceTotal = db.ServiceUsage
                        .Where(su => su.IID == CurrentInvoiceId)
                        .Sum(su => su.ServiceTotal);

                    invoice.Total = invoice.RoomTotal + invoice.ServiceTotal;
                    db.Invoice.Update(invoice);
                    db.SaveChanges();
                }
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
