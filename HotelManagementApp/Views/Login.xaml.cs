using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace HotelManagementApp.Views
{
    public partial class Login : Window  // Chú ý: Sử dụng Window thay vì Page nếu muốn là cửa sổ chính
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {// Tạo và hiển thị cửa sổ Dashboard (hoặc MainWindow)
            DashBoard dashboard = new DashBoard();  // Tạo đối tượng cửa sổ Dashboard
            dashboard.Show();  // Hiển thị cửa sổ Dashboard sau khi hiệu ứng hoàn tất
            this.Close();  // Đóng cửa sổ Login thay vì ẩn (Hide)
        }

    }
}