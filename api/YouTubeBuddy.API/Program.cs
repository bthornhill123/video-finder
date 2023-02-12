using YouTubeBuddy.API.Middlewares;
using YouTubeBuddy.API.Services;

namespace YouTubeBuddy.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

        // Add YouTube service
        builder.Services.AddSingleton<IVideoService, YoutubeService>();
        builder.Services.AddHttpClient<IVideoService, YoutubeService>(httpClient =>
        {
            string? youtubeApiBaseUrl = builder.Configuration.GetValue<string>("YouTube:BASE_URL");
            httpClient.BaseAddress = new Uri(youtubeApiBaseUrl ?? "https://www.googleapis.com/youtube/v3/");
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        app.UseAuthorization();

        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

        app.MapControllers();

        app.Run();
    }
}

