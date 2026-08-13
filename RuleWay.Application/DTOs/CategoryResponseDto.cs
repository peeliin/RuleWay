namespace RuleWay.Application.DTOs
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MinimumStockQuantity { get; set; }
    }
}