using HotelManagementApp.Database;
using HotelManagementApp.Models;
using HotelManagementApp.ViewModels;
using HotelManagementApp.Views.Dialogs;
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
            DataContext = new ServiceViewModel();
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
        }
        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddServiceDialog();
            var result = dialog.ShowDialog();
            if (result == true)
            {
                var service = dialog.NewService;

                using (var context = new AppDbContext())
                {
                    context.Service.Add(service);
                    context.SaveChanges();
                }

                // Refresh lại danh sách
                if (DataContext is HotelManagementApp.ViewModels.ServiceViewModel vm)
                {
                    vm.LoadServices();
                }
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
