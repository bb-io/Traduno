namespace Apps.Traduno.Polling;

public class TradunoStatusChangeMemory
{
    public DateTime? LastCheckedAt { get; set; }

    public string? EntityId { get; set; }

    public string? LastStatus { get; set; }
}
