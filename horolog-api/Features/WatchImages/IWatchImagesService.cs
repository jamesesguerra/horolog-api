namespace horolog_api.Features.WatchImages;

public interface IWatchImagesService
{
    Task<int> AddWatchImages(List<WatchImage> watchImages);
    Task<IEnumerable<WatchImage>> GetWatchImagesByRecordId(int id);
    Task<int> DeleteWatchImage(int id);
}