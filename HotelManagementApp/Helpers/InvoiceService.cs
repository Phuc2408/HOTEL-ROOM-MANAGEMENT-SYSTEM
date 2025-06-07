using System;
using System.Data;
using System.Data.SqlClient;

namespace HotelManagementApp.Helpers
{
    public class InvoiceService
    {
        private readonly string connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;";


        

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
                    WHERE r.ReID = @ReID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ReID", rentId);

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
