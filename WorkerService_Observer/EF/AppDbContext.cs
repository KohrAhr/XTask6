﻿using Microsoft.EntityFrameworkCore;
using WorkerService_Observer.Core;
using WorkerService_Observer.EF.Types;

namespace WorkerService_Observer.EF
{
    public class AppDbContext : DbContext
    {
        public DbSet<Config_Folders> Config_Folders { get; set; }
        public DbSet<TrackLog_Files> TrackLog_Files { get; set; }

        /// <summary>
        ///     Remove?
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(AppData.ConnectionString));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(AppData.ConnectionString);
            }
        }

        /// <summary>
        ///     Remove?
        /// </summary>
        /// <param name="services"></param>
        //public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        //{ 

        //}
    }
}
