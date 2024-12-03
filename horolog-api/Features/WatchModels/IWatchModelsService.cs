namespace horolog_api.Features.WatchModels;

public interface IWatchModelsService
{
    Task<IEnumerable<WatchModel>> GetWatchModelsByBrandId(int id);
    Task<WatchModel> AddWatchModel(WatchModel watchModel);
}