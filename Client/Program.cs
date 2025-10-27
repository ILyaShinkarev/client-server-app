using Client.Components;
using Client.Services;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient("api", c =>
{
    c.BaseAddress = new Uri("http://localhost:5275");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});

// DI сервисов
builder.Services.AddScoped<ApiService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

// Проксируем /api/** на backend согласно конфигу YARP
app.MapReverseProxy();

// Маршруты Razor-компонентов
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
