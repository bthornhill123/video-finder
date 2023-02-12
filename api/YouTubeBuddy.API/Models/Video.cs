namespace YouTubeBuddy.API.Models
{
    public record VideoResponse(string NextPageToken, List<Video> Videos);
    public record Video(string Id, string Title, string Description, string ThumbnailUrl, List<string> Tags);
}