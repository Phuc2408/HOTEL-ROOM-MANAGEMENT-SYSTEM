using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // THÊM DÒNG NÀY

namespace HotelManagementApp.Models
{
    public class Invoice
    {
        [Key]
        public int IID { get; set; }
        public int CID { get; set; }
        public int RelID { get; set; }
        public DateTime IDate { get; set; }
        public decimal RoomTotal { get; set; }
        public decimal ServiceTotal { get; set; }
        public decimal Total { get; set; }
        [NotMapped] // <<< THÊM DÒNG NÀY VÀO
        public ObservableCollection<Service> Services { get; set; }

        public Invoice()
        {
            Services = new ObservableCollection<Service>();
        }
    }
}
