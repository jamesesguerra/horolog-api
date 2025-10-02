namespace horolog_api.Features.WatchRecords;

public class WatchRecord
{
    public int Id { get; set; }
    public int? ModelId { get; init; }
    public string? ImageUrl { get; init; }
    public string? Description { get; init; }
    public string? Material { get; init; }
    public DateTime? DatePurchased { get; init; }
    public DateTime? DateReceived { get; init; }
    public DateTime? DateSold { get; init; }
    public DateTime? DateBorrowed { get; init; }
    public DateTime? DateReturned { get; init; }
    public DateTime? DatePickedUp { get; init; }
    public string? ReferenceNumber { get; init; }
    public string? SerialNumber { get; init; }
    public string? Location { get; init; }
    public bool? HasBox { get; init; }
    public bool? HasPapers { get; init; }
    public bool? IsConsigned { get; init; }
    public bool? IsWatchVault { get; init; }
    public bool? IsConsignedBySvg { get; init; }
    public long? Cost { get; init; }
    public string? Remarks { get; init; }
    public DateTime? CreatedAt { get; set; }
}