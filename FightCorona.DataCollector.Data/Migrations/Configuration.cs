namespace FightCorona.DataCollector.Data.Migrations
{
    using System.Data.Entity.Migrations;
    using FightCorona.DataCollector.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<StatisticsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

    }
}
