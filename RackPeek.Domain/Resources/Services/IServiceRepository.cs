namespace RackPeek.Domain.Resources.Services;

public interface IServiceRepository
{
    Task<int> GetCountAsync();
    Task<int> GetIpAddressCountAsync();

    Task<IReadOnlyList<Service>> GetAllAsync();
    Task AddAsync(Service service);
    Task UpdateAsync(Service service);
    Task DeleteAsync(string name);
    Task<Service?> GetByNameAsync(string name);
    Task<IReadOnlyList<Service>> GetBySystemHostAsync(string name);
}