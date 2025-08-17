using Prise.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddPrise();

var app = builder.Build();


app.Run();