using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

var key = builder.Configuration["Jwt:Key"];

// 1️⃣ Register AUTH FIRST
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var auth = context.Request.Headers["Authorization"].FirstOrDefault()
                       ?? context.Request.Headers["authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer "))
            {
                context.Token = auth.Substring("Bearer ".Length);
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// 2️⃣ THEN register Ocelot (so it sees the auth schemes)
builder.Services.AddOcelot();

var app = builder.Build();

// Debug header middleware
app.Use(async (context, next) =>
{
    Console.WriteLine("====== GATEWAY AUTHORIZATION DEBUG ======");
    Console.WriteLine($"Authorization: {context.Request.Headers["Authorization"]}");
    Console.WriteLine($"authorization: {context.Request.Headers["authorization"]}");
    Console.WriteLine("==========================================");
    await next();
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();
app.Run();
