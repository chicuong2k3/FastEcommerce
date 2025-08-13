using CloudinaryDotNet;
using MediaService.Api.Services;
using Shared.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedApi(builder.Configuration);

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddSingleton(_ =>
{
    var apiKey = builder.Configuration["Cloudinary:ApiKey"] ?? throw new ArgumentNullException("Cloudinary:ApiKey is not defined.");
    var apiSecret = builder.Configuration["Cloudinary:ApiSecret"] ?? throw new ArgumentNullException("Cloudinary:ApiSecret is not defined.");
    var cloudName = builder.Configuration["Cloudinary:CloudName"] ?? throw new ArgumentNullException("Cloudinary:CloudName is not defined.");
    var cloudinary = new Cloudinary($"cloudinary://{apiKey}:{apiSecret}@{cloudName}");
    cloudinary.Api.Secure = true;
    return cloudinary;
});

var app = builder.Build();


app.UseSharedApi(builder.Configuration);

app.Run();
