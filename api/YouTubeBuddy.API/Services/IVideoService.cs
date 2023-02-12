using YouTubeBuddy.API.Models;

namespace YouTubeBuddy.API.Services
{
    public interface IVideoService
    {
        Task<VideoResponse> SearchVideos(string query, string? nextPageToken);
    }
}