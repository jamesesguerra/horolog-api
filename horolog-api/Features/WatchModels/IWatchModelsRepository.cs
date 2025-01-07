namespace horolog_api.Features.WatchModels;

public interface IWatchModelsRepository
{
    Task<IEnumerable<WatchModel>> GetWatchModelsByBrandId(int id);
    Task<WatchModel> AddWatchModel(WatchModel watchModel);
    Task<IEnumerable<int>> GetIndependentBrandModelIds();
}