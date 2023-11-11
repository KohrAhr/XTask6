using System.ComponentModel.DataAnnotations;

namespace Lib.DataTypes.EF
{
    public class Data_Identifiers
    {
        [Key]
        public Int64 InId { get; set; }
        public string InSupplierId { get; set; } = string.Empty;
        public string InIdentifier { get; set; } = string.Empty;
        public Int64 TrackFileId { get; set; }
    }
}
