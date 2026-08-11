namespace RuleWay.Application.DTOs
{
    public class UpdateProductDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int? CategoryId { get; set; }
    }
}