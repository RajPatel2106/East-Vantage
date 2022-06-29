namespace East_Vantage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createtables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbl_Technician",
                c => new
                    {
                        TechnicianId = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TechnicianId);
            
            CreateTable(
                "dbo.tbl_WorkOrder",
                c => new
                    {
                        WorkOrderId = c.Int(nullable: false, identity: true),
                        Address = c.String(nullable: false),
                        WorkOrderDate = c.DateTime(nullable: false),
                        TechnicianId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.WorkOrderId)
                .ForeignKey("dbo.tbl_Technician", t => t.TechnicianId, cascadeDelete: true)
                .Index(t => t.TechnicianId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbl_WorkOrder", "TechnicianId", "dbo.tbl_Technician");
            DropIndex("dbo.tbl_WorkOrder", new[] { "TechnicianId" });
            DropTable("dbo.tbl_WorkOrder");
            DropTable("dbo.tbl_Technician");
        }
    }
}
