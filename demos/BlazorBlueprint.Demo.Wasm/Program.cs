using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorBlueprint.Demo.Extensions;
using BlazorBlueprint.Demo.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add all demo services via shared extension method
builder.Services.AddBlazorBlueprintDemo();

await builder.Build().RunAsync();
