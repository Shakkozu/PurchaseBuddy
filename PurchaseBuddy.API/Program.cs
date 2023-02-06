using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;
using System.Security.Claims;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();

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
