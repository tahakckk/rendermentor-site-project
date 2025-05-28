using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderMentor.Models
{
    [Table("Contacts", Schema = "dbo")]
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string MapUrl { get; set; }
        public string EmailSales { get; set; }
        public string EmailSupport { get; set; }
        public string Map { get; set; }
        public string TaxDistrict { get; set; }
        public string TaxNumber { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDesc { get; set; }
    }
}
