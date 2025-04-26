using HotelManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotelManagementApp.Views
{
    /// <summary>
    /// Interaction logic for DetailInvoice.xaml
    /// </summary>
    public partial class InvoiceDetail : Page
    {
        private Invoice _invoice;

        public InvoiceDetail()
        {
        }

        public InvoiceDetail(Invoice invoice)
        {
            InitializeComponent();
            _invoice = invoice;

            // Hiển thị dữ liệu nếu cần
            this.DataContext = _invoice;

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {

            this.NavigationService?.GoBack();
        }
    }
}