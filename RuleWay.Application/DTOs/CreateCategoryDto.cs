namespace RuleWay.Application.DTOs
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;

        public int MinimumStockQuantity { get; set; }
    }
}