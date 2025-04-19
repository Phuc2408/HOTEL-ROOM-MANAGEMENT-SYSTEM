using HotelManagementApp.ViewModels;
using System.ComponentModel;

namespace HotelManagementApp.Models
{
    public class ServiceModel : INotifyPropertyChanged
    {
        private string serviceID;
        public string ServiceID
        {
            get { return serviceID; }
            set
            {
                if (serviceID != value)
                {
                    serviceID = value;
                    OnPropertyChanged(nameof(ServiceID));
                }
            }
        }

        private string serviceName;
        public string ServiceName
        {
            get { return serviceName; }
            set
            {
                if (serviceName != value)
                {
                    serviceName = value;
                    OnPropertyChanged(nameof(ServiceName));
                }
            }
        }

        private string unit;
        public string Unit
        {
            get { return unit; }
            set
            {
                if (unit != value)
                {
                    unit = value;
                    OnPropertyChanged(nameof(Unit));
                }
            }
        }

        private decimal unitPrice;
        public decimal UnitPrice
        {
            get { return unitPrice; }
            set
            {
                if (unitPrice != value)
                {
                    unitPrice = value;
                    OnPropertyChanged(nameof(UnitPrice));
                    CalculateTotal();  // Recalculate total when UnitPrice changes
                }
            }
        }

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));  // Notify that Quantity changed
                    CalculateTotal(); // Recalculate total when Quantity changes
                    OnPropertyChanged(nameof(TotalAmount)); // Notify that TotalAmount changed

                    // Notify the ViewModel to update the filtered services when Quantity changes
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilteredServices"));
                }
            }
        }
        private decimal totalAmount;
        public decimal TotalAmount
        {
            get { return totalAmount; }
            private set
            {
                if (totalAmount != value)
                {
                    totalAmount = value;
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        // Tính toán tổng số tiền dựa trên UnitPrice và Quantity
        private void CalculateTotal()
        {
            TotalAmount = UnitPrice * Quantity;  // Tính tổng tiền cho mỗi dịch vụ
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
