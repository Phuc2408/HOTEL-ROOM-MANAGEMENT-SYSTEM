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
            if (checkOutDate.Date <= checkInDate.Date)
            {
                MessageBox.Show("Ngày trả phòng phải lớn hơn ngày nhận phòng.", "Lỗi ngày đặt phòng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
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
                        CheckOutTime = DateTime.Now.AddHours(1).TimeOfDay, 
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while creating booking: " + ex.Message);
                return false;
            }
        }

        
    }
}
