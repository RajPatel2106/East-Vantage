using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace East_Vantage.Models
{
    public class tbl_WorkOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkOrderId { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Work order date is required.")]
        public DateTime WorkOrderDate { get; set; }

        [Required(ErrorMessage = "Reference number is required.")]
        public string TechnicianId { get; set; }
        [ForeignKey("TechnicianId")]
        public virtual tbl_Technician Technician { get; set; }
    }
}