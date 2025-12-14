using System.Text;
using AlertrixAPI.Application.Interfaces;
using AlertrixAPI.Application.Services;
using AlertrixAPI.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
if (string.IsNullOrEmpty(mongoConnectionString))
{
    throw new Exception("MONGO_CONNECTION_STRING environment variable is not set.");
}
builder.Services.Configure<MongoSettings>(options =>
{
    options.ConnectionString = mongoConnectionString;
    options.DatabaseName = "Alertrix";
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var jwtOptions = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtOptions?.Secret ?? "");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions?.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions?.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
