using YouTubeBuddy.API.Models;

namespace YouTubeBuddy.API.Services
{
    public class YoutubeService : IVideoService
    {
        private record YouTubeSearchResult(string NextPageToken, List<Item> Items);
        private record Item(Id Id, Snippet Snippet);
        private record Id(string VideoId);
        private record Snippet(string Title, string Description, Dictionary<string, Thumbnail> Thumbnails);
        private record Thumbnail(string Url, int Width, int Height);

        private record YouTubeVideoResult(List<YouTubeVideoItem> Items);
        private record YouTubeVideoItem(string Id, YouTubeVideoSnippet Snippet);
        private record YouTubeVideoSnippet(List<string> Tags);

        private readonly ILogger<YoutubeService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public YoutubeService(ILogger<YoutubeService> logger, HttpClient httpClient, IConfiguration config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<VideoResponse> SearchVideos(string query, string? nextPageToken)
        {
            string? API_KEY = _config.GetValue<string>("YouTube:API_KEY");
            if (API_KEY == null)
            {
                throw new ArgumentNullException("No YouTube API key was supplied");
            }

            var searchResult = await GetSearchResults(query, API_KEY, nextPageToken);
            var videoIds = searchResult.Items.Select(item => item.Id.VideoId);
            var videoTagsMap = await GetVideoTags(videoIds, API_KEY);

            var videos = searchResult.Items
                .Select(item => new Video(
                    item.Id.VideoId,
                    item.Snippet.Title,
                    item.Snippet.Description,
                    item.Snippet.Thumbnails["high"].Url,
                    videoTagsMap[item.Id.VideoId].Snippet.Tags ?? new List<string>()
                ));

            return new VideoResponse(searchResult.NextPageToken, videos.ToList());
        }

        private async Task<YouTubeSearchResult> GetSearchResults(string query, string apiKey, string? nextPageToken)
        {
            string queryParams = $"part=snippet&type=video&q={query}&key={apiKey}";
            if (nextPageToken != null)
            {
                queryParams += $"&pageToken={nextPageToken}";
            }

            var searchResult = await _httpClient.GetFromJsonAsync<YouTubeSearchResult>($"search?{queryParams}");
            if (searchResult == null)
            {
                throw new ArgumentNullException("No search results were returned");
            }

            return searchResult;
        }

        private async Task<Dictionary<string, YouTubeVideoItem>> GetVideoTags(IEnumerable<string> videoIds, string apiKey)
        {
            string commaSeparatedVideoIds = String.Join(",", videoIds);
            string queryParams = $"part=snippet,contentDetails,statistics&id={commaSeparatedVideoIds}&key={apiKey}";
            var videosResult = await _httpClient.GetFromJsonAsync<YouTubeVideoResult>($"videos?{queryParams}");
            if (videosResult == null)
            {
                throw new Exception($"No results found for video ids: {commaSeparatedVideoIds}");
            }

            return videosResult.Items.ToDictionary(item => item.Id);
        }
    }
}