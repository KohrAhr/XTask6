using System.ComponentModel.DataAnnotations;

namespace Lib.DataTypes.EF
{
    public class TrackLog_Files
    {
        [Key]
        public Int64 TrackFileId { get; set; }

        [MaxLength(500)]
        public string FileFullPath { get; set; } = string.Empty;
        public DateTime? FileStartProceedTime { get; set; }
        public DateTime? FileFinishProceedTime { get; set; }
        public Int64 EntriesInFilesOK { get; set; }
        public Int64 EntriesInFilesFailed { get; set; }

        public bool? OverallSuccessStatus { get; set; }

        [MaxLength(500)]
        public string? ErrorMessage { get; set; }
    }
}
