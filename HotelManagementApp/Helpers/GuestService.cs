using System;
using System.Data;
using System.Data.SqlClient;
using HotelManagementApp.Models;

namespace HotelManagementApp.Helpers
{
    public class GuestService
    {
        private readonly string connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;";


        public int AddGuest(string name, string idCard, string phone, string country)
        {
            int newId = -1;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"
            INSERT INTO Customer (CName, CPersonalID, CPhone, CMail, CCountry)
            VALUES (@name, @id, @phone, '', @country);
            SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@id", idCard);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@country", country);

                        conn.Open();
                        var result = cmd.ExecuteScalar();

                        if (result != DBNull.Value) // Kiểm tra kết quả trả về hợp lệ
                        {
                            newId = Convert.ToInt32(result);
                        }
                        else
                        {
                            Console.WriteLine("Không thể lấy ID từ SCOPE_IDENTITY.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi thực thi câu lệnh SQL: " + ex.Message);
            }

            return newId;
        }


    }
}
