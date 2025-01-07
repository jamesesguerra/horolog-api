namespace horolog_api.Features.WatchModels;

public class WatchModelsService(IWatchModelsRepository repository) : IWatchModelsService
{
    public async Task<IEnumerable<WatchModel>> GetWatchModelsByBrandId(int id)
    {
        return await repository.GetWatchModelsByBrandId(id);
    }

    public async Task<WatchModel> AddWatchModel(WatchModel watchModel)
    {
        return await repository.AddWatchModel(watchModel);
    }

    public async Task<IEnumerable<int>> GetIndependentBrandModelIds()
    {
        return await repository.GetIndependentBrandModelIds();
    }
}