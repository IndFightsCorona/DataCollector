namespace WebScrapingService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CountriesStatusTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CountryStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Location = c.String(nullable: false),
                        Confirmed = c.Int(nullable: false),
                        Deaths = c.Int(nullable: false),
                        Recovered = c.Int(nullable: false),
                        Active = c.Int(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CountryStatus");
        }
    }
}
