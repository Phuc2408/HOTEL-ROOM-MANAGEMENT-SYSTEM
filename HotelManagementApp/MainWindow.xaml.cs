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
using System.IO;            
using System.Data.SqlClient;

namespace HotelManagementApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // <- không được lỗi
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.SingleBorderWindow; // hoặc ThreeDBorderWindow
            this.ResizeMode = ResizeMode.CanResize; // hoặc NoResize nếu muốn khóa kích thước
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
            // Tạo đường dẫn tới file SQL trong thư mục DB
            string scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database\\HotelDB.sql");

            // Kiểm tra file có tồn tại không
            if (!File.Exists(scriptPath))
            {
                MessageBox.Show("❌ Không tìm thấy file HotelDB.sql!");
                return;
            }

            // Đọc toàn bộ nội dung SQL
            string script = File.ReadAllText(scriptPath);

            // Chuỗi kết nối tới SQL Server LocalDB
            string connectionString = @"Server=(LocalDB)\MSSQLLocalDB;Integrated Security=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open(); // ✅ Đây là chỗ kiểm tra kết nối

                    SqlCommand cmd = conn.CreateCommand();

                    // Tách câu lệnh SQL theo GO
                    string[] commands = script.Split(new[] { "GO", "go", "Go" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string command in commands)
                    {
                        if (!string.IsNullOrWhiteSpace(command))
                        {
                            cmd.CommandText = command;
                            cmd.ExecuteNonQuery(); // Chạy từng câu lệnh
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kết nối hoặc thực thi SQL:\n" + ex.Message);
            }
        }
    }
}