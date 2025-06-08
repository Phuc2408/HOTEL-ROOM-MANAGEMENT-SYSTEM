using HotelManagementApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Invoice
{
    [Key]
    public int IID { get; set; }

    public int ReID { get; set; } // ✅ Đúng tên cột trong DB

    [Column("InvoiceDate")]
    public DateTime IDate { get; set; }

    public decimal RoomTotal { get; set; }

    public decimal ServiceTotal { get; set; }

    public decimal Total { get; set; }

    [NotMapped]
    public ObservableCollection<Service> Services { get; set; }

    // ✅ Alias cho IDate để dùng trong ViewModel/UI
    [NotMapped]
    public DateTime InvoiceDate
    {
        get => IDate;
        set => IDate = value;
    }

    // ✅ Navigation property để JOIN với Rent
    [ForeignKey("ReID")]
    public virtual Rent Rent { get; set; }

    public Invoice()
    {
        Services = new ObservableCollection<Service>();
    }
}
