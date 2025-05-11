using System;
using System.Data;
using System.Data.SqlClient;

namespace HotelManagementApp.Helpers
{
    public class BookingService
    {
        private readonly string connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;";


        // Hàm Check-in: thêm bản ghi thuê phòng và cập nhật trạng thái phòng
        public bool AddBooking(int customerId, int roomId, DateTime checkInDate, DateTime checkOutDate, int numberOfPeople)
        {
            TimeSpan checkInTime = DateTime.Now.TimeOfDay; // ⏰ Giờ thực tế khi check-in

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_AddRent", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CID", customerId);
                    cmd.Parameters.AddWithValue("@RID", roomId);
                    cmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                    cmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                    cmd.Parameters.AddWithValue("@CheckInTime", checkInTime);
                    cmd.Parameters.AddWithValue("@CheckOutTime", DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumberOfPeople", numberOfPeople);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi thêm đặt phòng: " + ex.Message);
                return false;
            }
        }


        // Hàm Check-out: cập nhật thời gian và chuyển phòng sang trạng thái "Cleaning"
        public bool CheckOut(int rentId)
        {
            try
            {
                TimeSpan actualCheckOutTime = DateTime.Now.TimeOfDay;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_CheckOut", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RelID", rentId);
                    cmd.Parameters.AddWithValue("@ActualCheckOutTime", actualCheckOutTime);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi trả phòng: " + ex.Message);
                return false;
            }
        }

    }
}
