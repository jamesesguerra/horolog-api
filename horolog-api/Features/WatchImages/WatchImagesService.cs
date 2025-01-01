namespace horolog_api.Features.WatchImages;

public class WatchImagesService(IWatchImagesRepository repository) : IWatchImagesService
{
    public async Task<int> AddWatchImages(List<WatchImage> watchImages)
    {
        return await repository.AddWatchImages(watchImages);
    }

    public async Task<IEnumerable<WatchImage>> GetWatchImagesByRecordId(int id)
    {
        return await repository.GetWatchImagesByRecordId(id);
    }

    public async Task<int> DeleteWatchImage(int id)
    {
        return await repository.DeleteWatchImage(id);
    }
}