using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;
using System.Windows;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class CheckOutDialog : Window
    {
        private RoomModel _room;

        public CheckOutDialog(RoomModel room)
        {
            InitializeComponent();
            _room = room;
        }

        // Khi nhấn nút Confirm, tính tổng và đóng cửa sổ
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (CheckOutDialogViewModel)this.DataContext;
            vm.CalculateTotalAmount();  // Tính tổng tiền
            this.DialogResult = true;   // Đóng cửa sổ và trả lại kết quả
            this.Close();
        }

        // Khi nhấn nút Cancel, đóng cửa sổ mà không làm gì
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
