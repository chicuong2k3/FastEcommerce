using Ordering.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrderingModule(builder.Configuration);
builder.AddSharedApi(builder.Configuration);


var app = builder.Build();

app.UseSharedApi(builder.Configuration);

app.Run();