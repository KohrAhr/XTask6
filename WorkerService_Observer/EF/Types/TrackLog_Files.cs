using System.ComponentModel.DataAnnotations;

namespace WorkerService_Observer.EF.Types
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

        /// <summary>
        ///     Reserved ?
        /// </summary>
        public Int64? FileSize { get; set; }
        public bool? OverallSuccessStatus { get; set; }

        [MaxLength(500)]
        public string? ErrorMessage { get; set; }
    }
}
