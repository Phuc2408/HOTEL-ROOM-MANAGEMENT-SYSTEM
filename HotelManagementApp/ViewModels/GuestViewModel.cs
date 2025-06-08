using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotelManagementApp.ViewModels
{
    public class GuestViewModel : INotifyPropertyChanged
    {
        public int CID { get; set; } // ánh xạ với Customer.CID

        private string _cName = string.Empty;
        public string CName
        {
            get => _cName;
            set { _cName = value; OnPropertyChanged(); }
        }

        private string _cPhone = string.Empty;
        public string CPhone
        {
            get => _cPhone;
            set { _cPhone = value; OnPropertyChanged(); }
        }

        private string _cPersonalID = string.Empty;
        public string CPersonalID
        {
            get => _cPersonalID;
            set { _cPersonalID = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}