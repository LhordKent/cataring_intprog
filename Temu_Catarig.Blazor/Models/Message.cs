namespace Temu_Catarig.Blazor.Models;

public class Message
{
    public string Id { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public bool IsOutgoing { get; set; }
    public string TimeFormatted => Timestamp.ToString("MMM dd, h:mm tt");
}
