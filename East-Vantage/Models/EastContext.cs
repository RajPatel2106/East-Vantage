using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace East_Vantage.Models
{
    public class EastContext: DbContext
    {
        public EastContext(): base("name=DBCS")
        {
        }

        public DbSet<tbl_Technician> tbl_Technicians { get; set; }

        public DbSet<tbl_WorkOrder> tbl_WorkOrders { get; set; }
    }
}