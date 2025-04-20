using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HotelManagementApp.ViewModels
{
    public class DashBoardViewModel : INotifyPropertyChanged
    {
        private string _currentTime;
        public string CurrentTime
        {
            get => _currentTime;
            set { _currentTime = value; OnPropertyChanged(nameof(CurrentTime)); }
        }

        private double _hourAngle;
        public double HourAngle
        {
            get => _hourAngle;
            set { _hourAngle = value; OnPropertyChanged(nameof(HourAngle)); }
        }

        private double _minuteAngle;
        public double MinuteAngle
        {
            get => _minuteAngle;
            set { _minuteAngle = value; OnPropertyChanged(nameof(MinuteAngle)); }
        }

        private double _secondAngle;
        public double SecondAngle
        {
            get => _secondAngle;
            set { _secondAngle = value; OnPropertyChanged(nameof(SecondAngle)); }
        }

        public DashBoardViewModel()
        {
            StartClock();
        }

        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                var now = DateTime.Now;
                CurrentTime = now.ToString("hh:mm:ss tt");
                HourAngle = (now.Hour % 12 + now.Minute / 60.0) * 30;
                MinuteAngle = (now.Minute + now.Second / 60.0) * 6;
                SecondAngle = now.Second * 6;
            };
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
