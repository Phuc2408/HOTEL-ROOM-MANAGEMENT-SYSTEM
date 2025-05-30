using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManagementApp.Views.Dialogs;
using HotelManagementApp.Database;
using System.Diagnostics;

namespace HotelManagementApp.Views
{
    public partial class RoomManagement : Page
    {
        public RoomManagementViewModel ViewModel { get; set; }

        public RoomManagement()
        {
            InitializeComponent();
            ViewModel = new RoomManagementViewModel();
            this.DataContext = ViewModel;
        }

        private void RoomBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is RoomModel room)
            {
                int roomId = room.RoomId;

                switch (room.Status)
                {
                    case "available":
                        ShowCheckInDialog(room);
                        break;
                    case "in_use":
                        ShowCheckOutDialog(room, roomId);
                        break;
                    case "cleaning":
                        ShowCleaningDoneDialog(room);
                        break;
                }
            }
        }

        private void ShowCheckInDialog(RoomModel room)
        {
            var dialog = new CheckInDialog(room.RoomId.ToString());
            if (dialog.ShowDialog() == true)
            {
                var vm = dialog.DataContext as CheckInDialogViewModel;

                if (vm.IsRepairRequested)
                {
                    room.Status = "repairing";
                    room.GuestName = null;
                    room.CheckInDate = null;
                }
                else
                {
                    room.Status = "in_use";
                    room.GuestName = vm.GuestName;
                    room.CheckInDate = vm.CheckInDate;
                }
            }
        }
        private void ShowCheckOutDialog(RoomModel room, int roomId)
        {
            var dialog = new CheckOutDialog(room);
            if (dialog.ShowDialog() == true)
            {
                room.Status = "cleaning";
                room.GuestName = null;
                room.CheckInDate = null;
            }
        }

        private void ShowCleaningDoneDialog(RoomModel room) // room là đối tượng RoomModel từ UI
        {
            if (room == null)
            {
                MessageBox.Show("Lỗi: Thông tin phòng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Sử dụng một thuộc tính dễ nhận biết của phòng để hiển thị, ví dụ room.RoomNumber hoặc room.RID
            string roomIdentifier = room.RID.ToString(); // Hoặc một thuộc tính tên phòng nếu có, ví dụ room.RType + room.RID 
                                                         // Bạn cần một cách để xác định phòng trong thông báo
                                                         // Ví dụ: if (room.RoomNumberProperty != null) roomIdentifier = room.RoomNumberProperty;


            var result = MessageBox.Show($"Xác nhận phòng [{roomIdentifier}] đã dọn dẹp xong và sẵn sàng?",
                                         "Xác Nhận Hoàn Tất Dọn Dẹp",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool updateSuccess = false;
                try
                {
                    using (var db = new AppDbContext()) // Khởi tạo DbContext
                    {
                        var roomEntityInDb = db.Room.FirstOrDefault(r => r.RID == room.RID);

                        if (roomEntityInDb != null)
                        {
                            roomEntityInDb.RStatus = "available";
                            db.SaveChanges();
                            updateSuccess = true;
                            Debug.WriteLine($"Đã cập nhật trạng thái phòng RID {room.RID} thành 'Available' trong DB.");
                        }
                        else
                        {
                            MessageBox.Show($"Không tìm thấy phòng [{roomIdentifier}] trong cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Lỗi khi cập nhật phòng RID {room.RID} xuống DB: {ex.Message}");
                    MessageBox.Show($"Đã xảy ra lỗi khi cập nhật trạng thái phòng: {ex.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (updateSuccess)
                {
                    room.Status = "available";
                    room.GuestName = null;
                    room.CheckInDate = null;

                }
                // Nếu updateSuccess là false, các thông báo lỗi đã được hiển thị.
            }
        }
    }
        
}
