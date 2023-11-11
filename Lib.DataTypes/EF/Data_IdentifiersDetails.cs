using System.ComponentModel.DataAnnotations;

namespace Lib.DataTypes.EF
{
    public class Data_IdentifiersDetails
    {
        [Key]
        public Int64 Id { get; set; }
        public Int64? IdentifierId { get; set; }
        [MaxLength(50)]
        public string? PoNumber { get; set; }
        [MaxLength(50)]
        public string? ISBN { get; set; }
        public int Qty { get; set; }
    }
}
