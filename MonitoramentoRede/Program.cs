using Microsoft.AspNetCore.Connections;
using MonitoramentoRede.Hubs;
using System.Net;
using Microsoft.AspNetCore.HttpsPolicy;

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configuração do SignalR
builder.Services.AddSignalR()
    .AddMessagePackProtocol();

// Serviços personalizados
builder.Services.AddHostedService<NetworkMonitoringService>();
builder.Services.AddTransient<IStartupFilter, PortCheckStartupFilter>();

// Configuração de portas via configuração
var httpPort = builder.Configuration.GetValue("HttpPort", builder.Environment.IsDevelopment() ? 5110 : 8080);
var httpsPort = builder.Configuration.GetValue("HttpsPort", builder.Environment.IsDevelopment() ? 7111 : 8443);

// Configuração do Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    try
    {
        options.ListenAnyIP(httpPort);
        options.ListenAnyIP(httpsPort, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    }
    catch (System.Net.Sockets.SocketException ex)
    {
        Console.WriteLine($"Erro ao tentar escutar nas portas {httpPort}/{httpsPort}: {ex.Message}");
        throw;
    }
});

// Redirecionamento HTTPS
builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = httpsPort;
});

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Endpoints
app.MapRazorPages();
app.MapHub<NetworkMonitorHub>("/networkMonitorHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
