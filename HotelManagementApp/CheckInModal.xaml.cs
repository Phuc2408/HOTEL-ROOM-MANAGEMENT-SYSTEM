using System;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagementApp
{
    public partial class CheckInModal : UserControl
    {
        public CheckInModal()
        {
            InitializeComponent();
        }

        // Sự kiện Cancel: Đóng popup
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Đóng popup
            this.Visibility = Visibility.Collapsed;
        }

        // Sự kiện Check In: Xử lý check-in và đóng popup
        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            // Thực hiện logic check-in, ví dụ:
            var guestName = GuestNameBox.Text;
            var idCard = IDCardBox.Text;
            var phone = PhoneBox.Text;
            var checkInDate = CheckInDate.SelectedDate;
            var checkOutDate = CheckOutDate.SelectedDate;

            // Logic xử lý check-in (có thể lưu dữ liệu vào database hoặc thực hiện các tác vụ khác)

            MessageBox.Show($"Checked in {guestName} to Room {RoomNumber}.");

            // Đóng popup sau khi check-in thành công
            this.Visibility = Visibility.Collapsed;
        }
    }
}
