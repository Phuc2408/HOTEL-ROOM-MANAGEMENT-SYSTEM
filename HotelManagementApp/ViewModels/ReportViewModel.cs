using HotelManagementApp.Database;
using HotelManagementApp.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManagementApp.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        // ============================
        // 1. Các property đã có sẵn
        // ============================
        public SeriesCollection ChartSeries { get; set; }
        public SeriesCollection RoomTypeSeries { get; set; }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public ICommand ChangeModeCommand { get; }

        public SeriesCollection GuestSeries { get; set; }
        public SeriesCollection ServiceUsageSeries { get; set; }

        public string[] GuestLabels { get; set; }

        private double _totalRevenue;
        public double TotalRevenue
        {
            get => _totalRevenue;
            set { _totalRevenue = value; OnPropertyChanged(); }
        }

        private string _revenueChangeText;
        public string RevenueChangeText
        {
            get => _revenueChangeText;
            set { _revenueChangeText = value; OnPropertyChanged(); }
        }

        private string _revenueChangeIcon;
        public string RevenueChangeIcon
        {
            get => _revenueChangeIcon;
            set { _revenueChangeIcon = value; OnPropertyChanged(); }
        }

        private Brush _revenueChangeColor;
        public Brush RevenueChangeColor
        {
            get => _revenueChangeColor;
            set { _revenueChangeColor = value; OnPropertyChanged(); }
        }

        private string _revenueCompareLabel;
        public string RevenueCompareLabel
        {
            get => _revenueCompareLabel;
            set { _revenueCompareLabel = value; OnPropertyChanged(); }
        }

        // ============================
        // 2. Constructor
        // ============================
        public ReportViewModel()
        {
            // Khởi tạo tất cả SeriesCollection
            ChartSeries = new SeriesCollection();
            GuestSeries = new SeriesCollection();
            ServiceUsageSeries = new SeriesCollection();
            RoomTypeSeries = new SeriesCollection();

            ChangeModeCommand = new RelayCommand<string>(mode => LoadChart(mode));

            // Mặc định vẽ biểu đồ "Week" (từ Thứ Hai đến Chủ Nhật tuần hiện tại)
            LoadChart("Week");

            // Khởi tạo các biểu đồ khác
            LoadMonthlyGuests();
            LoadServiceUsageChart();
            LoadRoomTypePieChart();
        }

        // ============================
        // 3. Load biểu đồ Doanh thu theo mode (Today/Week/Month/Year)
        //    – Chú ý: “Week” cố định từ Thứ Hai đến Chủ Nhật tuần hiện tại
        // ============================
        private void LoadChart(string mode)
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;
                List<(DateTime key, double revenue)> currentPeriod = new List<(DateTime, double)>();
                List<(DateTime key, double revenue)> previousPeriod = new List<(DateTime, double)>();

                double currentSum = 0;
                double previousSum = 0;

                if (mode == "Today")
                {
                    var invoicesToday = db.Invoice
                        .Include(i => i.Rent)
                        .Where(i => i.IDate == today && i.Rent.isDone)
                        .GroupBy(i => i.IDate.Hour)
                        .Select(g => new { Hour = g.Key, Sum = g.Sum(i => (double)i.Total) })
                        .ToDictionary(x => x.Hour, x => x.Sum);

                    var invoicesYesterday = db.Invoice
                        .Include(i => i.Rent)
                        .Where(i => i.IDate == today.AddDays(-1) && i.Rent.isDone)
                        .GroupBy(i => i.IDate.Hour)
                        .Select(g => new { Hour = g.Key, Sum = g.Sum(i => (double)i.Total) })
                        .ToDictionary(x => x.Hour, x => x.Sum);

                    for (int h = 0; h < 24; h++)
                    {
                        double rev = invoicesToday.ContainsKey(h) ? invoicesToday[h] : 0;
                        currentPeriod.Add((today.AddHours(h), rev));
                        currentSum += rev;

                        double prevRev = invoicesYesterday.ContainsKey(h) ? invoicesYesterday[h] : 0;
                        previousPeriod.Add((today.AddDays(-1).AddHours(h), prevRev));
                        previousSum += prevRev;
                    }
                }
                else if (mode == "Week")
                {
                    int diffToMonday = (today.DayOfWeek == DayOfWeek.Sunday)
                                          ? -6
                                          : (DayOfWeek.Monday - today.DayOfWeek);
                    DateTime monday = today.AddDays(diffToMonday);
                    DateTime sunday = monday.AddDays(6);
                    DateTime prevMonday = monday.AddDays(-7);
                    DateTime prevSunday = sunday.AddDays(-7);

                    var invoicesWeek = db.Invoice
                        .Include(i => i.Rent)
                        .Where(i => i.IDate >= monday && i.IDate <= sunday && i.Rent.isDone)
                        .GroupBy(i => i.IDate.Date)
                        .Select(g => new { Date = g.Key, Sum = g.Sum(i => (double)i.Total) })
                        .ToDictionary(x => x.Date, x => x.Sum);

                    var invoicesPrevWeek = db.Invoice
                        .Include(i => i.Rent)
                        .Where(i => i.IDate >= prevMonday && i.IDate <= prevSunday && i.Rent.isDone)
                        .GroupBy(i => i.IDate.Date)
                        .Select(g => new { Date = g.Key, Sum = g.Sum(i => (double)i.Total) })
                        .ToDictionary(x => x.Date, x => x.Sum);

                    for (int i = 0; i < 7; i++)
                    {
                        var date = monday.AddDays(i).Date;
                        double rev = invoicesWeek.ContainsKey(date) ? invoicesWeek[date] : 0;
                        currentPeriod.Add((date, rev));
                        currentSum += rev;

                        var prevDate = prevMonday.AddDays(i).Date;
                        double prevRev = invoicesPrevWeek.ContainsKey(prevDate) ? invoicesPrevWeek[prevDate] : 0;
                        previousPeriod.Add((prevDate, prevRev));
                        previousSum += prevRev;
                    }
                }
                else if (mode == "Month")
                {
                    // ... giữ nguyên như cũ, thêm filter i.Rent.isDone ...
                    int year = today.Year;
                    int month = today.Month;
                    DateTime firstDayThisMonth = new DateTime(year, month, 1);
                    int daysThisMonth = DateTime.DaysInMonth(year, month);
                    DateTime lastDayThisMonth = new DateTime(year, month, daysThisMonth);
                    int offsetToSunday = ((int)DayOfWeek.Sunday - (int)firstDayThisMonth.DayOfWeek + 7) % 7;
                    DateTime firstSunday = firstDayThisMonth.AddDays(offsetToSunday);

                    var weekRangesThisMonth = new List<(DateTime Start, DateTime End)>();
                    if (firstSunday <= lastDayThisMonth)
                        weekRangesThisMonth.Add((firstDayThisMonth, firstSunday));
                    else
                        weekRangesThisMonth.Add((firstDayThisMonth, lastDayThisMonth));

                    DateTime weekStart = firstSunday.AddDays(1);
                    while (weekStart <= lastDayThisMonth)
                    {
                        DateTime weekEnd = weekStart.AddDays(6);
                        if (weekEnd > lastDayThisMonth) weekEnd = lastDayThisMonth;
                        weekRangesThisMonth.Add((weekStart, weekEnd));
                        weekStart = weekEnd.AddDays(1);
                    }

                    DateTime firstDayLastMonth = firstDayThisMonth.AddMonths(-1);
                    int yearLast = firstDayLastMonth.Year;
                    int monthLast = firstDayLastMonth.Month;
                    int daysLastMonth = DateTime.DaysInMonth(yearLast, monthLast);
                    DateTime lastDayLastMonth = new DateTime(yearLast, monthLast, daysLastMonth);
                    int offsetToSundayPrev = ((int)DayOfWeek.Sunday - (int)firstDayLastMonth.DayOfWeek + 7) % 7;
                    DateTime firstSundayPrev = firstDayLastMonth.AddDays(offsetToSundayPrev);

                    var weekRangesLastMonth = new List<(DateTime Start, DateTime End)>();
                    if (firstSundayPrev <= lastDayLastMonth)
                        weekRangesLastMonth.Add((firstDayLastMonth, firstSundayPrev));
                    else
                        weekRangesLastMonth.Add((firstDayLastMonth, lastDayLastMonth));

                    DateTime weekStartPrev = firstSundayPrev.AddDays(1);
                    while (weekStartPrev <= lastDayLastMonth)
                    {
                        DateTime weekEndPrev = weekStartPrev.AddDays(6);
                        if (weekEndPrev > lastDayLastMonth) weekEndPrev = lastDayLastMonth;
                        weekRangesLastMonth.Add((weekStartPrev, weekEndPrev));
                        weekStartPrev = weekEndPrev.AddDays(1);
                    }

                    for (int w = 0; w < weekRangesThisMonth.Count; w++)
                    {
                        var (start, end) = weekRangesThisMonth[w];
                        double rev = db.Invoice
                            .Include(i => i.Rent)
                            .Where(i => i.IDate >= start && i.IDate <= end && i.Rent.isDone)
                            .Sum(i => (double)i.Total);
                        currentPeriod.Add((start, rev));
                        currentSum += rev;
                    }

                    for (int w = 0; w < weekRangesLastMonth.Count; w++)
                    {
                        var (start, end) = weekRangesLastMonth[w];
                        double rev = db.Invoice
                            .Include(i => i.Rent)
                            .Where(i => i.IDate >= start && i.IDate <= end && i.Rent.isDone)
                            .Sum(i => (double)i.Total);
                        previousPeriod.Add((start, rev));
                        previousSum += rev;
                    }

                    Labels = weekRangesThisMonth
                        .Select((range, idx) => $"W{idx + 1}")
                        .ToArray();

                    var values = new ChartValues<double>(currentPeriod.Select(x => x.revenue));
                    ChartSeries.Clear();
                    ChartSeries.Add(new LineSeries
                    {
                        Title = "Revenue",
                        Values = values,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    });

                    YFormatter = value => value.ToString("N0") + " đ";

                    TotalRevenue = currentSum;
                    double percentChange = previousSum == 0
                        ? (currentSum > 0 ? 1 : 0)
                        : (currentSum - previousSum) / previousSum;
                    RevenueChangeText = Math.Abs(percentChange).ToString("P0");
                    RevenueChangeIcon = percentChange >= 0 ? "▲" : "▼";
                    RevenueChangeColor = percentChange >= 0 ? Brushes.Green : Brushes.Red;
                    RevenueCompareLabel = "compared to last month";

                    OnPropertyChanged(nameof(ChartSeries));
                    OnPropertyChanged(nameof(Labels));
                    OnPropertyChanged(nameof(YFormatter));
                    OnPropertyChanged(nameof(TotalRevenue));
                    OnPropertyChanged(nameof(RevenueChangeText));
                    OnPropertyChanged(nameof(RevenueChangeIcon));
                    OnPropertyChanged(nameof(RevenueChangeColor));
                    OnPropertyChanged(nameof(RevenueCompareLabel));
                }
                else if (mode == "Year")
                {
                    int year = today.Year;
                    int prevYear = year - 1;

                    var invoicesThisYear = db.Invoice
                        .Include(i => i.Rent)
                        .Where(i => i.IDate.Year == year && i.Rent.isDone)
                        .GroupBy(i => i.IDate.Month)
                        .Select(g => new { Month = g.Key, Sum = g.Sum(i => (double)i.Total) })
                        .ToDictionary(x => x.Month, x => x.Sum);

                    var invoicesLastYear = db.Invoice
                        .Include(i => i.Rent)
                        .Where(i => i.IDate.Year == prevYear && i.Rent.isDone)
                        .GroupBy(i => i.IDate.Month)
                        .Select(g => new { Month = g.Key, Sum = g.Sum(i => (double)i.Total) })
                        .ToDictionary(x => x.Month, x => x.Sum);

                    for (int m = 1; m <= 12; m++)
                    {
                        var date = new DateTime(year, m, 1);
                        double rev = invoicesThisYear.ContainsKey(m) ? invoicesThisYear[m] : 0;
                        currentPeriod.Add((date, rev));
                        currentSum += rev;

                        var prevDate = new DateTime(prevYear, m, 1);
                        double prevRev = invoicesLastYear.ContainsKey(m) ? invoicesLastYear[m] : 0;
                        previousPeriod.Add((prevDate, prevRev));
                        previousSum += prevRev;
                    }
                }

                if (mode != "Month")
                {
                    TotalRevenue = currentSum;

                    double percentChange = previousSum == 0
                        ? (currentSum > 0 ? 1 : 0)
                        : (currentSum - previousSum) / previousSum;
                    RevenueChangeText = Math.Abs(percentChange).ToString("P0");
                    RevenueChangeIcon = percentChange >= 0 ? "▲" : "▼";
                    RevenueChangeColor = percentChange >= 0 ? Brushes.Green : Brushes.Red;

                    RevenueCompareLabel = mode switch
                    {
                        "Today" => "compared to yesterday",
                        "Week" => "compared to last week",
                        "Year" => "compared to last year",
                        _ => ""
                    };

                    var values = new ChartValues<double>();
                    var labelsList = new List<string>();
                    foreach (var item in currentPeriod)
                    {
                        values.Add(item.revenue);
                        if (mode == "Today")
                            labelsList.Add(item.key.ToString("HH") + "h");
                        else if (mode == "Week")
                            labelsList.Add(item.key.ToString("ddd"));
                        else
                            labelsList.Add(item.key.ToString("MMM"));
                    }

                    ChartSeries.Clear();
                    ChartSeries.Add(new LineSeries
                    {
                        Title = "Revenue",
                        Values = values,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    });

                    Labels = labelsList.ToArray();
                    YFormatter = value => value.ToString("N0") + " đ";

                    OnPropertyChanged(nameof(ChartSeries));
                    OnPropertyChanged(nameof(Labels));
                    OnPropertyChanged(nameof(YFormatter));
                    OnPropertyChanged(nameof(TotalRevenue));
                    OnPropertyChanged(nameof(RevenueChangeText));
                    OnPropertyChanged(nameof(RevenueChangeIcon));
                    OnPropertyChanged(nameof(RevenueChangeColor));
                    OnPropertyChanged(nameof(RevenueCompareLabel));
                }
            }
        }



        // ============================
        // 4. Load số khách theo tháng
        //    (theo CheckOutDate trong năm hiện tại)
        // ============================
        private void LoadMonthlyGuests()
        {
            using (var db = new AppDbContext())
            {
                int year = DateTime.Today.Year;
                var dataDict = db.Rent
                    .Where(r => r.CheckOutDate != null && r.CheckOutDate.Year == year)
                    .GroupBy(r => r.CheckOutDate.Month)
                    .Select(g => new { Month = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.Month, x => x.Count);

                GuestLabels = Enumerable.Range(1, 12)
                    .Select(m => new DateTime(year, m, 1).ToString("MMM"))
                    .ToArray();

                var guestValues = new ChartValues<int>(
                    Enumerable.Range(1, 12)
                              .Select(m => dataDict.ContainsKey(m) ? dataDict[m] : 0)
                );

                GuestSeries.Clear();
                GuestSeries.Add(new ColumnSeries
                {
                    Title = "Guests",
                    Values = guestValues,
                    DataLabels = true
                });

                OnPropertyChanged(nameof(GuestSeries));
                OnPropertyChanged(nameof(GuestLabels));
            }
        }

        // ============================
        // 5. Load phân phối dịch vụ (Pie Chart)
        //    – Tính tổng số lượng (Quantity) của mỗi dịch vụ
        //    – Hiển thị Top 4 dịch vụ nhiều nhất, phần còn lại gom thành "Other"
        // ============================
        private void LoadServiceUsageChart()
        {
            using (var db = new AppDbContext())
            {
                // ===[ THÊM ]=== Bước 1: Lấy danh sách ID của các lượt thuê đã hoàn thành
                var completedRentIds = db.Rent
                    .Where(r => r.isDone == true)
                    .Select(r => r.ReID)
                    .ToList();

                if (!completedRentIds.Any())
                {
                    // Nếu không có lượt thuê nào hoàn thành, xóa dữ liệu cũ và thoát
                    ServiceUsageSeries.Clear();
                    OnPropertyChanged(nameof(ServiceUsageSeries));
                    return;
                }

                // ===[ SỬA ]=== Bước 2: Chỉ lấy ServiceUsage từ các lượt thuê đã hoàn thành
                var usageData = db.ServiceUsage
                    .Where(su => completedRentIds.Contains(su.ReID)) // Thêm điều kiện lọc ở đây
                    .GroupBy(su => su.SID)
                    .Select(g => new
                    {
                        SID = g.Key,
                        TotalQty = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(x => x.TotalQty)
                    .ToList();

                // Lấy danh sách tên dịch vụ một lần để tối ưu
                var serviceIds = usageData.Select(ud => ud.SID).ToList();
                var services = db.Service
                    .Where(s => serviceIds.Contains(s.SID))
                    .ToDictionary(s => s.SID, s => s.SName);

                ServiceUsageSeries.Clear();

                // Lấy top 4
                var top4Data = usageData.Take(4).ToList();
                double othersTotal = usageData.Skip(4).Sum(x => x.TotalQty);

                foreach (var item in top4Data)
                {
                    // Lấy tên dịch vụ từ Dictionary đã tải sẵn
                    string serviceName = services.TryGetValue(item.SID, out var name) ? name : $"Service {item.SID}";

                    ServiceUsageSeries.Add(new PieSeries
                    {
                        Title = serviceName,
                        Values = new ChartValues<double> { item.TotalQty },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{chartPoint.Y:N0}"
                    });
                }

                // Nếu còn phần “Other” ≥ 1, thêm slice "Other"
                if (othersTotal > 0)
                {
                    ServiceUsageSeries.Add(new PieSeries
                    {
                        Title = "Other",
                        Values = new ChartValues<double> { othersTotal },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{chartPoint.Y:N0}"
                    });
                }

                OnPropertyChanged(nameof(ServiceUsageSeries));
            }
        }

        // ============================
        // 6. Load phân phối loại phòng (Pie Chart)
        //    – Tính số phòng được sử dụng (CheckOut) theo loại trong tháng hiện tại
        // ============================
        private void LoadRoomTypePieChart()
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;
                int currentYear = today.Year;
                int currentMonth = today.Month;

                // Join Rent và Room, nơi Rent.CheckOutDate nằm trong tháng này
                var roomUsageData = db.Rent
                    .Where(r => r.CheckOutDate != null
                             && r.CheckOutDate.Year == currentYear
                             && r.CheckOutDate.Month == currentMonth)
                    .Join(db.Room,
                          rent => rent.RID,
                          room => room.RID,
                          (rent, room) => room.RType)
                    .GroupBy(rtype => rtype)
                    .Select(g => new { RType = g.Key, Count = g.Count() })
                    .ToList();

                RoomTypeSeries.Clear();

                foreach (var item in roomUsageData)
                {
                    RoomTypeSeries.Add(new PieSeries
                    {
                        Title = item.RType,
                        Values = new ChartValues<double> { item.Count },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{chartPoint.Y:N0}"
                    });
                }

                OnPropertyChanged(nameof(RoomTypeSeries));
            }
        }

        // ============================
        // 7. Implementation of RelayCommand<T>
        // ============================
        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Func<T, bool> _canExecute;

            public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                if (_canExecute == null) return true;
                return _canExecute((T)parameter);
            }

            public void Execute(object parameter)
            {
                _execute((T)parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }

        // ============================
        // 8. INotifyPropertyChanged
        // ============================
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
