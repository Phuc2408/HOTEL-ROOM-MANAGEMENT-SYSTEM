using HotelManagementApp.Models;
using System;
using System.Windows;

namespace HotelManagementApp.Views.Dialogs
{
    public partial class AddServiceDialog : Window
    {
        public Service NewService { get; set; }

        public AddServiceDialog()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServiceName.Text))
            {
                MessageBox.Show("Service name must be add.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtUnitPrice.Text, out var price))
            {
                MessageBox.Show("Unit price must be a number.");
                return;
            }
            if (price <= 0)
            {
                MessageBox.Show("Price must be greater than 0.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NewService = new Service
            {
                SName = txtServiceName.Text,
                SUnit = txtUnit.Text,
                SPrice = price
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
