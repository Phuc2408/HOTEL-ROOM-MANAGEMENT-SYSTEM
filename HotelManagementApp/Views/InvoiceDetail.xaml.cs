// InvoiceDetail.xaml.cs
using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;
using HotelManagementApp.Database;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagementApp.Views
{
    public partial class InvoiceDetail : Page
    {
        public InvoiceDetail(Invoice invoice)
        {
            InitializeComponent();

            

            var vm = new InvoiceDetailViewModel(invoice);
            this.DataContext = vm;


        }

        public InvoiceDetail() : this(GetSampleInvoice()) {
            MessageBox.Show("Constructor mặc định được gọi");
        }

        private static Invoice GetSampleInvoice()
        {
            MessageBox.Show("Vào được GetSampleInvoice");
            using var context = new AppDbContext();
            var invoice = context.Invoice.FirstOrDefault();

            if (invoice == null)
            {
                MessageBox.Show("Không tìm thấy invoice nào trong cơ sở dữ liệu.");
            }
            else
            {
                MessageBox.Show($"Lấy được invoice ID: {invoice.IID}");
            }

            return invoice;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.GoBack();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
