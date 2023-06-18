using Microsoft.OpenApi.Models;
using PurchaseBuddy.API;
using PurchaseBuddy.Database;
using PurchaseBuddyLibrary.src.auth.infra;
using PurchaseBuddyLibrary.src.utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
	{
		c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			In = ParameterLocation.Header,
			Description = "Please enter session id in field below",
			Name = "Authorization",
			Type = SecuritySchemeType.ApiKey
		});
		c.AddSecurityRequirement(new OpenApiSecurityRequirement
		{{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});

//var configBuilder = new ConfigurationBuilder()
//	.AddEnvironmentVariables()
//	.AddUserSecrets<Program>();
//var configuration = configBuilder.Build();
//var databaseConnectionString = configuration.GetValue<string>("ElephantSQLConnectionURL").ToConnectionString();
var databaseConnectionString = @"postgres://mgmlhlit:MV-MWGFwmI1Hg66DjbDi8dazb1fOXakX@dumbo.db.elephantsql.com/mgmlhlit".ToConnectionString();
if (string.IsNullOrWhiteSpace(databaseConnectionString))
	throw new ArgumentException("database connection string is invalid");

MigrationsRunner.RunMigrations(builder.Services, databaseConnectionString);

PurchaseBuddyFixture.RegisterDependencies(builder.Services, databaseConnectionString);
builder.Services.AddAuthentication("CustomHeaderAuthentication")
	.AddScheme<CustomHeaderAuthenticationOptions, CustomHeaderAuthenticationHandler>(
		"CustomHeaderAuthentication", options =>
		{
			options.HeaderName = "Authorization";
		});


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: MyAllowSpecificOrigins,
						policy =>
						{
							policy.AllowAnyOrigin();
							policy.AllowAnyHeader();
							policy.AllowAnyMethod();
						});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program
{

}