using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrderingModule(builder.Configuration);
builder.AddSharedApi(builder.Configuration);


var app = builder.Build();

app.Services.MigrateOrderingDatabaseAsync().GetAwaiter().GetResult();

app.UseSharedApi(builder.Configuration);

app.Run();