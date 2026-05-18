namespace Temu_Catarig.Models;

public class Product
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public string ImageUrl { get; set; } = string.Empty;
    public double Price { get; set; }
    public double Rating { get; set; } = 4.0;
    public int ReviewsCount { get; set; } = 0;
    public string SellerId { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public int Stock { get; set; } = 10;

    // Status: "Pending", "Approved", "Rejected"
    public string Status { get; set; } = "Pending";

    // For UI display
    public string PriceFormatted => $"₱{Price:N0}";
    public string RatingFormatted => $"★ {Rating:F2}";
}
