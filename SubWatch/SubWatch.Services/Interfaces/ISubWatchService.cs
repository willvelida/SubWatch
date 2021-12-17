using Microsoft.AspNetCore.Http;

namespace SubWatch.Services.Interfaces
{
    public interface ISubWatchService
    {
        Task AddSubscripion(HttpRequest httpRequest);
    }
}
