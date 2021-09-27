using Blazored.LocalStorage;
using Client.Providers;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<InMemoryDatabaseCache>();

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddAuthorizationCore();

            builder.Services.AddScoped<AppAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
                provider.GetRequiredService<AppAuthenticationStateProvider>());

            await builder.Build().RunAsync();
        }
    }
}
