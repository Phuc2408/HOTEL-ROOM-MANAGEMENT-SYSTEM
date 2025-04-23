using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HotelManagementApp;
using HotelManagementApp.Views;
using System.IO;                 // Cho File, Path
using System.Data.SqlClient;     // Cho SqlConnection, SqlCommand, SqlException

namespace HotelManagementApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // <- không được lỗi
            RebuildDatabaseOnStartup();
            NavigateToDashBoard(null, null); // Navigate to DashBoard on startup
        }

        // Navigate to Guests
        private void NavigateToGuest(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new GuestManagement());
        }
        private void NavigateToDashBoard(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new DashBoard());
        }

        // Navigate to Rooms
        private void NavigateToRooms(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new RoomManagement());
        }

        // Navigate to Billing
        private void NavigateToInvoice(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new InvoiceManagement());
        }

        //Navigate to Services
        private void NavigateToServices(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new ServicesPage());
        }

        // Navigate to Report
        private void NavigateToReport(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new ReportPage());
        }
        private void RebuildDatabaseOnStartup()
        {
            string scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database\\HotelDB.sql");
            if (!File.Exists(scriptPath))
            {
                MessageBox.Show("ERROR WHEN CREATE DATABASE!");
                return;
            }

            string script = File.ReadAllText(scriptPath);
            string connectionString = @"Server=(LocalDB)\MSSQLLocalDB;Integrated Security=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();

                    string[] commands = script.Split(new[] { "GO", "go", "Go" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string command in commands)
                    {
                        if (!string.IsNullOrWhiteSpace(command))
                        {
                            cmd.CommandText = command;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi khi tạo lại DB: " + ex.Message);
            }
        }
    }
}