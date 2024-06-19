using Microsoft.EntityFrameworkCore;
using Infra.DB; 
using Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Domain.Options;
using System.Text;
using Services;
using Infra;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;



var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IUnsubscriptionService, UnsubscriptionService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer { Token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

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


builder.Services.AddScoped<IUnsubscriptionService, UnsubscriptionService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<ToDoListHttpClient>();
builder.Services.AddScoped<HttpClient>();

builder.Services.AddDbContext<MyDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AceleraDev"),
        sqlOptions => sqlOptions.MigrationsAssembly("Infra")));


var tokenOptions = builder.Configuration.GetSection("Token").Get<TokenOptions>();
if (tokenOptions == null || string.IsNullOrEmpty(tokenOptions.Key))
{
	throw new ArgumentNullException(nameof(tokenOptions.Key), "Token key must be configured.");
}
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.Key));

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


var httpClient = new HttpClient();
var todoListClient = new ToDoListHttpClient(httpClient, configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors("AllowAllHeaders");

app.UseMiddleware(typeof(ExceptionHandler));

app.UseAuthorization();

app.MapControllers();

app.Run();
