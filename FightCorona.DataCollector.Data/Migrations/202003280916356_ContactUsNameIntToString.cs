namespace WebScrapingService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactUsNameIntToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContactUs", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ContactUs", "Name", c => c.Int(nullable: false));
        }
    }
}
