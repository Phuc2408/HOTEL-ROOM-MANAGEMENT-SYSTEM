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

namespace HotelManagementApp
{
    /// <summary>
    /// Interaction logic for GuestManagement.xaml
    /// </summary>
    public partial class GuestManagement : Page
    {
        public class Guest
        {
            public string? Name { get; set; }
            public string? IDCard { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Email { get; set; }
            public string? Country { get; set; }
            public string? Room { get; set; }
            public Brush? StatusColor { get; set; }
        }

        public GuestManagement()
        {
            InitializeComponent();

            var demoData = new List<Guest>
        {
            new Guest { Name = "Jane Cooper", IDCard = "KA0963", PhoneNumber = "(225) 555-0118", Email = "jane@microsoft.com", Country = "USA", Room = "Room 101", StatusColor = Brushes.Green },
            new Guest { Name = "Floyd Miles", IDCard = "K09637", PhoneNumber = "(205) 555-0100", Email = "floyd@yahoo.com", Country = "Kiribati", Room = "Room 201", StatusColor = Brushes.Red }
        };

            GuestDataGrid.ItemsSource = demoData;
        }
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Search")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search";
                SearchBox.Foreground = Brushes.Gray;
            }
        }

    }
}
