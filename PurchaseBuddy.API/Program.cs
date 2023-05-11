using Microsoft.OpenApi.Models;
using PurchaseBuddy.API;
using PurchaseBuddy.Database;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.stores.app;
using PurchaseBuddyLibrary.src.stores.persistance;

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

var configBuilder = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = configBuilder.Build();
var databaseConnectionString = configuration.GetConnectionString("Database");
if (databaseConnectionString == null)
	throw new ArgumentException("databsae connection string is invalid");

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
							//policy.WithOrigins("http://localhost:4200");
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