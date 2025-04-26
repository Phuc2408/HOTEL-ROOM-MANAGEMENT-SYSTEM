using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementApp.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public int CID { get; set; }

        public string CName { get; set; }

        public string CPhone { get; set; }

        public string CPersonalID { get; set; }

        public string CMail { get; set; }

        public string CCountry { get; set; }
    }
}
