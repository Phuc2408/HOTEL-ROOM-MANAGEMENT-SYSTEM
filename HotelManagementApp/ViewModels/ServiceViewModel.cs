using HotelManagementApp.Database;
using HotelManagementApp.Models;
using HotelManagementApp.Views.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using HotelManagementApp;

namespace HotelManagementApp.ViewModels
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        private AppDbContext _context;
        private ObservableCollection<Service> _services;
        private Service _selectedService;

        public ObservableCollection<Service> Services
        {
            get => _services;
            set
            {
                _services = value;
                OnPropertyChanged();
            }
        }

        public Service SelectedService
        {
            get => _selectedService;
            set
            {
                _selectedService = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }

        public ServiceViewModel()
        {
            _context = new AppDbContext();
            LoadServices();

            DeleteCommand = new RelayCommand<object>(
                execute: (param) => DeleteSelectedService(),
                canExecute: (param) => SelectedService != null
            );

            EditCommand = new RelayCommand<object>(
                execute: (param) => EditSelectedService(),
                canExecute: (param) => SelectedService != null
            );
        }

        public void LoadServices()
        {
            var serviceList = _context.Service.ToList();
            Services = new ObservableCollection<Service>(serviceList);
        }

        private void DeleteSelectedService()
        {
            if (SelectedService == null)
            {
                MessageBox.Show("Please select a service to delete.");
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the service \"{SelectedService.SName}\"?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var serviceToDelete = _context.Service.FirstOrDefault(s => s.SID == SelectedService.SID);
                    if (serviceToDelete != null)
                    {
                        _context.Service.Remove(serviceToDelete);
                        _context.SaveChanges();

                        Services.Remove(SelectedService);
                        SelectedService = null;

                        MessageBox.Show("Service deleted successfully.");
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Error deleting service: {ex.Message}", "Error Details", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditSelectedService()
        {
            if (SelectedService == null) return;

            var dialog = new EditServiceDialog(SelectedService);
            var result = dialog.ShowDialog();

            if (result == true)
            {
                var updated = dialog.EditedService;

                try
                {
                    var serviceInDb = _context.Service.FirstOrDefault(s => s.SID == updated.SID);
                    if (serviceInDb != null)
                    {
                        serviceInDb.SName = updated.SName;
                        serviceInDb.SUnit = updated.SUnit;
                        serviceInDb.SPrice = updated.SPrice;

                        _context.SaveChanges();
                        LoadServices();
                        MessageBox.Show("Service updated successfully.");
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Error updating service: {ex.Message}", "Error Details", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
