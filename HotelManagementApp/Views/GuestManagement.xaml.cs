using HotelManagementApp.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using HotelManagementApp.Views.Dialogs;
using HotelManagementApp.ViewModels;

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
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GuestManagementViewModel vm && vm.SelectedGuest != null)
            {
                // Chuyển từ GuestModel sang GuestViewModel để chỉnh sửa
                var guestVM = new GuestViewModel
                {
                    CID = vm.SelectedGuest.CID,
                    CName = vm.SelectedGuest.GuestName,
                    CPhone = vm.SelectedGuest.PhoneNumber,
                    CPersonalID = vm.SelectedGuest.IdCard
                };

                var dialog = new EditGuestDialog(guestVM);
                if (dialog.ShowDialog() == true)
                {
                    vm.UpdateGuest(guestVM);
                    vm.Guests.Clear();
                    vm.LoadGuests(); // làm mới danh sách
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
