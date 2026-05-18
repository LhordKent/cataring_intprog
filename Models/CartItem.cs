namespace Temu_Catarig.Models;

public class CartItem
{
    public string ProductId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Quantity { get; set; } = 1;
    public string SellerId { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;

    public string TotalPriceFormatted => $"₱{(Price * Quantity):N0}";
    public string PriceFormatted => $"₱{Price:N0}";
}
