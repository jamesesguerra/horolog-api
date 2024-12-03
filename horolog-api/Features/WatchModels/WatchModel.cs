namespace horolog_api.Features.WatchModels;

public class WatchModel
{
    public int Id { get; set; }
    public int BrandId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}