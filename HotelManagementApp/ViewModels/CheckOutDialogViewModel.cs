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

        public int CurrentCustomerId { get; set; }
        public int CurrentInvoiceId { get; set; }
        public int CurrentRoomId { get; set; } 

        private DispatcherTimer saveDelayTimer;
        private ServiceModel lastChangedService;

        private decimal roomPrice;
        private string roomType;
        private ServiceModel roomItemModel;
        // ===== KHAI BÁO BIẾN MỚI Ở ĐÂY =====
        private int activeRelID_forCheckout; // Trường để lưu RelID của Rent record đang được checkout

        private bool _canPerformCheckout; // Trường private cho thuộc tính CanPerformCheckout
        public bool CanPerformCheckout
        {
            get => _canPerformCheckout;
            private set // Nên là private set để chỉ ViewModel này quản lý
            {
                if (_canPerformCheckout != value)
                {
                    _canPerformCheckout = value;
                    OnPropertyChanged(nameof(CanPerformCheckout));
                }
            }
        }
        // ===== KẾT THÚC KHAI BÁO BIẾN MỚI =====
        public CheckOutDialogViewModel()
        {
            saveDelayTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(600)
            };
            saveDelayTimer.Tick += SaveDelayTimer_Tick;

            Services = new ObservableCollection<ServiceModel>();
        }

        public void InitializeByRoomId(int roomId)
        {
            this.CurrentRoomId = roomId;
            this.Services.Clear(); // Xóa dịch vụ từ lần mở trước
            this.activeRelID_forCheckout = 0;
            this.CurrentCustomerId = 0;
            this.CurrentInvoiceId = 0; // Mặc định là chưa có Invoice ID
            this.TotalPrice = 0;
            this.roomItemModel = null;
            this.CanPerformCheckout = false;

            using (var db = new AppDbContext())
            {
                var roomDetails = db.Room.FirstOrDefault(r => r.RID == roomId);
                if (roomDetails == null)
                {
                    Debug.WriteLine($"[CheckOut Init] Lỗi: Không tìm thấy thông tin phòng RID: {roomId}.");
                    MessageBox.Show("Lỗi: Không tìm thấy thông tin phòng để checkout.", "Lỗi Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                    OnPropertyChanged(nameof(CanPerformCheckout)); // Cập nhật trạng thái nút nếu có binding
                    return;
                }
                this.roomPrice = roomDetails.RPrice; // Gán cho biến instance để LoadServicesFromDatabase sử dụng
                this.roomType = roomDetails.RType;   // Gán cho biến instance

                // 1. Tìm bản ghi Rent đang active cho phòng này (chưa có hóa đơn)
                var currentActiveRent = db.Rent
                    .Where(r => r.RID == roomId && !db.Invoice.Any(inv => inv.RelID == r.RelID))
                    .OrderByDescending(r => r.CheckInDate)
                    .FirstOrDefault();

                if (currentActiveRent == null)
                {
                    Debug.WriteLine($"[CheckOut Init] KHÔNG TÌM THẤY LƯỢT THUÊ ACTIVE (chưa có hóa đơn) cho Phòng ID: {roomId}.");
                    MessageBox.Show("Phòng này không có thông tin thuê đang hoạt động (chưa thanh toán) để checkout.", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Load services cơ bản và room item với giá trị 0 (nếu vẫn muốn hiển thị cấu trúc dialog)
                    LoadServicesFromDatabase(); // roomItemModel.Quantity sẽ là 0 do activeRelID_forCheckout = 0
                    FilterServices();
                    CalculateTotalPrice();
                    OnPropertyChanged(nameof(CanPerformCheckout));
                    return;
                }

                // Lưu lại thông tin quan trọng của lượt Rent sẽ được checkout
                this.activeRelID_forCheckout = currentActiveRent.RelID;
                this.CurrentCustomerId = currentActiveRent.CID;

                // 2. Kiểm tra xem có hóa đơn nào (do lỗi logic/đồng bộ trước đó) đã tồn tại cho Rent này không.
                // Theo định nghĩa currentActiveRent thì không nên có.
                var invoiceRecord = db.Invoice.FirstOrDefault(i => i.RelID == this.activeRelID_forCheckout);

                if (invoiceRecord == null)
                {
                    // CHƯA CÓ HÓA ĐƠN => TẠO MỘT HÓA ĐƠN NHÁP NGAY BÂY GIỜ
                    // để ServiceUsage có thể được lưu với IID này trong quá trình người dùng thao tác trên dialog.
                    Debug.WriteLine($"[CheckOut Init] Chưa có hóa đơn cho lượt thuê active (RelID: {this.activeRelID_forCheckout}). Tạo hóa đơn nháp.");
                    invoiceRecord = new Invoice
                    {
                        CID = this.CurrentCustomerId, // Lấy từ currentActiveRent
                        RelID = this.activeRelID_forCheckout,
                        IDate = DateTime.Now.Date, // Ngày tạo hóa đơn nháp
                        RoomTotal = this.roomPrice, // Tiền phòng ban đầu
                        ServiceTotal = 0,
                        Total = this.roomPrice,
                        // Bạn có thể thêm một trường Status cho Invoice, ví dụ: "Draft"
                        // Status = "Draft"
                    };
                    db.Invoice.Add(invoiceRecord);
                    try
                    {
                        db.SaveChanges(); // Lưu hóa đơn nháp để lấy IID
                        this.CurrentInvoiceId = invoiceRecord.IID; // Lấy IID của hóa đơn vừa tạo
                        Debug.WriteLine($"[CheckOut Init] Hóa đơn nháp (IID: {this.CurrentInvoiceId}) đã được tạo cho RelID: {this.activeRelID_forCheckout}.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[CheckOut Init] Lỗi khi tạo hóa đơn nháp: {ex.Message}\n{ex.StackTrace}");
                        MessageBox.Show("Lỗi khi khởi tạo thông tin thanh toán. Vui lòng thử lại.", "Lỗi Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
                        OnPropertyChanged(nameof(CanPerformCheckout));
                        return; // Không thể tiếp tục nếu không tạo được hóa đơn nháp
                    }
                }
                else
                {
                    // Đã có hóa đơn (trường hợp bất thường hoặc đang chỉnh sửa hóa đơn đã có từ trước)
                    Debug.WriteLine($"[CheckOut Init] Đã tìm thấy hóa đơn (IID: {invoiceRecord.IID}) cho lượt thuê (RelID: {this.activeRelID_forCheckout}). Sẽ tải hóa đơn này.");
                    this.CurrentInvoiceId = invoiceRecord.IID;
                    LoadExistingServiceUsages(); // Tải các dịch vụ đã dùng cho hóa đơn này
                }
            } // Kết thúc using (var db = new AppDbContext())

            // Load tất cả các loại dịch vụ và tạo roomItemModel
            // LoadServicesFromDatabase nên được gọi sau khi this.roomPrice và this.roomType đã có giá trị
            LoadServicesFromDatabase();

            // Nếu CurrentInvoiceId != 0 (tức là invoice đã tồn tại HOẶC vừa được tạo nháp và có IID),
            // LoadExistingServiceUsages có thể được gọi (nếu chưa gọi ở trên).
            // Tuy nhiên, nếu là hóa đơn nháp mới tạo, nó sẽ không có service usage nào.
            // Việc gọi LoadExistingServiceUsages sau khi tạo nháp chủ yếu để đảm bảo code nhất quán.

            FilterServices();
            CalculateTotalPrice();
            this.CanPerformCheckout = (this.activeRelID_forCheckout != 0); // Cho phép checkout nếu có active rent
            OnPropertyChanged(nameof(CanPerformCheckout));
        }
        // Trong CheckOutDialogViewModel.cs
        public bool PerformCheckout()
        {
            if (!CanPerformCheckout || this.activeRelID_forCheckout == 0 || this.CurrentInvoiceId == 0)
            {
                Debug.WriteLine("[PerformCheckout] Checkout không thể thực hiện: thiếu thông tin Rent hoặc Invoice ID.");
                MessageBox.Show("Không thể thực hiện checkout. Thông tin khởi tạo không đầy đủ.", "Lỗi Checkout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (saveDelayTimer.IsEnabled)
            {
                saveDelayTimer.Stop();
                if (lastChangedService != null) SaveOrUpdateServiceUsage(lastChangedService);
            }
            CalculateTotalPrice();

            using (var db = new AppDbContext())
            {
                // Tìm hóa đơn đã được tạo nháp (hoặc đã có) để hoàn tất
                var invoiceToFinalize = db.Invoice.FirstOrDefault(i => i.IID == this.CurrentInvoiceId);

                if (invoiceToFinalize == null)
                {
                    Debug.WriteLine($"[PerformCheckout] Lỗi nghiêm trọng: Không tìm thấy hóa đơn nháp với IID: {this.CurrentInvoiceId}.");
                    MessageBox.Show("Lỗi: Không tìm thấy thông tin hóa đơn để hoàn tất checkout.", "Lỗi Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // Cập nhật hóa đơn với thông tin cuối cùng
                invoiceToFinalize.RoomTotal = this.roomItemModel?.TotalAmount ?? 0;
                invoiceToFinalize.ServiceTotal = Math.Max(0, this.TotalPrice - (this.roomItemModel?.TotalAmount ?? 0));
                invoiceToFinalize.Total = this.TotalPrice;
                // Nếu có trường Status trong Invoice:
                // invoiceToFinalize.Status = "Finalized";

                var rentToUpdate = db.Rent.FirstOrDefault(r => r.RelID == invoiceToFinalize.RelID); // Lấy Rent từ RelID của Invoice
                if (rentToUpdate != null)
                {
                    rentToUpdate.CheckOutDate = DateTime.Now.Date;
                    rentToUpdate.CheckOutTime = DateTime.Now.TimeOfDay;
                }

                var roomToUpdate = db.Room.FirstOrDefault(r => r.RID == this.CurrentRoomId); // Hoặc rentToUpdate.RID
                if (roomToUpdate != null)
                {
                    roomToUpdate.RStatus = "Cleaning";
                }

                try
                {
                    db.SaveChanges();
                    Debug.WriteLine($"[PerformCheckout] Checkout thành công. Hóa đơn (IID: {invoiceToFinalize.IID}) đã được hoàn tất/cập nhật.");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[PerformCheckout] Lỗi khi lưu các thay đổi checkout: {ex.Message}\n{ex.StackTrace}");
                    MessageBox.Show($"Lỗi khi lưu thông tin checkout: {ex.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
        public bool SetRoomStatusToCleaning()
        {
            if (this.CurrentRoomId == 0)
            {
                Debug.WriteLine("[SetRoomStatus] Lỗi: CurrentRoomId chưa được thiết lập.");
                // Có thể throw exception hoặc thông báo lỗi cụ thể hơn nếu cần
                return false;
            }

            using (var db = new AppDbContext())
            {
                var room = db.Room.FirstOrDefault(r => r.RID == this.CurrentRoomId);
                if (room == null)
                {
                    Debug.WriteLine($"[SetRoomStatus] Lỗi: Không tìm thấy phòng với RID: {this.CurrentRoomId}.");
                    return false;
                }
                room.RStatus = "cleaning";

                try
                {
                    db.SaveChanges();
                    Debug.WriteLine($"[SetRoomStatus] Đã cập nhật thành công trạng thái phòng RID: {this.CurrentRoomId} thành 'Cleaning'.");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SetRoomStatus] Lỗi khi lưu thay đổi trạng thái phòng cho RID {this.CurrentRoomId}: {ex.Message}");
                    // Ghi log chi tiết lỗi ex
                    return false;
                }
            }
        }
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
                Debug.WriteLine("[Save] Skipped because CID or IID = 0");
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
