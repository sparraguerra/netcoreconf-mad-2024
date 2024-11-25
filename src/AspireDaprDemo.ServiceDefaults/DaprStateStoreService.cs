public interface IDaprStateStoreService
{
    Task DeleteStateAsync(string key, string bindingName, CancellationToken cancellationToken);
    Task DeleteStateAsync(string key, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken);
    Task<T> GetStateAsync<T>(string key, string bindingName, CancellationToken cancellationToken);
    Task<T> GetStateAsync<T>(string key, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken);
    Task SaveStateAsync<T>(T value, string key, string bindingName, CancellationToken cancellationToken);
    Task SaveStateAsync<T>(T value, string key, IReadOnlyDictionary<string, string>? metadata, string bindingName,  CancellationToken cancellationToken);
    Task<IEnumerable<T>> QueryStateAsync<T>(string key, string jsonQuery, string bindingName, CancellationToken cancellationToken);
    Task<IEnumerable<T>> QueryStateAsync<T>(string key, string jsonQuery, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken);
}

public class DaprStateStoreService : IDaprStateStoreService
{
    private readonly DaprClient client;
    private readonly ILogger<DaprStateStoreService> logger;

    public DaprStateStoreService(DaprClient client, ILogger<DaprStateStoreService> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task DeleteStateAsync(string key, string bindingName, CancellationToken cancellationToken) =>
       await DeleteStateAsync(key, default, bindingName, cancellationToken);

    public async Task DeleteStateAsync(string key, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken) =>
        await client.DeleteStateAsync(bindingName, key, default, metadata, cancellationToken);

    public async Task<T> GetStateAsync<T>(string key, string bindingName, CancellationToken cancellationToken) =>
        await GetStateAsync<T>(key, default, bindingName, cancellationToken);

    public async Task<T> GetStateAsync<T>(string key, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken) =>
        await client.GetStateAsync<T>(bindingName, key, default, metadata, cancellationToken);

    public Task<IEnumerable<T>> QueryStateAsync<T>(string key, string jsonQuery, string bindingName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> QueryStateAsync<T>(string key, string jsonQuery, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task SaveStateAsync<T>(T value, string key, string bindingName, CancellationToken cancellationToken) => 
        await SaveStateAsync(value, key, default, bindingName, cancellationToken);

    public async Task SaveStateAsync<T>(T value, string key, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Saving state {Value} to {BindingName}", value, bindingName);

        await client.SaveStateAsync<T>(bindingName, key, value, default, metadata, cancellationToken);
    }
}
