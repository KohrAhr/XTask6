namespace Lib.DataTypes
{
    public class DataFileResult
    {
        public int EntriesInFilesOK { get; set; }
        public int EntriesInFilesFailed { get; set; }
        public bool Suceeded { get; set; } = false;
        public string ErrorMessaget { get; set; } = string.Empty;
    }
}
