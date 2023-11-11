using System.ComponentModel.DataAnnotations;

namespace Lib.DataTypes.EF
{
    public class Config_Folders
    {
        [Key]
        public Int64 FolderId { get; set; }

        [MaxLength(500)]
        public string FolderToObserver { get; set; } = string.Empty;

        [MaxLength(50)]
        public string FilePattern { get; set; } = string.Empty;

        public bool FolderIsActive { get; set; }
        public Int64 AssignToObserver { get; set; }

        [MaxLength(50)]
        public string? Template_HeaderStartWith { get; set; }

        [MaxLength(50)]
        public string? Template_LineStartWith { get; set; }

        [MaxLength(500)]
        public string? Template_HeaderDataRegex { get; set; }

        [MaxLength(500)]
        public string? Template_HeaderLineRegex { get; set; }
    }

}
