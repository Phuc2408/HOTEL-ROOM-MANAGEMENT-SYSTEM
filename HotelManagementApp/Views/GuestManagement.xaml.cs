using HotelManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace HotelManagementApp.Views
{
    /// <summary>
    /// Interaction logic for GuestManagement.xaml
    /// </summary>
    public partial class GuestManagement : Page
    {
        public GuestManagement()
        {
            InitializeComponent();

           

            // Gán dữ liệu khách vào DataGrid
        }

        // Navigate to EditGuest Page
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

            if (GuestDataGrid.SelectedItem is GuestModel selectedGuest)
            {
                // Điều hướng sang trang Edit và truyền selectedGuest (nếu cần)
                this.NavigationService?.Navigate(new EditGuest(selectedGuest));
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách cần sửa trước khi nhấn nút Edit.", "Chưa chọn khách", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
