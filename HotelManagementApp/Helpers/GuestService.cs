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
        public Guest GetGuestByIdCard(string idCard)
        {
            using (var db = new AppDbContext())
            {
                // Bước 1: Lấy thực thể 'Customer' từ cơ sở dữ liệu.
                var customerEntity = db.Customer.FirstOrDefault(c => c.CPersonalID.Trim().ToUpper() == idCard.Trim().ToUpper());

                // Bước 2: Nếu không tìm thấy customer trong CSDL, trả về null.
                if (customerEntity == null)
                {
                    return null;
                }

                // Bước 3: Tạo một đối tượng 'Guest' mới và sao chép dữ liệu từ 'customerEntity'.
                var guestResult = new Guest
                {
                    CName = customerEntity.CName,
                    CPersonalID = customerEntity.CPersonalID,
                    CPhone = customerEntity.CPhone,
                    CCountry = customerEntity.CCountry,
                    CMail = customerEntity.CMail
                };

                // Bước 4: Trả về đối tượng 'Guest' đã được tạo.
                return guestResult;
            }
        }
    }
}
