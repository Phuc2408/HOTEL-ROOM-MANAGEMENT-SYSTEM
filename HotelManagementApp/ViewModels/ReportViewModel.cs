using HotelManagementApp.Models;
using LiveCharts;
using LiveCharts.Wpf;
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
        public SeriesCollection ChartSeries { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public ICommand ChangeModeCommand { get; }

        public SeriesCollection GuestSeries { get; set; }
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

        public ReportViewModel()
        {
            ChartSeries = new SeriesCollection();
            GuestSeries = new SeriesCollection();
            ChangeModeCommand = new RelayCommand<string>(mode => LoadChart(mode));
            LoadChart("Today");
            LoadMonthlyGuests();
        }

        private void LoadChart(string mode)
        {
            var random = new Random();
            var currentPeriod = new List<(DateTime date, double revenue)>();
            var previousPeriod = new List<(DateTime date, double revenue)>();

            if (mode == "Today")
            {
                for (int i = 0; i < 24; i++)
                    currentPeriod.Add((DateTime.Today.AddHours(i), random.Next(100, 500)));
                for (int i = 0; i < 24; i++)
                    previousPeriod.Add((DateTime.Today.AddDays(-1).AddHours(i), random.Next(100, 500)));
            }
            else if (mode == "Week")
            {
                for (int i = 6; i >= 0; i--)
                    currentPeriod.Add((DateTime.Today.AddDays(-i), random.Next(1000, 3000)));
                for (int i = 13; i >= 7; i--)
                    previousPeriod.Add((DateTime.Today.AddDays(-i), random.Next(1000, 3000)));
            }
            else if (mode == "Month")
            {
                int daysThisMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                for (int i = 1; i <= daysThisMonth; i++)
                    currentPeriod.Add((new DateTime(DateTime.Today.Year, DateTime.Today.Month, i), random.Next(2000, 5000)));
                var lastMonth = DateTime.Today.AddMonths(-1);
                int daysLastMonth = DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month);
                for (int i = 1; i <= daysLastMonth; i++)
                    previousPeriod.Add((new DateTime(lastMonth.Year, lastMonth.Month, i), random.Next(2000, 5000)));
            }
            else if (mode == "Year")
            {
                for (int i = 1; i <= 12; i++)
                    currentPeriod.Add((new DateTime(DateTime.Today.Year, i, 1), random.Next(10000, 30000)));
                for (int i = 1; i <= 12; i++)
                    previousPeriod.Add((new DateTime(DateTime.Today.Year - 1, i, 1), random.Next(10000, 30000)));
            }

            TotalRevenue = currentPeriod.Sum(r => r.revenue);
            double lastRevenue = previousPeriod.Sum(r => r.revenue);

            double percentChange = lastRevenue == 0 ? 0 : (TotalRevenue - lastRevenue) / lastRevenue * 100;
            RevenueChangeText = Math.Abs(percentChange).ToString("0.0") + "%";
            RevenueChangeIcon = percentChange >= 0 ? "↑" : "↓";
            RevenueChangeColor = percentChange >= 0 ? Brushes.Green : Brushes.Red;

            RevenueCompareLabel = mode switch
            {
                "Today" => "compared to yesterday",
                "Week" => "compared to last week",
                "Month" => "compared to last month",
                "Year" => "compared to last year",
                _ => ""
            };

            var values = new ChartValues<double>();
            var labels = new List<string>();
            foreach (var record in currentPeriod)
            {
                values.Add(record.revenue);
                labels.Add(mode == "Year" ? record.date.ToString("MM") :
                           mode == "Month" ? record.date.ToString("dd") :
                           mode == "Week" ? record.date.ToString("ddd") :
                           record.date.ToString("HH") + "h");
            }

            ChartSeries.Clear();
            ChartSeries.Add(new LineSeries
            {
                Title = "Revenue",
                Values = values,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 8
            });

            Labels = labels.ToArray();
            YFormatter = value => value.ToString("N0") + " đ";

            OnPropertyChanged(nameof(ChartSeries));
            OnPropertyChanged(nameof(Labels));
            OnPropertyChanged(nameof(YFormatter));
        }

        private void LoadMonthlyGuests()
        {
            var stats = new List<MonthlyGuestStats>
            {
                new() { Month = "Jan", GuestCount = 120 },
                new() { Month = "Feb", GuestCount = 98 },
                new() { Month = "Mar", GuestCount = 145 },
                new() { Month = "Apr", GuestCount = 102 },
                new() { Month = "May", GuestCount = 165 },
                new() { Month = "Jun", GuestCount = 155 },
                new() { Month = "Jul", GuestCount = 170 },
                new() { Month = "Aug", GuestCount = 148 },
                new() { Month = "Sep", GuestCount = 133 },
                new() { Month = "Oct", GuestCount = 158 },
                new() { Month = "Nov", GuestCount = 144 },
                new() { Month = "Dec", GuestCount = 177 },
            };

            GuestLabels = stats.Select(s => s.Month).ToArray();

            GuestSeries = new SeriesCollection
            {
                 new ColumnSeries
    {
        Title = "",
        Values = new ChartValues<int>(stats.Select(s => s.GuestCount)),
        DataLabels = true
    }
};

            // Notify UI cập nhật
            OnPropertyChanged(nameof(GuestSeries));
            OnPropertyChanged(nameof(GuestLabels));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}