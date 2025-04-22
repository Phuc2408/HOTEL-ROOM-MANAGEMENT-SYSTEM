using HotelManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HotelManagementApp.Views
{
    /// <summary>  
    /// Interaction logic for InsertGuestWindow.xaml  
    /// </summary>  
    public partial class InsertService : Page
    {
        public ServiceModel SelectedService { get; }

        // Removed duplicate constructor  
        public InsertService(ServiceModel selectedService)
        {
            SelectedService = selectedService;
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Lưu dữ liệu sửa đổi  
            // ...  

            // Quay về ServicesPage sau khi lưu  
            this.NavigationService?.Navigate(new ServicesPage());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.GoBack(); // Hoặc Navigate về ServicesPage nếu cần  
        }
    }
}
