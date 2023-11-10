namespace Lib.DataTypes
{
    public class Box
    {
        public string SupplierIdentifier { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public List<Content> Contents { get; set; } = new List<Content>();

        public class Content
        {
            public string PoNumber { get; set; } = string.Empty;
            public string Isbn { get; set; } = string.Empty;
            public int Quantity { get; set; }
        }
    }
}