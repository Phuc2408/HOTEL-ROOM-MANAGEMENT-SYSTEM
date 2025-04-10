using HotelManagementApp;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using MahApps.Metro.IconPacks;

namespace HotelManagementApp
{
    public partial class GuestManagement : Page
    {
        private int currentPage = 1;
        private int pageSize = 2;
        private int totalPages => (int)Math.Ceiling((double)FilteredGuests.Count / pageSize);

        public class Guest
        {
            public string? Name { get; set; }
            public string? IDCard { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Email { get; set; }
            public string? Country { get; set; }
            public string? Room { get; set; }
            public Brush? StatusColor { get; set; }
            public DateTime CheckInDate { get; set; }
        }

        public List<Guest> Guests { get; set; }

        public GuestManagement()
        {
            InitializeComponent();

            // Dữ liệu mẫu
            Guests = new List<Guest>
            {
                new Guest
                {
                    Name = "Jane Cooper", IDCard = "KA0963", PhoneNumber = "(225) 555-0118",
                    Email = "jane@microsoft.com", Country = "USA", Room = "Room 101", StatusColor = Brushes.Green,
                    CheckInDate = new DateTime(2024, 12, 20)
                },
                new Guest
                {
                    Name = "Floyd Miles", IDCard = "K09637", PhoneNumber = "(205) 555-0100",
                    Email = "floyd@yahoo.com", Country = "Kiribati", Room = "Room 201", StatusColor = Brushes.Red,
                    CheckInDate = new DateTime(2025, 1, 15)
                }
            };

            SortComboBox.SelectedIndex = 0;
            LoadGuestsForPage(currentPage);
            GeneratePaginationButtons();
        }

        private List<Guest> FilteredGuests =>
            Guests.Where(g =>
                string.IsNullOrWhiteSpace(SearchBox.Text) || SearchBox.Text == "Search" ||
                (g.Name != null && g.Name.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase)) ||
                (g.IDCard != null && g.IDCard.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase))
            ).ToList();

        private void ApplySearchAndSort()
        {
            if (SortComboBox == null || GuestDataGrid == null) return;

            var filtered = FilteredGuests;

            if (SortComboBox.SelectedIndex == 0)
                filtered = filtered.OrderByDescending(g => g.CheckInDate).ToList();
            else
                filtered = filtered.OrderBy(g => g.CheckInDate).ToList();

            currentPage = 1;
            LoadGuestsForPage(currentPage);
            GeneratePaginationButtons();
        }


        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Search")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.Black;
            }
        }
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search";
                SearchBox.Foreground = Brushes.Gray;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsInitialized) // tránh lỗi lúc khởi tạo
                ApplySearchAndSort();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInitialized)
                ApplySearchAndSort();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (GuestDataGrid.SelectedItem is Guest selectedGuest)
            {
                EditGuest editPage = new EditGuest(selectedGuest);

                this.NavigationService.Navigated += (s, args) =>
                {
                    GuestDataGrid.Items.Refresh();
                };

                this.NavigationService.Navigate(editPage);
            }
            else
            {
                MessageBox.Show("Please select a guest to edit.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (GuestDataGrid.SelectedItem is Guest selectedGuest)
            {
                var result = MessageBox.Show("Are you sure you want to delete this guest?", "Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Guests.Remove(selectedGuest);
                    ApplySearchAndSort();
                }
            }
            else
            {
                MessageBox.Show("Please select a guest to delete.");
            }
        }

        private void GuestDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: xử lý chọn dòng
        }
        
        private void LoadGuestsForPage(int page)
        {
            int skip = (page - 1) * pageSize;
            var filtered = FilteredGuests;

            if (SortComboBox.SelectedIndex == 0)
                filtered = filtered.OrderByDescending(g => g.CheckInDate).ToList();
            else
                filtered = filtered.OrderBy(g => g.CheckInDate).ToList();

            var pagedGuests = filtered.Skip(skip).Take(pageSize).ToList();
            GuestDataGrid.ItemsSource = pagedGuests;
        }


        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadGuestsForPage(currentPage);
                GeneratePaginationButtons();
            }
        }
        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag.ToString(), out int page))
            {
                currentPage = page;
                LoadGuestsForPage(currentPage);
                GeneratePaginationButtons();
            }
        }

        private void BackPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadGuestsForPage(currentPage);
                GeneratePaginationButtons();
            }
        }

        private void GeneratePaginationButtons()
        {
            PaginationPanel.Children.Clear();

            var backButton = CreatePageButton("back", "ChevronLeft", BackPage_Click);
            PaginationPanel.Children.Add(backButton);

            for (int i = 1; i <= totalPages; i++)
            {
                var btn = new Button
                {
                    Content = i.ToString(),
                    Style = (Style)FindResource("PaginationButtonStyle"),
                    Tag = i
                };

                if (i == currentPage)
                    btn.Background = Brushes.LightGray;

                btn.Click += PageButton_Click;
                PaginationPanel.Children.Add(btn);
            }

            var nextButton = CreatePageButton("next", "ChevronRight", NextPage_Click);
            PaginationPanel.Children.Add(nextButton);
        }
        
        private Button CreatePageButton(string name, string iconKind, RoutedEventHandler click)
        {
            var icon = new PackIconMaterial
            {
                Kind = (PackIconMaterialKind)Enum.Parse(typeof(PackIconMaterialKind), iconKind),
                Width = 16,
                Height = 16,
                Foreground = Brushes.Black
            };

            var button = new Button
            {
                Style = (Style)FindResource("PaginationButtonStyle"),
                Content = icon,
                Tag = name
            };
            button.Click += click;
            return button;
        }
    }
}
