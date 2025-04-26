using HotelManagementApp.Database;
using HotelManagementApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HotelManagementApp.ViewModels
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        private AppDbContext _context;
        private ObservableCollection<Service> _services;

        public ObservableCollection<Service> Services
        {
            get => _services;
            set
            {
                _services = value;
                OnPropertyChanged();
            }
        }

        public ServiceViewModel()
        {
            _context = new AppDbContext();
            LoadServices();
        }

        private void LoadServices()
        {
            var serviceList = _context.Service.ToList(); // Query thẳng Service từ DB
            Services = new ObservableCollection<Service>(serviceList); // Gán vô ObservableCollection
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
