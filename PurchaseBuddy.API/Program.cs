using Microsoft.OpenApi.Models;
using PurchaseBuddy.API;
using PurchaseBuddy.Database;
using PurchaseBuddyLibrary.src.auth.infra;

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
});

var config = builder.Configuration
	.SetBasePath(builder.Environment.ContentRootPath)
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
	.AddEnvironmentVariables()
	.Build();

var databaseConnectionString = ConnectionStringProvider.GetConnectionString(config);
if(string.IsNullOrEmpty(databaseConnectionString))
	throw new ArgumentNullException(nameof(databaseConnectionString));

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