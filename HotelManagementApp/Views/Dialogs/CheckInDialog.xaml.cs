using HotelManagementApp.Database;
using HotelManagementApp.Helpers;
using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;
using System;
using System.Linq;
using System.Windows;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class CheckInDialog : Window
    {
        public CheckInDialog(string roomId)
        {
            InitializeComponent();

            var vm = new CheckInDialogViewModel();
            vm.RoomId = roomId;
            this.DataContext = vm;
        }

        // ---------------------------------------------------------------------------------
        // MỚI: PHƯƠNG THỨC XỬ LÝ NÚT KIỂM TRA ID (BẮT ĐẦU)
        // ---------------------------------------------------------------------------------
        private void CheckIdButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy viewModel hiện tại từ DataContext
            if (DataContext is CheckInDialogViewModel vm)
            {
                if (string.IsNullOrWhiteSpace(vm.IdCard))
                {
                    MessageBox.Show("Please enter an ID Card number to check.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Khởi tạo GuestService để tương tác với CSDL
                var guestService = new GuestService();
                try
                {
                    // KIỂM TRA: Đảm bảo bạn có phương thức 'GetGuestByIdCard' trong GuestService.
                    // Nếu chưa có, hãy xem hướng dẫn ở mục 2 bên dưới.
                    var existingGuest = guestService.GetGuestByIdCard(vm.IdCard);

                    if (existingGuest != null)
                    {
                        // Nếu tìm thấy, cập nhật thông tin lên ViewModel

                        // KIỂM TRA: Đảm bảo các tên thuộc tính (CName, CPhone, CCountry)
                        // khớp với tên trong class Model 'Guest' của bạn.
                        vm.GuestName = existingGuest.CName;
                        vm.PhoneNumber = existingGuest.CPhone;
                        vm.SelectedCountry = existingGuest.CCountry;

                        MessageBox.Show("Guest information found and pre-filled.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No guest found with this ID card number.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("System error when checking guest: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        // ---------------------------------------------------------------------------------
        // MỚI: PHƯƠNG THỨC XỬ LÝ NÚT KIỂM TRA ID (KẾT THÚC)
        // ---------------------------------------------------------------------------------


        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CheckInDialogViewModel vm)
            {
                var guestService = new GuestService();
                var bookingService = new BookingService();

                try
                {
                    int cid = guestService.AddGuest(vm.GuestName, vm.IdCard, vm.PhoneNumber, vm.SelectedCountry);

                    if (cid <= 0)
                    {
                        return;
                    }

                    bool success = bookingService.AddBooking(
                        cid,
                        vm.RoomIdInt,
                        vm.CheckInDateValue,
                        vm.CheckOutDateValue,
                        vm.PeopleCountInt
                    );

                    if (success)
                    {
                        MessageBox.Show("Check-in successful!");
                        this.DialogResult = true;
                    }
                    else
                    {
                        this.DialogResult = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("System error: " + ex.Message);
                    this.DialogResult = false;
                }

                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void RepairButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CheckInDialogViewModel vm)
            {
                int rid = vm.RoomIdInt;
                bool updateSuccess = false;

                try
                {
                    using (var db = new AppDbContext())
                    {
                        var roomEntity = db.Room.FirstOrDefault(r => r.RID == rid);
                        if (roomEntity != null)
                        {
                            roomEntity.RStatus = "repairing";
                            db.SaveChanges();
                            updateSuccess = true;
                        }
                        else
                        {
                            MessageBox.Show($"Không tìm thấy phòng RID {rid} trong cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật trạng thái phòng: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (updateSuccess)
                {
                    MessageBox.Show($"Phòng RID {rid} đã được chuyển sang trạng thái “Repairing”.", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                    vm.IsRepairRequested = true;
                    this.DialogResult = true;
                }
                else
                {
                    this.DialogResult = false;
                }

                this.Close();
            }
        }
    }
}