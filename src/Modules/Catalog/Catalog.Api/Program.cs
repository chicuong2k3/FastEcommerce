using Catalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCatalogModule(builder.Configuration);
builder.AddSharedApi(builder.Configuration);

var app = builder.Build();


app.UseSharedApi(builder.Configuration);



app.Run();
