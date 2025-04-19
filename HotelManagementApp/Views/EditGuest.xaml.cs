using System.Windows;
using System.Windows.Controls;
using HotelManagementApp.Models;

namespace HotelManagementApp.Views
{
    public partial class EditGuest : Page
    {
        private GuestModel _guestToEdit;

        public EditGuest(GuestModel guest)
        {
            InitializeComponent();
            _guestToEdit = guest;
            this.DataContext = _guestToEdit;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Nếu binding đang hoạt động đúng thì không cần gán lại giá trị ở đây.
            // Tuy nhiên bạn có thể xử lý thêm validation tại đây nếu cần.

            MessageBox.Show("Guest information updated successfully.");
            NavigationService.GoBack();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

}
