using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string dbConn = await VaultHelper.GetSecrets("dbconn");
string jwtToken = await VaultHelper.GetSecrets("jwt");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<DecodeToken>();
builder.Services.AddTransient<UserRepo>();
builder.Services.AddTransient<CategoryRepo>();
builder.Services.AddTransient<TagRepo>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(option =>
{
    option.EnableAnnotations();

    option.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "TuongTLCBE",
            Description = "APIs for TuongTLC Website"
        }
    );

    OpenApiSecurityScheme securityScheme =
        new()
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference()
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };
    option.AddSecurityDefinition("Bearer", securityScheme);

    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement() { { securityScheme, new string[] { } } }
    );
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://tuongtlc.ddns.net",
            ValidAudience = "https://tuongtlc.ddns.net",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtToken)),
        };
    });

builder.Services.AddDbContext<TuongTlcdbContext>(options => options.UseSqlServer(dbConn));

builder.Services.AddCors(
    p =>
        p.AddPolicy(
            "AllowOrigin",
            builder =>
            {
                _ = builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }
        )
);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) { }

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
