using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace East_Vantage.Models
{
    public class tbl_Technician
    {
        [Key]
        public string TechnicianId { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        public bool Active { get; set; }
    }
}