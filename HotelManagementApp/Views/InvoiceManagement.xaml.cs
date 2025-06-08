using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HotelManagementApp.Database;
using HotelManagementApp.Models;

namespace HotelManagementApp.Views
{
    public partial class InvoiceManagement : Page
    {
        public InvoiceManagement()
        {
            InitializeComponent();
        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is InvoiceDisplayModel displayModel)
            {
                using var context = new AppDbContext();
                var invoiceEntity = context.Invoice.FirstOrDefault(i => i.IID == displayModel.IID);

                if (invoiceEntity != null)
                {
                    NavigationService?.Navigate(new InvoiceDetail(invoiceEntity));
                }
                else
                {
                    MessageBox.Show(
                        $"Không tìm thấy hóa đơn với IID = {displayModel.IID}",
                        "Thông báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Không thể lấy thông tin hóa đơn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InvoiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Tùy chọn: xử lý khi chọn dòng
        }
    }
}
