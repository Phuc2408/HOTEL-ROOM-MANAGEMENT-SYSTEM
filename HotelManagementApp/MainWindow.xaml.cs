using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HotelManagementApp;
using HotelManagementApp.Views;

namespace HotelManagementApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // <- không được lỗi
        }

        // Navigate to Guests
        private void NavigateToGuest(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new GuestManagement());
        }
        private void NavigateToDashBoard(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new DashBoard());
        }

        // Navigate to Rooms
        private void NavigateToRooms(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new RoomManagement());
        }

        // Navigate to Billing
        private void NavigateToInvoice(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new InvoiceManagement());
        }

        //Navigate to Services
        private void NavigateToServices(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new ServicesPage());
        }

        // Navigate to Report
        private void NavigateToReport(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new ReportPage());
        }
    }
}