namespace horolog_api.Features.WatchImages;

public interface IWatchImagesRepository
{
    Task<int> AddWatchImages(List<WatchImage> watchImages);
    Task<IEnumerable<WatchImage>> GetWatchImagesByRecordId(int id);
}