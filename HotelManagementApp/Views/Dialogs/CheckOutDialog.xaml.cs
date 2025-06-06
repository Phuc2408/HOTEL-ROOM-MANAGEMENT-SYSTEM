using HotelManagementApp.Database;
using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class CheckOutDialog : Window
    {
        private RoomModel _room;

        public CheckOutDialog(RoomModel room)
        {
            InitializeComponent();

            var vm = new CheckOutDialogViewModel();
            vm.InitializeByRoomId(room.RID); // Chỉ dùng room.RID
            this.DataContext = vm;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as CheckOutDialogViewModel;
            if (vm == null)
            {
                MessageBox.Show("Lỗi: Không tìm thấy ViewModel.", "Lỗi Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Chỉ thực hiện việc chuyển trạng thái phòng sang "Cleaning"
            bool statusUpdated = vm.SetRoomStatusToCleaning();

            if (statusUpdated)
            {
                MessageBox.Show($"Trạng thái của phòng (ID: {vm.CurrentRoomId}) đã được cập nhật thành 'Cleaning'.",
                                "Cập Nhật Thành Công",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                this.DialogResult = true; // DialogResult có thể là true để báo hiệu hành động thành công
                this.Close();
            }
            else
            {
                // Hiển thị thông báo lỗi nếu cập nhật không thành công
                MessageBox.Show($"Không thể cập nhật trạng thái cho phòng (ID: {vm.CurrentRoomId}).\nVui lòng kiểm tra log hoặc thử lại.",
                                "Lỗi Cập Nhật Trạng Thái",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                // Trong trường hợp lỗi, bạn có thể không đóng dialog để người dùng xem xét
                // this.DialogResult = false; // Hoặc
                // this.Close(); // Hoặc không đóng
            }
        }
        private void QuantityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            var be = tb?.GetBindingExpression(TextBox.TextProperty);
            be?.UpdateSource();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }
    }
}
