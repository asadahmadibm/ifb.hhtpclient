namespace ifb.httpclient
{
    public interface IHttpService
    {
        Dictionary<string, string> Headers { get; }
        string RequestUri { get; set; }
        Task<O> GetAsync<O>();
        Task<O> GetAsync<O>(string methodName, params string[] args);
        Task<O> GetByIdAsync<O>(string id);
        Task<O> PostAsync<I, O>(I viewModel);
        Task<O> PostAsync<I, O>(string methodName, I viewModel);
        Task<O> PutAsync<I, O>(I model);
        Task<O> PutAsync<I, O>(string methodName, I viewModel);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteAsync(string methodName, string id);
    }
}
