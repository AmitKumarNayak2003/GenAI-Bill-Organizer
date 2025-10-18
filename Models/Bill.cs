namespace BillOrganizerAPI.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string MerchantName { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ExtractedText { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public List<BillItem> Items { get; set; } = new List<BillItem>();
    }

    public class BillItem
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
