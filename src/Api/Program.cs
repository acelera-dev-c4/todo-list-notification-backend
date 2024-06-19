using Microsoft.EntityFrameworkCore;
using Infra.DB;
using Api.Middlewares;
using Domain.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("Token"));
builder.Services.Configure<NotificationOptions>(builder.Configuration.GetSection("Notification"));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<UnsubscriptionService, UnsubscriptionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHttpClient<INotificationService, NotificationService>();


builder.Services.AddDbContext<MyDBContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MyMySQLConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MyMySQLConnection")),
        mysqlOptions => mysqlOptions.MigrationsAssembly("Infra")));

var tokenOptions = builder.Configuration.GetSection("Token").Get<TokenOptions>();
if (tokenOptions == null || string.IsNullOrEmpty(tokenOptions.Key))
{
    throw new ArgumentNullException(nameof(tokenOptions.Key), "Token key must be configured.");
}
var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenOptions.Key));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
      .AddJwtBearer(options =>
      {
          options.RequireHttpsMetadata = false;
          options.SaveToken = true;
          options.TokenValidationParameters = new TokenValidationParameters
          {
              IssuerSigningKey = securityKey,
              ValidateIssuerSigningKey = true,
              ValidateAudience = true,
              ValidAudience = tokenOptions.Audience,
              ValidateIssuer = true,
              ValidIssuer = tokenOptions.Issuer,
              ValidateLifetime = true
          };
      });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Para abrir o Swagger na raiz do aplicativo

    });

}

app.UseHttpsRedirection();


app.UseCors("AllowAllHeaders");

app.UseMiddleware(typeof(ExceptionHandler));

app.UseAuthorization();

app.MapControllers();

app.Run();
