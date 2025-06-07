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

        // ===[ SỬA 1 ]=== Cập nhật lại toàn bộ luồng xử lý của nút Confirm
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as CheckOutDialogViewModel;
            if (vm == null)
            {
                MessageBox.Show("Lỗi: Không tìm thấy ViewModel.", "Lỗi Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Bước 1: Gọi phương thức chốt hóa đơn từ ViewModel.
            // Phương thức này sẽ tính toán và cập nhật tiền phòng vào CSDL.
            //bool billFinalized = vm.FinalizeBill();
            //if (!billFinalized)
            //{
                // Nếu có lỗi, dừng lại. Thông báo lỗi đã được hiển thị bên trong ViewModel.
                //return;
            //}

            // Bước 2: Sau khi chốt hóa đơn thành công, tiến hành đổi trạng thái phòng.
            bool statusUpdated = vm.SetRoomStatusToCleaning();

            if (statusUpdated)
            {
                MessageBox.Show($"Thanh toán thành công!\nTrạng thái của phòng (ID: {vm.CurrentRoomId}) đã được cập nhật thành 'Cleaning'.",
                                "Thành Công",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show($"Không thể cập nhật trạng thái cho phòng (ID: {vm.CurrentRoomId}).\nVui lòng kiểm tra log hoặc thử lại.",
                                "Lỗi Cập Nhật Trạng Thái",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        // ===[ SỬA 2 ]=== Thêm logic xử lý cho sự kiện LostFocus
        private void QuantityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Lấy ServiceModel tương ứng với dòng hiện tại của TextBox
            var service = textBox.DataContext as ServiceModel;
            if (service == null) return;

            // Lấy ViewModel chính của Dialog
            var vm = this.DataContext as CheckOutDialogViewModel;
            if (vm == null) return;

            // Lấy biểu thức người dùng đã nhập
            string expression = textBox.Text;

            // Gọi phương thức trong ViewModel để xử lý tính toán
            vm.EvaluateAndSetQuantity(service, expression);

            // Vì UpdateSourceTrigger=Explicit, chúng ta phải tự cập nhật lại giao diện
            // để TextBox hiển thị giá trị cuối cùng sau khi đã tính toán.
            var binding = textBox.GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateTarget();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Có thể bỏ trống nếu không dùng
        }
    }
}