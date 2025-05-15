using HotelManagementApp.Database;
using HotelManagementApp.Models;
using System;
using System.Windows;

namespace HotelManagementApp.Helpers
{
    public class GuestService
    {
        public int AddGuest(string name, string idCard, string phone, string country)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var guest = new Customer
                    {
                        CName = name,
                        CPhone = phone,
                        CPersonalID = idCard,
                        CCountry = country,
                        CMail = "" // Trường này không được null nếu DB yêu cầu
                    };

                    context.Customer.Add(guest);
                    context.SaveChanges();

                    return guest.CID;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi thêm khách hàng: " + ex.Message);
                return -1;
            }
        }
    }
}
