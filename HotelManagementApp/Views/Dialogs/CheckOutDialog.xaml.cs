using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class CheckOutDialog : Window
    {
        private readonly CheckOutDialogViewModel _vm;
        private DateTime? _oldCheckOutDate;
        private bool _isExtending;
        private bool _dateChanged;

        public CheckOutDialog(RoomModel room)
        {
            InitializeComponent();
            _vm = (CheckOutDialogViewModel)DataContext;
            _vm.InitializeByRoomId(room.RID);
        }

        private void ExtendButton_Click(object sender, RoutedEventArgs e)
        {
            // Lưu ngày cũ và bật mode extend
            _oldCheckOutDate = _vm.CheckOutDate;
            _vm.OldCheckOutDate = _oldCheckOutDate;
            _isExtending = true;
            _dateChanged = false;
            _vm.IsCheckOutEditable = true;

            // Chỉ pick ngày sau ngày cũ
            dpCheckOut.DisplayDateStart = _oldCheckOutDate.Value.AddDays(1);

            // Mở popup
            dpCheckOut.IsDropDownOpen = true;
            dpCheckOut.Focus();
        }

        private void dpCheckOut_CalendarOpened(object sender, RoutedEventArgs e)
        {
            // Khi mở ra, reset lại flag
            _dateChanged = false;
        }

        private void dpCheckOut_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Chỉ set flag, không xử lý popup nào ở đây
            if (_isExtending)
                _dateChanged = true;
        }

        private void dpCheckOut_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (!_isExtending)
                return;

            var newDate = dpCheckOut.SelectedDate;
            // 1) Không chọn ngày hoặc chọn ngày không hợp lệ
            if (!newDate.HasValue || newDate.Value <= _oldCheckOutDate.Value)
            {
                MessageBox.Show(
                    $"Bạn phải chọn ngày sau {_oldCheckOutDate:dd/MM/yyyy}.",
                    "Ngày không hợp lệ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                // rollback và khóa lại
                _vm.CancelExtend();
            }
            else
            {
                // 2) Ngày hợp lệ, hỏi xác nhận
                var ans = MessageBox.Show(
                    $"Bạn có chắc muốn gia hạn đến ngày {newDate:dd/MM/yyyy} không?",
                    "Xác nhận gia hạn",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (ans == MessageBoxResult.Yes)
                    _vm.ConfirmExtend(newDate.Value);
                else
                    _vm.CancelExtend();
            }

            _isExtending = false;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_vm.SetRoomStatusToCleaning())
            {
                MessageBox.Show(
                    $"Không thể cập nhật trạng thái cho phòng (ID: {_vm.CurrentRoomId}).",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Thanh toán thành công!", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void QuantityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.DataContext is ServiceModel svc)
            {
                _vm.EvaluateAndSetQuantity(svc, tb.Text);
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
