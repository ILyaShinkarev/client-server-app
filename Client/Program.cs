using Client.Components;
using Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Регистрация HTTP-клиента для API
builder.Services.AddHttpClient("api", c =>
{
    c.BaseAddress = new Uri("https://localhost:8080");  // Адрес серверного API
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<ApiService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();