using Microsoft.AspNetCore.Mvc;
using YouTubeBuddy.API.Models;
using YouTubeBuddy.API.Services;

namespace YouTubeBuddy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoController : ControllerBase
{
    private readonly ILogger<VideoController> _logger;
    private readonly IVideoService _videoService;

    public VideoController(ILogger<VideoController> logger, IVideoService videoService)
    {
        _logger = logger;
        _videoService = videoService;
    }

    [HttpGet(Name = "SearchVideos")]
    public async Task<VideoResponse> Get([FromQuery] string q, [FromQuery] string? pageToken)
    {
        return await _videoService.SearchVideos(q, pageToken);
    }
}