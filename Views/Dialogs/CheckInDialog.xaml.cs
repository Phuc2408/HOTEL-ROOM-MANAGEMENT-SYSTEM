using HotelManagementApp.ViewModels;
using System.Windows;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class CheckInDialog : Window
    {
        public CheckInDialog(string roomId)
        {
            InitializeComponent();
            if (DataContext is ViewModels.CheckInDialogViewModel vm)
            {
                vm.RoomId = roomId;
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
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