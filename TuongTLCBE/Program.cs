using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string dbConn = await VaultHelper.GetSecrets("dbconn");
string jwtToken = await VaultHelper.GetSecrets("jwt");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Auth", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(jwtToken)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddDbContext<TuongTlcdbContext>(
        options => options.UseSqlServer(dbConn));

builder.Services.AddCors(p => p.AddPolicy("AllowOrigin", builder =>
{
    _ = builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddScoped<UserService>();
builder.Services.AddTransient<RefreshTokenRepo>();
builder.Services.AddTransient<UserRepo>();
WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{

}

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

