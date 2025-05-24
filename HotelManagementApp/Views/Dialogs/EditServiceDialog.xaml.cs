using HotelManagementApp.Models;
using System.Windows;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class EditServiceDialog : Window
    {
        public Service EditedService { get; private set; }

        public EditServiceDialog(Service originalService)
        {
            InitializeComponent();

            // Tạo một bản sao để chỉnh sửa, không ảnh hưởng trực tiếp đến SelectedService trong ViewModel
            EditedService = new Service
            {
                SID = originalService.SID,
                SName = originalService.SName,
                SUnit = originalService.SUnit,
                SPrice = originalService.SPrice
            };

            this.DataContext = EditedService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Trả về kết quả dialog là thành công
            this.DialogResult = true;
            this.Close();
        }
    }
}
