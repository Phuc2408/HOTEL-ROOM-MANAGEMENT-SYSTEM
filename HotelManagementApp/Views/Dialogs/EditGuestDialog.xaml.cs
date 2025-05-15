using System.Windows;
using HotelManagementApp.ViewModels;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class EditGuestDialog : Window
    {
        public GuestViewModel Guest { get; private set; }

        public EditGuestDialog(GuestViewModel guestViewModel)
        {
            InitializeComponent();
            Guest = guestViewModel;
            this.DataContext = Guest;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
