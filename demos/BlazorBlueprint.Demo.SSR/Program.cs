using BlazorBlueprint.Demo.Extensions;
using BlazorBlueprint.Demo.SSR;

var builder = WebApplication.CreateBuilder(args);

// Add Razor components with interactive server support.
// Interactive server components are registered so that individual components
// can opt-in to interactivity via @rendermode="InteractiveServer", but the
// app-level default is static SSR (no rendermode on Routes).
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBlueprintDemo();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Only map this project's own assembly — we use our own Routes, not the shared demo's.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
