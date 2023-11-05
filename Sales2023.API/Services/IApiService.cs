using Sales2023.Shared.Responses;

namespace Sales2023.API.Services
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(string servicePrefix, string controller);
    }
}
