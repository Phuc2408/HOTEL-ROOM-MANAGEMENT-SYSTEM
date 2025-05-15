using System;
using System.Data;
using System.Data.SqlClient;

namespace HotelManagementApp.Helpers
{
    public class InvoiceService
    {
        private readonly string connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;";


        // Hàm duy nhất để tạo hóa đơn hoàn chỉnh
        public bool CreateInvoice(int customerId, int rentId)
        {
            try
            {
                decimal roomTotal = CalculateRoomTotal(rentId);
                decimal serviceTotal = CalculateServiceTotal(customerId);
                decimal total = roomTotal + serviceTotal;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_AddInvoice", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CID", customerId);
                    cmd.Parameters.AddWithValue("@RelID", rentId);
                    cmd.Parameters.AddWithValue("@IDate", DateTime.Today);
                    cmd.Parameters.AddWithValue("@RoomTotal", roomTotal);
                    cmd.Parameters.AddWithValue("@ServiceTotal", serviceTotal);
                    cmd.Parameters.AddWithValue("@Total", total);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi tạo hóa đơn: " + ex.Message);
                return false;
            }
        }

        private decimal CalculateRoomTotal(int rentId)
        {
            decimal roomPrice = 0;
            int numberOfDays = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT DATEDIFF(DAY, r.CheckInDate, r.CheckOutDate) AS Days, rm.RPrice
                    FROM Rent r
                    JOIN Room rm ON r.RID = rm.RID
                    WHERE r.RelID = @RelID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RelID", rentId);

                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    numberOfDays = reader.GetInt32(0);
                    roomPrice = reader.GetDecimal(1);
                }
            }

            return numberOfDays * roomPrice;
        }

        private decimal CalculateServiceTotal(int customerId)
        {
            decimal total = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT SUM(SU.Quantity * S.SPrice)
                    FROM ServiceUsage SU
                    JOIN Service S ON SU.SID = S.SID
                    WHERE SU.CID = @CID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CID", customerId);

                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                    total = Convert.ToDecimal(result);
            }

            return total;
        }
    }
}
