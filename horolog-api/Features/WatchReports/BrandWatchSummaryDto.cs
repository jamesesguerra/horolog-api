namespace horolog_api.Features.WatchReports;

public class BrandWatchSummaryDto
{
    public string Brand { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public long TotalCost { get; init; }
}