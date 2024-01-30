using Microsoft.EntityFrameworkCore;
using Lib.DataTypes.EF;
using Lib.AppDb.Interfaces;

namespace Lib.AppDb.EF
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Data_Identifiers> Data_Identifiers { get; set; }
        public DbSet<Data_IdentifiersDetails> Data_IdentifiersDetails { get; set; }

        public DbSet<Config_Folders> Config_Folders { get; set; }

        public DbSet<TrackLog_Files> TrackLog_Files { get; set; }

        private string _dbConnectionString = string.Empty;
        
        public void SetConnectionString(string aConnectionString)
        {
            _dbConnectionString = aConnectionString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_dbConnectionString);
            }
        }
    }
}
