using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using static HotelManagementApp.GuestManagement;

namespace HotelManagementApp
{
    public partial class EditGuest : Page
    {
        private Guest _guestToEdit;

        // Constructor nhận đối tượng Guest để sửa
        public EditGuest(Guest guest)
        {
            InitializeComponent();
            _guestToEdit = guest;

            // Đảm bảo thiết lập DataContext để binding
            this.DataContext = _guestToEdit;  // Liên kết đối tượng Guest với DataContext

            // Hiển thị thông tin trong các trường TextBox
            Name.Text = _guestToEdit.Name;
            Phone.Text = _guestToEdit.PhoneNumber;
            Email.Text = _guestToEdit.Email;
            Country.Text = _guestToEdit.Country;
            Status.Text = _guestToEdit.Room;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Cập nhật lại thông tin khách hàng
            _guestToEdit.Name = Name.Text;
            _guestToEdit.PhoneNumber = Phone.Text;
            _guestToEdit.Email = Email.Text;
            _guestToEdit.Country = Country.Text;
            _guestToEdit.Room = Status.Text;

            // Lưu vào cơ sở dữ liệu hoặc bộ nhớ nếu cần
            MessageBox.Show("Guest information updated successfully.");

            // Quay lại trang trước
            NavigationService.GoBack();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại trang trước
            NavigationService.GoBack();
        }
    }

}
