using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UIShared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSharedServices();

await builder.Build().RunAsync();
