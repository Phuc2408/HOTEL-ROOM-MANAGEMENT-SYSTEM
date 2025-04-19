using HotelManagementApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace HotelManagementApp.ViewModels
{
    public class CheckOutDialogViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ServiceModel> services;
        public ObservableCollection<ServiceModel> Services
        {
            get { return services; }
            set
            {
                if (services != value)
                {
                    services = value;
                    OnPropertyChanged(nameof(Services));
                    FilterServices();  // Reapply the filter when Services collection changes
                }
            }
        }

        private ICollectionView filteredServices;
        public ICollectionView FilteredServices
        {
            get { return filteredServices; }
            set
            {
                if (filteredServices != value)
                {
                    filteredServices = value;
                    OnPropertyChanged(nameof(FilteredServices));
                }
            }
        }

        private decimal totalAmount;
        public decimal TotalAmount
        {
            get { return totalAmount; }
            set
            {
                if (totalAmount != value)
                {
                    totalAmount = value;
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        public CheckOutDialogViewModel()
        {
            // Example data for services
            Services = new ObservableCollection<ServiceModel>
        {
            new ServiceModel { ServiceID = "1", ServiceName = "Extra Bed", Unit = "Piece", UnitPrice = 20, Quantity = 1 },
            new ServiceModel { ServiceID = "2", ServiceName = "Breakfast", Unit = "Set", UnitPrice = 10, Quantity = 0 },
            new ServiceModel { ServiceID = "3", ServiceName = "Laundry", Unit = "Piece", UnitPrice = 5, Quantity = 0 }
        };

            // Initialize filteredServices and filter
            FilterServices();
            CalculateTotalAmount();
        }

        // Method to filter services based on Quantity > 0
        public void FilterServices()
        {
            var collectionViewSource = new CollectionViewSource();
            collectionViewSource.Source = Services;

            // Subscribe to the Filter event
            collectionViewSource.Filter += (sender, args) =>
            {
                var service = args.Item as ServiceModel;
                if (service != null)
                {
                    args.Accepted = service.Quantity > 0; // Only include services with Quantity > 0
                }
            };

            // Update FilteredServices collection
            FilteredServices = collectionViewSource.View;
        }

        // Method to calculate total amount for all services
        public void CalculateTotalAmount()
        {
            decimal total = 0;
            foreach (var service in FilteredServices)
            {
                var serviceModel = service as ServiceModel;
                if (serviceModel != null)
                {
                    total += serviceModel.TotalAmount; // Sum the TotalAmount of each service
                }
            }
            TotalAmount = total;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
