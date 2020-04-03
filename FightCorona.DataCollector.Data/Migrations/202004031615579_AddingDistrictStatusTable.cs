namespace FightCorona.DataCollector.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingDistrictStatusTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DistrictStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Location = c.String(nullable: false),
                        Confirmed = c.Int(nullable: false),
                        Active = c.Int(nullable: false),
                        Recovered = c.Int(nullable: false),
                        Deaths = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DistrictStatus");
        }
    }
}
