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
        }
        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedInvoice = ((Button)sender).DataContext as Invoice;

            // Điều hướng sang trang Edit và truyền selectedGuest (nếu cần)
            this.NavigationService?.Navigate(new InvoiceDetail(selectedInvoice));

        }
    }
}