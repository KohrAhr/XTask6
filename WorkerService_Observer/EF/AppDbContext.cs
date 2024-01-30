using Microsoft.EntityFrameworkCore;
using WorkerService_Observer.Core;
using Lib.DataTypes.EF;

namespace WorkerService_Observer.EF
{
    public class AppDbContext : DbContext
    {
        public DbSet<Config_Folders> Config_Folders { get; set; }
        public DbSet<TrackLog_Files> TrackLog_Files { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(AppData.ConnectionString);
            }
        }
    }
}
