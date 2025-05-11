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
            var button = sender as Button;
            var displayModel = button?.DataContext as InvoiceDisplayModel;

            if (displayModel == null)
            {
                MessageBox.Show("Không lấy được hóa đơn.");
                return;
            }

            using (var context = new AppDbContext())
            {
                // Dùng chính IID để lấy từ DB
                var invoiceEntity = context.Invoice.FirstOrDefault(i => i.IID == displayModel.IID);

                if (invoiceEntity != null)
                {
                    this.NavigationService?.Navigate(new InvoiceDetail(invoiceEntity));
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy hóa đơn với IID = {displayModel.IID}");
                }
            }
        }

        private void InvoiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // (nếu cần xử lý chọn dòng thì thêm sau)
        }
    }
}
