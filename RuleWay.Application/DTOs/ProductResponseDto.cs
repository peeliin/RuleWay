namespace RuleWay.Application.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public bool IsLive { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
    }
}