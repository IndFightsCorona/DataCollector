using System.Data.Entity;
using FightCorona.DataCollector.Data.Models;

namespace FightCorona.DataCollector.Data
{
    public class StatisticsContext : DbContext
    {
        public StatisticsContext()
            : base("DBConnectionString")
        {
            Database.SetInitializer(new StatisticsDbInitializer());
        }
        public DbSet<Statistics> Statistics { get; set; }

        public DbSet<OverallStatistics> OverallStatistics { get; set; }

        public DbSet<ReaderStatus> ReaderStatus { get; set; }

        public DbSet<ContactUs> ContactUs { get; set; }

        public DbSet<CountryStatus> CountriesStatus { get; set; }

        public DbSet<DistrictStatus> DistrictsStatus { get; set; }
    }
}
