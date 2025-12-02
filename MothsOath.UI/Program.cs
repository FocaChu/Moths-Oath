using MothsOath.Core;
using MothsOath.Core.Factories;
using MothsOath.Core.Services;
using MothsOath.UI.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<BlueprintLoader>();
builder.Services.AddSingleton<ActionFactory>();
builder.Services.AddSingleton<EnemyFactory>();
builder.Services.AddSingleton<CardFactory>();
builder.Services.AddSingleton<PlayerFactory>();
builder.Services.AddSingleton<StateFactory>();
builder.Services.AddSingleton<GameStateManager>();

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
