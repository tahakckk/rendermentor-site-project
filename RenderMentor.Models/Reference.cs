using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Models
{
    public class Reference
    {
        [Key]
        public int Id { get; set; }
        public string Logo { get; set; }
        public int ListOrder { get; set; }
    }
}
