using HotelManagementApp.Database;
using HotelManagementApp.Models;
using HotelManagementApp.Views.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

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
                MessageBox.Show("Vui lòng chọn một dịch vụ để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 1. Kiểm tra xem có bất kỳ hóa đơn / ServiceUsage nào đang tham chiếu đến dịch vụ này không
            bool isUsed = _context.ServiceUsage.Any(su => su.SID == SelectedService.SID);
            if (isUsed)
            {
                MessageBox.Show(
                    $"Không thể xóa dịch vụ \"{SelectedService.SName}\" vì đã có hóa đơn hoặc phiếu sử dụng dịch vụ liên quan.",
                    "Không thể xóa",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // 2. Confirm xóa
            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa dịch vụ \"{SelectedService.SName}\"?",
                "Xác nhận xóa",
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

                        MessageBox.Show("Xóa dịch vụ thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa dịch vụ: {ex.Message}", "Chi tiết lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditSelectedService()
        {
            if (SelectedService == null) return;

            // 1. Kiểm tra xem có bất kỳ hóa đơn / ServiceUsage nào đang tham chiếu đến dịch vụ này không
            bool isUsed = _context.ServiceUsage.Any(su => su.SID == SelectedService.SID);
            if (isUsed)
            {
                // Lấy giá trị cũ trước khi mở dialog
                string originalName = SelectedService.SName;
                string originalUnit = SelectedService.SUnit;
                decimal originalPrice = SelectedService.SPrice;

                // Thông báo nhắc người dùng: chỉ được đổi tên
                MessageBox.Show(
                    $"Dịch vụ \"{originalName}\" đang có hóa đơn liên quan.\n" +
                    "Bạn chỉ được phép sửa tên, hệ thống sẽ giữ nguyên giá và đơn vị tính.",
                    "Chỉ được sửa tên", MessageBoxButton.OK, MessageBoxImage.Information
                );

                // Tạo một bản sao tạm để đẩy vào dialog, nhưng chúng ta chỉ lấy SName
                var tempService = new Service
                {
                    SID = SelectedService.SID,
                    SName = originalName,
                    SUnit = originalUnit,
                    SPrice = originalPrice
                };

                var dialog = new EditServiceDialog(tempService);
                var result = dialog.ShowDialog();

                if (result == true)
                {
                    var updated = dialog.EditedService; // chứa SName, SUnit, SPrice người dùng nhập

                    try
                    {
                        // Tìm bản thật trong DB
                        var serviceInDb = _context.Service.FirstOrDefault(s => s.SID == updated.SID);
                        if (serviceInDb != null)
                        {
                            // Chỉ cập nhật tên mới, giữ nguyên đơn vị và giá cũ
                            serviceInDb.SName = updated.SName;
                            serviceInDb.SUnit = originalUnit;
                            serviceInDb.SPrice = originalPrice;

                            _context.SaveChanges();
                            LoadServices();
                            MessageBox.Show("Cập nhật tên dịch vụ thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi cập nhật dịch vụ: {ex.Message}", "Chi tiết lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                return;
            }

            // Nếu dịch vụ chưa được dùng, cho phép sửa toàn bộ (tên, đơn vị, giá)
            var fullDialog = new EditServiceDialog(SelectedService);
            var fullResult = fullDialog.ShowDialog();

            if (fullResult == true)
            {
                var updated = fullDialog.EditedService;
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
                        MessageBox.Show("Cập nhật dịch vụ thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật dịch vụ: {ex.Message}", "Chi tiết lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
