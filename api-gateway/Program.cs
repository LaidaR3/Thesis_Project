using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Ocelot services
builder.Services.AddOcelot();

var app = builder.Build();

// Ocelot middleware
await app.UseOcelot();

app.Run();
