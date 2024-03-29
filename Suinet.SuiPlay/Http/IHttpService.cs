﻿namespace Suinet.SuiPlay.Http
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
        Task<HttpResponseMessage> PatchAsync(string url, HttpContent content);
    }
}
