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
        private string _guestName, _idCard, _phoneNumber, _selectedCountry, _roomNumber;
        private int _peopleCount;
        private DateTime? _checkInDate, _checkOutDate;
        private ICollectionView filteredServices;
        private decimal totalPrice;
        private bool _canPerformCheckout;

        public string GuestName { get => _guestName; set { _guestName = value; OnPropertyChanged(nameof(GuestName)); } }
        public string IdCard { get => _idCard; set { _idCard = value; OnPropertyChanged(nameof(IdCard)); } }
        public string PhoneNumber { get => _phoneNumber; set { _phoneNumber = value; OnPropertyChanged(nameof(PhoneNumber)); } }
        public string SelectedCountry { get => _selectedCountry; set { _selectedCountry = value; OnPropertyChanged(nameof(SelectedCountry)); } }
        public string RoomNumber { get => _roomNumber; set { _roomNumber = value; OnPropertyChanged(nameof(RoomNumber)); } }
        public int PeopleCount { get => _peopleCount; set { _peopleCount = value; OnPropertyChanged(nameof(PeopleCount)); } }
        public DateTime? CheckInDate { get => _checkInDate; set { _checkInDate = value; OnPropertyChanged(nameof(CheckInDate)); } }
        public DateTime? CheckOutDate { get => _checkOutDate; set { _checkOutDate = value; OnPropertyChanged(nameof(CheckOutDate)); } }

        public ObservableCollection<ServiceModel> Services { get; set; } = new();
        public ICollectionView FilteredServices { get => filteredServices; set { filteredServices = value; OnPropertyChanged(nameof(FilteredServices)); } }
        public decimal TotalPrice { get => totalPrice; set { totalPrice = value; OnPropertyChanged(nameof(TotalPrice)); } }

        public int CurrentCustomerId { get; set; }
        public int CurrentInvoiceId { get; set; }
        public int CurrentRoomId { get; set; }
        public bool CanPerformCheckout { get => _canPerformCheckout; private set { _canPerformCheckout = value; OnPropertyChanged(nameof(CanPerformCheckout)); } }

        private DispatcherTimer saveDelayTimer;
        private ServiceModel lastChangedService;
        private decimal roomPrice;
        private string roomType;
        private ServiceModel roomItemModel;
        private int activeReID_forCheckout;
        private Dictionary<int, int> OriginalQuantities = new();

        public CheckOutDialogViewModel()
        {
            saveDelayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(600) };
            saveDelayTimer.Tick += SaveDelayTimer_Tick;
        }

        public void InitializeByRoomId(int roomId)
        {
            CurrentRoomId = roomId;
            Services.Clear();
            activeReID_forCheckout = 0;
            CurrentCustomerId = 0;
            CurrentInvoiceId = 0;
            TotalPrice = 0;
            roomItemModel = null;
            CanPerformCheckout = false;

            using (var db = new AppDbContext())
            {
                var roomDetails = db.Room.FirstOrDefault(r => r.RID == roomId);
                if (roomDetails == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                RoomNumber = roomDetails.RID.ToString();
                roomPrice = roomDetails.RPrice;
                roomType = roomDetails.RType;

                var currentActiveRent = db.Rent
                    .Where(r => r.RID == roomId && r.isDone == false)
                    .OrderByDescending(r => r.CheckInDate)
                    .FirstOrDefault();

                if (currentActiveRent == null)
                {
                    MessageBox.Show("Phòng không có lượt thuê đang hoạt động.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadServicesFromDatabase();
                    FilterServices();
                    CalculateTotalPrice();
                    return;
                }

                activeReID_forCheckout = currentActiveRent.ReID;
                CurrentCustomerId = currentActiveRent.CID;

                var customer = db.Customer.FirstOrDefault(c => c.CID == currentActiveRent.CID);
                if (customer != null)
                {
                    GuestName = customer.CName;
                    IdCard = customer.CPersonalID;
                    PhoneNumber = customer.CPhone;
                    SelectedCountry = customer.CCountry;
                }

                PeopleCount = currentActiveRent.NumberOfPeople;
                CheckInDate = currentActiveRent.CheckInDate;
                CheckOutDate = DateTime.Now;

                DateTime now = DateTime.Now;
                int numberOfDays = (now.Date - currentActiveRent.CheckInDate.Date).Days;
                if (numberOfDays <= 0) numberOfDays = 1;

                TimeSpan defaultCheckoutTime = new TimeSpan(12, 0, 0);
                DateTime expectedCheckout = now.Date + defaultCheckoutTime;
                TimeSpan delay = now - expectedCheckout;

                int extraHours = 0;
                if (delay.TotalMinutes > 0)
                {
                    extraHours = (int)Math.Ceiling(delay.TotalHours);
                    if (extraHours > 6)
                    {
                        numberOfDays += 1;
                        extraHours = 0; // reset late checkout
                    }
                }

                var invoiceRecord = db.Invoice.FirstOrDefault(i => i.ReID == activeReID_forCheckout);
                if (invoiceRecord == null)
                {
                    invoiceRecord = new Invoice
                    {
                        ReID = activeReID_forCheckout,
                        IDate = now,
                        RoomTotal = roomPrice * numberOfDays,
                        ServiceTotal = 0,
                        Total = roomPrice * numberOfDays
                    };
                    db.Invoice.Add(invoiceRecord);
                }
                else
                {
                    invoiceRecord.RoomTotal = roomPrice * numberOfDays;
                    invoiceRecord.IDate = now;
                    db.Invoice.Update(invoiceRecord);
                }

                db.SaveChanges();
                CurrentInvoiceId = invoiceRecord.IID;

                LoadServicesFromDatabase();

                roomItemModel = new ServiceModel
                {
                    ServiceID = -999,
                    ServiceName = $"Room Type: {roomType}",
                    Unit = "day",
                    UnitPrice = roomPrice,
                    Quantity = numberOfDays,
                    IsReadOnly = true
                };
                roomItemModel.PropertyChanged += Service_PropertyChanged;
                Services.Insert(0, roomItemModel);
                SaveOrUpdateServiceUsage(roomItemModel);

                if (extraHours >= 0)
                {
                    var lateCheckout = Services.FirstOrDefault(s => s.ServiceName == "Late Checkout");
                    if (lateCheckout != null)
                    {
                        lateCheckout.Quantity = extraHours;
                        SaveOrUpdateServiceUsage(lateCheckout);
                    }
                }
                
                MarkSpecialServicesAsReadOnly();
                if (CurrentInvoiceId != 0)
                {
                    LoadExistingServiceUsages();
                }

                FilterServices();
                CalculateTotalPrice();
                CanPerformCheckout = (activeReID_forCheckout != 0);
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
            }
        }

        private void LoadExistingServiceUsages()
        {
            using (var db = new AppDbContext())
            {
                var usages = db.ServiceUsage
                    .Where(s => s.ReID == activeReID_forCheckout)
                    .ToList();

                foreach (var usage in usages)
                {
                    var matched = Services.FirstOrDefault(s => s.ServiceID == usage.SID);
                    if (matched != null)
                        matched.Quantity = usage.Quantity;
                }
            }
        }

        private void Service_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ServiceModel.Quantity))
            {
                var service = sender as ServiceModel;
                if (service == null) return;

                // Lấy giá trị gốc (đã cộng dồn) từ dictionary, mặc định 0 nếu chưa có
                int original = OriginalQuantities.TryGetValue(service.ServiceID, out var old) ? old : 0;

                // Giá trị user mới nhập (có thể dương hoặc âm)
                int inputValue = service.Quantity;

                // Tính newQuantity = original + inputValue (nếu inputValue âm => trừ bớt)
                int newQuantity = original + inputValue;

                // Nếu kết quả < 0 thì reset về 0
                if (newQuantity < 0)
                    newQuantity = 0;

                // Để tránh vòng lặp PropertyChanged, tạm unsubscribe rồi gán lại
                service.PropertyChanged -= Service_PropertyChanged;
                service.Quantity = newQuantity;
                service.PropertyChanged += Service_PropertyChanged;

                // Cập nhật lại giá trị cuối cùng vào dictionary
                OriginalQuantities[service.ServiceID] = newQuantity;

                // Đánh dấu service này để lưu chậm (saveDelayTimer)
                lastChangedService = service;
                saveDelayTimer.Stop();
                saveDelayTimer.Start();

                // Cập nhật lại danh sách lọc và tổng tiền
                FilterServices();
                CalculateTotalPrice();
            }
        }


        private void SaveOrUpdateServiceUsage(ServiceModel service)
        {
            if (service.ServiceID == -999) return;
            if (CurrentCustomerId == 0 || CurrentInvoiceId == 0) return;

            using (var db = new AppDbContext())
            {
                var existing = db.ServiceUsage.FirstOrDefault(s =>
                    s.ReID == activeReID_forCheckout && s.SID == service.ServiceID);

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
                        ReID = activeReID_forCheckout,
                        Quantity = service.Quantity,
                        ServiceTotal = service.TotalAmount
                    });
                }

                db.SaveChanges();

                var invoice = db.Invoice.FirstOrDefault(i => i.IID == CurrentInvoiceId);
                if (invoice != null)
                {
                    invoice.ServiceTotal = db.ServiceUsage
                        .Where(su => su.ReID == invoice.ReID)
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
                    e.Accepted = service.Quantity > 0 || service.ServiceID == -999 || service.ServiceName == "Late Checkout";
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
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void MarkSpecialServicesAsReadOnly()
        {
            foreach (var service in Services)
            {
                if (service.ServiceName == "Late Checkout")
                {
                    service.IsReadOnly = true;
                }
            }
        }

        // ============================
        // 6. Định nghĩa bổ sung bị thiếu: SetRoomStatusToCleaning
        // ============================
        public bool SetRoomStatusToCleaning()
        {
            if (this.CurrentRoomId == 0)
                return false;

            using (var db = new AppDbContext())
            {
                var room = db.Room.FirstOrDefault(r => r.RID == this.CurrentRoomId);
                if (room == null) return false;

                room.RStatus = "cleaning";

                var activeRent = db.Rent
                    .Where(r => r.RID == this.CurrentRoomId && r.isDone == false)
                    .OrderByDescending(r => r.CheckInDate)
                    .FirstOrDefault();

                if (activeRent != null)
                {
                    activeRent.isDone = true;
                    activeRent.CheckOutDate = DateTime.Now;
                }

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
