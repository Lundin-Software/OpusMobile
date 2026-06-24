namespace Opus.Mobile.Shared.Orders;

public class OrderItem
{
    public int Id { get; set; }

    public int? OrderNr { get; set; }

    public string? Description { get; set; }

    public string? Supplier { get; set; }

    public string? Department { get; set; }

    public bool Received { get; set; }

    public string Location { get; set; } = string.Empty;
}
