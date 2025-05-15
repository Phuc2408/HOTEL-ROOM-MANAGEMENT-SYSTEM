using HotelManagementApp.Helpers;
using HotelManagementApp.ViewModels;
using System;
using System.Windows;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class CheckInDialog : Window
    {
        public CheckInDialog(string roomId)
        {
            InitializeComponent();

            var vm = new CheckInDialogViewModel();
            vm.RoomId = roomId;
            this.DataContext = vm;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CheckInDialogViewModel vm)
            {
                var guestService = new GuestService();
                var bookingService = new BookingService();

                try
                {
                    int cid = guestService.AddGuest(vm.GuestName, vm.IdCard, vm.PhoneNumber, vm.SelectedCountry);

                    if (cid <= 0)
                    {
                        MessageBox.Show("Failed to create guest.");
                        return;
                    }

                    bool success = bookingService.AddBooking(
                        cid,
                        vm.RoomIdInt,
                        vm.CheckInDateValue,
                        vm.CheckOutDateValue,
                        vm.PeopleCountInt
                    );

                    if (success)
                    {
                        MessageBox.Show("Check-in successful!");
                        this.DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("Failed to check in when creating booking.");
                        this.DialogResult = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("System error: " + ex.Message);
                    this.DialogResult = false;
                }

                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void RepairButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CheckInDialogViewModel vm)
            {
                vm.IsRepairRequested = true;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
