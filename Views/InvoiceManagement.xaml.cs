using System.Windows;
using System.Windows.Controls;
using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;

namespace HotelManagementApp.Views
{
    public partial class InvoiceManagement : Page
    {
        // Khởi tạo ViewModel và gán cho DataContext
        public InvoiceManagement()
        {
            InitializeComponent();
            // Tạo dữ liệu test
            var sampleInvoice = new InvoiceModel
            {
                GuestName = "Nguyễn Văn A",
                InvoiceID = "INV001",
                RentID = "RNT123",
                CheckOutDate = DateTime.Today,
                Total = 2500000
            };

            // Gán vào DataGrid tạm thời để test
            InvoiceDataGrid.ItemsSource = new List<InvoiceModel> { sampleInvoice };
        }
        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedInvoice = ((Button)sender).DataContext as InvoiceModel;

            // Điều hướng sang trang Edit và truyền selectedGuest (nếu cần)
                this.NavigationService?.Navigate(new InvoiceDetail(selectedInvoice));
            
        }
    }
}