using HotelManagementApp.Database;
using HotelManagementApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        private DispatcherTimer saveDelayTimer;
        private ServiceModel lastChangedService;

        private decimal roomPrice;
        private string roomType;
        private ServiceModel roomItemModel;

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
            using (var db = new AppDbContext())
            {
                var rent = db.Rent.FirstOrDefault(r => r.RID == roomId);
                if (rent == null) return;

                var invoice = db.Invoice.FirstOrDefault(i => i.RelID == rent.RelID);
                if (invoice == null) return;

                var room = db.Room.FirstOrDefault(r => r.RID == roomId);
                if (room == null) return;

                roomPrice = room.RPrice;
                roomType = room.RType;

                CurrentCustomerId = rent.CID;
                CurrentInvoiceId = invoice.IID;
            }

            LoadServicesFromDatabase();
            LoadExistingServiceUsages();
            FilterServices();
            CalculateTotalPrice();
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
