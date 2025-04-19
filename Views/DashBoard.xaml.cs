using System;
using System.Windows;
using System.Windows.Input;

namespace HotelManagementApp.Views
{
    public partial class DashBoard : Window
    {
        public DashBoard()
        {
            InitializeComponent();
        }
        private void NavigateToDashBoard(object sender, MouseButtonEventArgs e)
        {
            DashBoard dashboard = new DashBoard(); // Tạo đối tượng Dashboard (là cửa sổ)
            dashboard.Show(); // Hiển thị cửa sổ Dashboard

            // Bạn có thể đóng cửa sổ hiện tại nếu không muốn giữ nó mở nữa
            this.Close(); // Đóng cửa sổ hiện tại (MainWindow)
        }

        // Điều hướng đến trang Guests
        private void NavigateToGuest(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GuestManagement());  // GuestManagement phải là Page
        }

        // Điều hướng đến trang Rooms
        private void NavigateToRooms(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RoomManagement());  // RoomManagement phải là Page
        }

        // Điều hướng đến trang Invoice
        private void NavigateToInvoice(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new InvoiceManagement());  // InvoiceManagement phải là Page
        }

        // Điều hướng đến trang Services
        private void NavigateToServices(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ServicesPage());  // ServicesPage phải là Page
        }

        // Điều hướng đến trang Report
        private void NavigateToReport(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new ReportPage());  // ReportPage phải là Page
        }
    }
}
