using System.ComponentModel;

namespace HotelManagementApp.Models
{
    public class ServiceModel : INotifyPropertyChanged
    {
        private int serviceID;
        public int ServiceID
        {
            get => serviceID;
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
            get => serviceName;
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
            get => unit;
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
            get => unitPrice;
            set
            {
                if (unitPrice != value)
                {
                    unitPrice = value;
                    OnPropertyChanged(nameof(UnitPrice));
                    CalculateTotal();  // Gọi lại khi giá thay đổi
                }
            }
        }

        private int quantity;
        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    CalculateTotal(); // Gọi lại khi số lượng thay đổi
                }
            }
        }

        private decimal totalAmount;
        public decimal TotalAmount
        {
            get => totalAmount;
            private set
            {
                if (totalAmount != value)
                {
                    totalAmount = value;
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        // Tính lại tổng tiền
        private void CalculateTotal()
        {
            TotalAmount = UnitPrice * Quantity;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
