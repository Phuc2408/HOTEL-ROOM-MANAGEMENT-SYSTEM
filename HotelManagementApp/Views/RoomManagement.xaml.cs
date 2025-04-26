using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManagementApp.Views.Dialogs;

namespace HotelManagementApp.Views
{
    public partial class RoomManagement : Page
    {
        public RoomManagementViewModel ViewModel { get; set; }

        public RoomManagement()
        {
            InitializeComponent();
            ViewModel = new RoomManagementViewModel();
            this.DataContext = ViewModel;
        }

        private void RoomBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is RoomModel room)
            {
                int roomId = room.RoomId;

                switch (room.Status)
                {
                    case "empty":
                        ShowCheckInDialog(room);
                        break;
                    case "in_use":
                        ShowCheckOutDialog(room, roomId);
                        break;
                    case "cleaning":
                        ShowCleaningDoneDialog(room);
                        break;
                }
            }
        }

        private void ShowCheckInDialog(RoomModel room)
        {
            var dialog = new CheckInDialog(room.RoomId.ToString());
            if (dialog.ShowDialog() == true)
            {
                var vm = dialog.DataContext as CheckInDialogViewModel;

                if (vm.IsRepairRequested)
                {
                    room.Status = "repairing";
                    room.GuestName = null;
                    room.CheckInDate = null;
                }
                else
                {
                    room.Status = "in_use";
                    room.GuestName = vm.GuestName;
                    room.CheckInDate = vm.CheckInDate;
                }
            }
        }
        private void ShowCheckOutDialog(RoomModel room, int roomId)
        {
            var dialog = new CheckOutDialog(room); // sẽ tạo sau
            if (dialog.ShowDialog() == true)
            {
                room.Status = "cleaning";
            }
        }

        private void ShowCleaningDoneDialog(RoomModel room)
        {
            var result = MessageBox.Show("Xác nhận dọn xong phòng này?", "Xác nhận", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                room.Status = "empty";
                room.GuestName = null;
                room.CheckInDate = null;
            }
        }
    }
}
