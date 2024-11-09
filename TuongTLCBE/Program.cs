using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TuongTLCBE.Business;
using TuongTLCBE.Business.CacheService;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string dbConn = await VaultHelper.GetSecrets("dbconn");
string jwtToken = await VaultHelper.GetSecrets("jwt");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<PostCategoryService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<DecodeToken>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<PostCommentService>();
builder.Services.AddTransient<UserRepo>();
builder.Services.AddTransient<CategoryRepo>();
builder.Services.AddTransient<TagRepo>();
builder.Services.AddTransient<PostCategoryRepo>();
builder.Services.AddTransient<PostTagRepo>();
builder.Services.AddTransient<PostRepo>();
builder.Services.AddTransient<FileRepo>();
builder.Services.AddTransient<PostCommentRepo>();
builder.Services.AddTransient<UserInteractCommentRepo>();
builder.Services.AddTransient<UserInteractPostRepo>();
builder.Services.AddTransient<OTPCodeRepo>();
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
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };
    option.AddSecurityDefinition("Bearer", securityScheme);

    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement { { securityScheme, new string[] { } } }
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
            ValidIssuer = "https://tuongtlc.site",
            ValidAudience = "https://tuongtlc.site",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtToken))
        };
    });

builder.Services.AddDbContext<TuongTLCDBContext>(options => options.UseSqlServer(dbConn));

builder.Services.AddCors(
    p =>
        p.AddPolicy(
            "AllowOrigin",
            policyBuilder => { _ = policyBuilder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); }
        )
);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();