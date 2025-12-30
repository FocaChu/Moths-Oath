using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MothsOath.BlazorUI;
using MothsOath.Core;
using MothsOath.Core.Factories;
using MothsOath.Core.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HttpClient as singleton for BlueprintLoader
var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
builder.Services.AddSingleton(httpClient);
builder.Services.AddScoped(sp => httpClient);

// Register BlueprintLoader with HttpClient for WebAssembly
builder.Services.AddSingleton<BlueprintLoader>(sp => new BlueprintLoader(httpClient));

// Register BlueprintCache
builder.Services.AddSingleton<BlueprintCache>();

// Register factories that don't depend on blueprints first
builder.Services.AddSingleton<PassiveEffectFactory>();
builder.Services.AddSingleton<BehaviorFactory>();
builder.Services.AddSingleton<ActionFactory>();

// Register factories that depend on BlueprintCache (will be initialized after cache)
builder.Services.AddSingleton<CardFactory>();
builder.Services.AddSingleton<NpcFactory>();
builder.Services.AddSingleton<PlayerFactory>();
builder.Services.AddSingleton<StateFactory>();
builder.Services.AddSingleton<GameStateManager>();

var host = builder.Build();

try
{
    // Initialize BlueprintCache asynchronously before starting the app
    var blueprintCache = host.Services.GetRequiredService<BlueprintCache>();
    await blueprintCache.InitializeAsync();

    // Hide loading screen after initialization
    var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
    await jsRuntime.InvokeVoidAsync("hideLoadingScreen");
}
catch (Exception ex)
{
    Console.WriteLine($"Error during initialization: {ex.Message}");
}

await host.RunAsync();
