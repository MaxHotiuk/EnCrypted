var builder = WebApplication.CreateBuilder(args);

// Add YARP configuration
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Register ServerManager as a singleton
builder.Services.AddSingleton<ServerManager>();

var app = builder.Build();

// Start two servers on boot (on ports 5173 and 5172)
var serverManager = app.Services.GetRequiredService<ServerManager>();
serverManager.StartNewServer(5173);  // Start first server on port 5173
serverManager.StartNewServer(5172);  // Start second server on port 5172
serverManager.StopServer(5172);       // Stop second server on port 5172

// Middleware to dynamically add more servers based on load
app.UseRouting();

// Map the reverse proxy
app.MapReverseProxy();

// Register shutdown event to dispose of the ServerManager
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    serverManager.Dispose();
});

app.Run();
