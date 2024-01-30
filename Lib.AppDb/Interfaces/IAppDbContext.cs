using Lib.DataTypes.EF;
using Microsoft.EntityFrameworkCore;

namespace Lib.AppDb.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Data_Identifiers> Data_Identifiers { get; set; }
        DbSet<Data_IdentifiersDetails> Data_IdentifiersDetails { get; set; }

        DbSet<Config_Folders> Config_Folders { get; set; }

        DbSet<TrackLog_Files> TrackLog_Files { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aConnectionString"></param>
        void SetConnectionString(string aConnectionString);

        // Methods we need from DbContext
        abstract int SaveChanges();
        void Dispose();
    }
}
