using HotelManagementApp.Database; // để dùng AppDbContext
using HotelManagementApp.Helpers;
using HotelManagementApp.Models;   // nếu cần RoomModel
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
                        MessageBox.Show("Failed to create guest.");
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
                        MessageBox.Show("Failed to check in when creating booking.");
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
                            // Nếu dùng RoomModel trong ViewModel, bạn có thể cập nhật thêm:
                            // roomModel.Status = "Repairing";
                            // roomModel.GuestName = null;
                            // roomModel.CheckInDate = null;
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
