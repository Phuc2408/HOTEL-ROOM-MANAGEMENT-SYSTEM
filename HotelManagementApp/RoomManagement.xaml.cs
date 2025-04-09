using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HotelManagementApp
{
    public partial class RoomPage : Page
    {
        public class Room
        {
            public string? RoomNumber { get; set; }
            public string? RoomType { get; set; }
            public string? CheckInDate { get; set; }
            public string? CheckOutDate { get; set; }
            public RoomStatus Status { get; set; }
        }

        public enum RoomStatus
        {
            Empty,
            InUse,
            Overdue,
            Cleaning,
            Repairing
        }

        private Dictionary<int, List<Room>> roomsPerFloor = new Dictionary<int, List<Room>>();

        public RoomPage()
        {
            InitializeComponent();
            InitializeRooms();
            LoadRooms(1); // Load tầng 1 mặc định
        }

        private void InitializeComponent()
        {
            // This method is usually auto-generated in the .xaml.cs file
            // Ensure that the XAML file is correctly associated with this code-behind file
        }

        private void InitializeRooms()
        {
            roomsPerFloor = new Dictionary<int, List<Room>>();

            for (int floor = 1; floor <= 5; floor++)
            {
                var rooms = new List<Room>();
                for (int i = 1; i <= 28; i++)
                {
                    RoomStatus status;

                    if (floor == 1)
                    {
                        // Test data cho tầng 1
                        if (i <= 5)
                            status = RoomStatus.InUse;       // Xanh
                        else if (i <= 10)
                            status = RoomStatus.Overdue;     // Đỏ
                        else if (i <= 15)
                            status = RoomStatus.Cleaning;    // Vàng
                        else if (i <= 18)
                            status = RoomStatus.Repairing;   // Xám
                        else
                            status = RoomStatus.Empty;       // Trắng
                    }
                    else
                    {
                        status = RoomStatus.Empty;
                    }

                    rooms.Add(new Room
                    {
                        RoomNumber = $"{floor}0{i:D2}",
                        RoomType = (i % 3 == 0) ? "Double" : (i % 4 == 0) ? "VIP" : "Single",
                        CheckInDate = (status != RoomStatus.Empty) ? "2024-04-01" : "",
                        CheckOutDate = (status != RoomStatus.Empty) ? "2024-04-07" : "",
                        Status = status
                    });
                }
                roomsPerFloor[floor] = rooms;
            }
        }


        private void LoadRooms(int floor)
        {
            RoomWrapPanel.Children.Clear();
            var rooms = roomsPerFloor[floor];

            foreach (var room in rooms)
            {
                var roomControl = new RoomBox(room);
                roomControl.Margin = new Thickness(10);
                roomControl.OnRoomClicked += RoomBoxClicked;
                RoomWrapPanel.Children.Add(roomControl);
            }
        }

        private void RoomBoxClicked(Room room)
        {
            switch (room.Status)
            {
                case RoomStatus.Empty:
                    ShowCheckInDialog(room);
                    break;
                case RoomStatus.InUse:
                case RoomStatus.Overdue:
                    ShowCheckOutDialog(room);
                    break;
                case RoomStatus.Cleaning:
                    ConfirmClean(room);
                    break;
                case RoomStatus.Repairing:
                    MessageBox.Show("This room is under maintenance.");
                    break;
            }
        }

        private void ShowCheckInDialog(Room room)
        {
            CheckInPopup.Visibility = Visibility.Visible;

            CheckInPopup.OnCheckInConfirmed += (name, id, phone, checkIn, checkOut) =>
            {
                room.Status = RoomStatus.InUse;
                room.CheckInDate = checkIn?.ToShortDateString();
                room.CheckOutDate = checkOut?.ToShortDateString();
                LoadRooms(GetCurrentFloor());
            };
        }


        private void ConfirmClean(Room room)
        {
            var result = MessageBox.Show("Confirm cleaning completed?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                room.Status = RoomStatus.Empty;
                LoadRooms(GetCurrentFloor());
            }
        }

        private int GetCurrentFloor()
        {
            // TODO: Lấy số tầng hiện tại từ UI nếu cần
            return 1;
        }

        private void FloorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Content.ToString(), out int floor))
            {
                LoadRooms(floor);
            }
        }
    }
}
s