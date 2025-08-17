using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInventoryModule(builder.Configuration);
builder.AddSharedApi(builder.Configuration);


var app = builder.Build();

app.Services.MigrateInventoryDatabaseAsync().GetAwaiter().GetResult();

app.UseSharedApi(builder.Configuration);


app.Run();