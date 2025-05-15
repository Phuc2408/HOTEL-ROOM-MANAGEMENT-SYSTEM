using HotelManagementApp.Database;
using HotelManagementApp.Models;
using System;
using System.Windows;

namespace HotelManagementApp.Helpers
{
    public class BookingService
    {
        public bool AddBooking(int customerId, int roomId, DateTime checkInDate, DateTime checkOutDate, int numberOfPeople)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var rent = new Rent
                    {
                        CID = customerId,
                        RID = roomId,
                        CheckInDate = checkInDate.Date,
                        CheckOutDate = checkOutDate.Date,
                        CheckInTime = DateTime.Now.TimeOfDay,
                        CheckOutTime = DateTime.Now.AddHours(1).TimeOfDay, // có thể để null nếu cần
                        NumberOfPeople = numberOfPeople
                    };

                    context.Rent.Add(rent);
                    var room = context.Room.FirstOrDefault(r => r.RID == roomId);
                    if (room != null)
                    {
                        room.RStatus = "in_use";
                    }

                    context.SaveChanges();
                    return true;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi tạo booking: " + ex.Message);
                return false;
            }
        }

        public bool CheckOut(int rentId)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var rent = context.Rent.Find(rentId);
                    if (rent == null) return false;

                    rent.CheckOutTime = DateTime.Now.TimeOfDay;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi trả phòng: " + ex.Message);
                return false;
            }
        }
    }
}