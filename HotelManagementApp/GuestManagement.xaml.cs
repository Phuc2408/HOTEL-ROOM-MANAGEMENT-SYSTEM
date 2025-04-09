using HotelManagementApp;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace HotelManagementApp
{
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

        // Declare the Guests list at the class level
        public List<Guest> Guests { get; set; }

        public GuestManagement()
        {
            InitializeComponent(); // Ensure this method is defined in the generated partial class
            Guests = new List<Guest>
        {
            new Guest { Name = "Jane Cooper", IDCard = "KA0963", PhoneNumber = "(225) 555-0118", Email = "jane@microsoft.com", Country = "USA", Room = "Room 101", StatusColor = Brushes.Green },
            new Guest { Name = "Floyd Miles", IDCard = "K09637", PhoneNumber = "(205) 555-0100", Email = "floyd@yahoo.com", Country = "Kiribati", Room = "Room 201", StatusColor = Brushes.Red }
        };
            GuestDataGrid.ItemsSource = Guests;
        }

        // Handle SearchBox focus events
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
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (GuestDataGrid.SelectedItem != null)
            {
                var selectedGuest = (Guest)GuestDataGrid.SelectedItem;
                EditGuest editPage = new EditGuest(selectedGuest);

                // Use the Navigated event of the NavigationService
                this.NavigationService.Navigated += (s, args) =>
                {
                    // Refresh the data after edit
                    GuestDataGrid.Items.Refresh();
                };

                this.NavigationService.Navigate(editPage);
            }
            else
            {
                MessageBox.Show("Please select a guest to edit.");
            }
        }


        // Handle Delete Button Click
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (GuestDataGrid.SelectedItem != null)
            {
                var selectedGuest = (Guest)GuestDataGrid.SelectedItem;

                // Confirm deletion
                var result = MessageBox.Show("Are you sure you want to delete this guest?", "Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // Delete the guest (this should be implemented)
                    DeleteGuest(selectedGuest);

                    // Update the DataGrid after deletion
                    var updatedGuests = Guests.Where(g => g != selectedGuest).ToList();
                    GuestDataGrid.ItemsSource = updatedGuests;
                }
            }
            else
            {
                MessageBox.Show("Please select a guest to delete.");
            }
        }

        // Delete a guest (implement this method to remove the guest from the data source)
        private void DeleteGuest(Guest guest)
        {
            // Remove the guest from the in-memory list
            Guests.Remove(guest);
        }
    }
}
