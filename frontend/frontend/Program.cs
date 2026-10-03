using frontend;
using frontend.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = ResolveApiBaseUrl(
    builder.HostEnvironment.BaseAddress,
    builder.Configuration["ApiBaseUrl"]);

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});
builder.Services.AddScoped<PhotoApiService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AdminApiService>();

await builder.Build().RunAsync();

static string ResolveApiBaseUrl(string frontendBaseAddress, string? configuredApiBaseUrl)
{
    var frontendUri = new Uri(frontendBaseAddress);
    var apiPort = frontendUri.Port switch
    {
        8081 => 8080, // docker-compose
        5066 => 5299, // local IDE (http)
        _ => (int?)null
    };

    if (apiPort is null)
    {
        return configuredApiBaseUrl ?? frontendBaseAddress;
    }

    return new UriBuilder(frontendUri.Scheme, frontendUri.Host, apiPort.Value)
        .Uri
        .ToString();
}
