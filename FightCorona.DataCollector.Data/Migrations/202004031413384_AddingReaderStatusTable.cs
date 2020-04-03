namespace FightCorona.DataCollector.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingReaderStatusTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReaderStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReaderName = c.String(nullable: false),
                        Version = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.LastUpdates");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.LastUpdates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.ReaderStatus");
        }
    }
}
