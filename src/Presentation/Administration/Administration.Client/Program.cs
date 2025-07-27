using Administration.Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UIShared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSharedServices();

builder.Services.AddAuthorizationCore(options =>
{

});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();


await builder.Build().RunAsync();
