// using Ocelot.DependencyInjection;
// using Ocelot.Middleware;

// var builder = WebApplication.CreateBuilder(args);

// // Load Ocelot config
// builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// // Register Ocelot ONLY
// builder.Services.AddOcelot();

// var app = builder.Build();




// await app.UseOcelot();
// app.Run();



using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 🔐 IMPORTANT: Scheme name MUST be "Bearer"
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "auth-service",
            ValidAudience = "api-gateway",

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();
app.Run();
