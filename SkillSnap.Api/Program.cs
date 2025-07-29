using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 📦 Конфигурация строки подключения и DbContext
builder.Services.AddDbContext<SkillSnapContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 Identity: пользователи, роли, токены
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<SkillSnapContext>()
.AddDefaultTokenProviders();

// 🔑 JWT-аутентификация
var config = builder.Configuration;
var jwtKey = Encoding.UTF8.GetBytes(config["Jwt:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
    };
});

// 🔐 Авторизация
builder.Services.AddAuthorization();

// 🧠 Кэширование
builder.Services.AddMemoryCache();

// 🌐 Контроллеры и CORS
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins("https://localhost:5001")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 📘 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🛠️ Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

