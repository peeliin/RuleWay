namespace RuleWay.Application.DTOs
{
    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;

        public int MinimumStockQuantity { get; set; }
    }
}
