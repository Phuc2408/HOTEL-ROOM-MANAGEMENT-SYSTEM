using HotelManagementApp.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotelManagementApp.Views
{
    /// <summary>
    /// Interaction logic for ServicesPage.xaml
    /// </summary>
    public partial class ServicesPage : Page
    {

        public ServicesPage()
        {
            InitializeComponent();
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedService = (ServiceModel)((Button)sender).DataContext;

            // Điều hướng sang EditService Page, truyền service
            this.NavigationService?.Navigate(new EditService(selectedService));
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            Models.ServiceModel selectedService = null;
            this.NavigationService?.Navigate(new InsertService(selectedService));
        }

    }
}
