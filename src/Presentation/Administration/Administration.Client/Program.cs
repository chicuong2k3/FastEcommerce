using Administration.Client;
using Administration.Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorAutoBridge.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


/* Share Services */
builder.Services.AddSharedServices();
/* Share Services */

builder.Services.AddBlazorAutoBridge((sp, client) =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}forwarders"); // don't change this
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddAuthorizationCore(options =>
{

});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();


await builder.Build().RunAsync();
