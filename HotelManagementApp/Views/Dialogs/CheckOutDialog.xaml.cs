using HotelManagementApp.Database;
using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;
using System.Linq;
using System.Windows;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class CheckOutDialog : Window
    {
        private RoomModel _room;

        public CheckOutDialog(RoomModel room)
        {
            InitializeComponent();

            var vm = new CheckOutDialogViewModel();
            vm.InitializeByRoomId(room.RID); // Chỉ dùng room.RID
            this.DataContext = vm;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (CheckOutDialogViewModel)this.DataContext;
            vm.CalculateTotalPrice();
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }
    }
}
