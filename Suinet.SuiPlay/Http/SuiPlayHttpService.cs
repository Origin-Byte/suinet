namespace Suinet.SuiPlay.Http
{
    public class SuiPlayHttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public SuiPlayHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return _httpClient.GetAsync(url);
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return _httpClient.PostAsync(url, content);
        }

        public Task<HttpResponseMessage> PatchAsync(string url, HttpContent content)
        {
            return _httpClient.PatchAsync(url, content);
        }
    }

}
