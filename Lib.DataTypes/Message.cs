namespace Lib.DataTypes
{
    public class Message
    {
        public string FileName { get; set; } = string.Empty;

        // We can use only FileName
        public Int64 TrackFileId { get; set; }

        // Ok, specification how to parse
//        public Int64 FolderId { get; set;}
    }
}
