using Microsoft.EntityFrameworkCore;
using WorkerService_Executor.Core;
using Lib.DataTypes.EF;

namespace WorkerService_Executor.EF
{
    public class AppDbContext : DbContext
    {
        public DbSet<TrackLog_Files> TrackLog_Files { get; set; }
        public DbSet<Data_Identifiers> Data_Identifiers { get; set; }
        public DbSet<Data_IdentifiersDetails> Data_IdentifiersDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(AppData.ConnectionString);
            }
        }
    }
}
