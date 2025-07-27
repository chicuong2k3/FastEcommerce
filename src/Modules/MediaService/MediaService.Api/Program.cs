using CloudinaryDotNet;
using MediaService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddSingleton(_ =>
{
    var cloudinary = new Cloudinary(builder.Configuration["CloudinaryUrl"] ?? throw new ArgumentNullException("CloudinaryUrl is not defined."));
    cloudinary.Api.Secure = true;
    return cloudinary;
});

var app = builder.Build();


app.Run();
