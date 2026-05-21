namespace Temu_Catarig.Blazor.Models;

public class Conversation
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductTitle { get; set; } = string.Empty;
    public string BuyerId { get; set; } = string.Empty;
    public string SellerId { get; set; } = string.Empty;
    public string BuyerName { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastMessageDate { get; set; } = DateTime.UtcNow;

    public string GetOtherParticipantName(string currentUserId)
    {
        return currentUserId == SellerId ? BuyerName : SellerName;
    }

    public string DateFormatted => LastMessageDate.ToString("MMM dd");
}
