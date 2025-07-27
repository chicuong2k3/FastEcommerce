using Shared.AuthZ;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthZ(builder.Configuration);

var app = builder.Build();

app.UseAuthZ();

app.MapReverseProxy();

app.Run();
