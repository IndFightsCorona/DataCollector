namespace WebScrapingService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverallStatisticsColumnChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OverallStatistics", "LastUpdatedTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.OverallStatistics", "CreatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OverallStatistics", "CreatedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.OverallStatistics", "LastUpdatedTime");
        }
    }
}
