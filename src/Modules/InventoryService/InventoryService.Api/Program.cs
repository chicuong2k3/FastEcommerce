using InventoryService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInventoryModule(builder.Configuration);
builder.AddSharedApi(builder.Configuration);


var app = builder.Build();

app.UseSharedApi(builder.Configuration);


app.Run();