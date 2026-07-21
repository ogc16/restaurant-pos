using RestaurantPOS.Web.Components;
using RestaurantPOS.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5001");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5280";

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<ApiService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var auth = sp.GetRequiredService<AuthService>();
    var logger = sp.GetRequiredService<ILogger<ApiService>>();
    var http = factory.CreateClient("Api");
    return new ApiService(http, auth, logger);
});

builder.Services.AddSingleton<AuthService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var logger = sp.GetRequiredService<ILogger<AuthService>>();
    var http = factory.CreateClient("Api");
    return new AuthService(http, logger);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
