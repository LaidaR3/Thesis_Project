using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Load JWT key
var key = builder.Configuration["Jwt:Key"];

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            // Your token uses this schema
            RoleClaimType = ClaimTypes.Role
        };

        // FIX lowercase Authorization header from Ocelot
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var auth = context.Request.Headers["Authorization"].FirstOrDefault()
                           ?? context.Request.Headers["authorization"].FirstOrDefault();

                if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer "))
                    context.Token = auth.Substring("Bearer ".Length);

                return Task.CompletedTask;
            }
        };
    });

// Authorization
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// AUTHENTICATION MUST BE ENABLED HERE
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
