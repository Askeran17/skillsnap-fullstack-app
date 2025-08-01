using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SkillSnap.Client;
using SkillSnap.Client.Services;
using System.Net.Http;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 🧠 Загружаем конфигурацию через HttpClient
using var tempClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var config = await tempClient.GetFromJsonAsync<Dictionary<string, string>>("appsettings.json");

var apiBaseUrl = config?["ApiBaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
    throw new InvalidOperationException("❌ ApiBaseUrl не найден в appsettings.json");

// ✅ Регистрируем HttpClient с базовым адресом
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// 🧠 Пользовательские сервисы
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserSessionService>();
builder.Services.AddScoped<ProjectService>();

await builder.Build().RunAsync();








