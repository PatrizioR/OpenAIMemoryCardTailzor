using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using OpenAIMemoryCardTailzor;
using OpenAIMemoryCardTailzor.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var memoryCards = builder.Configuration.GetSection("MemoryCards").Get<List<MemoryCard>>();
builder.Services.AddSingleton<List<MemoryCard>>(memoryCards ?? new List<MemoryCard>());

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
