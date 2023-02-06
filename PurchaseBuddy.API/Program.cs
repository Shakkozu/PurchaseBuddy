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
builder.Services.AddSingleton<IUserSessionCache, UserSessionCache>();
builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();
// configure auth to keep session in in authorization cookie
// configure auth to keep session in authorization header

// configure authentication to use authorizaation header containing session identifier
builder.Services.AddAuthentication("CustomHeaderAuthentication")
	.AddScheme<CustomHeaderAuthenticationOptions, CustomHeaderAuthenticationHandler>(
		"CustomHeaderAuthentication", options =>
		{
			options.HeaderName = "Authorization";
		});





//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//	.AddCookie(options =>
//	{
//		options.Cookie.Name = "auth";
//		options.Cookie.HttpOnly = false;
//		options.Cookie.SameSite = SameSiteMode.Strict;
//		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//		options.LoginPath = "/auth/login";
//		options.LogoutPath = "/auth/logout";
//		options.AccessDeniedPath = "/auth/accessdenied";
//		options.ExpireTimeSpan = new TimeSpan(0, 0, 30, 0);
//		options.SlidingExpiration = true;
//		options.Events.OnRedirectToLogin = context =>
//		{
//			context.Response.StatusCode = 401;
//			return Task.CompletedTask;
//		};
//		options.Events.OnRedirectToAccessDenied = context =>
//		{
//			context.Response.StatusCode = 403;
//			return Task.CompletedTask;
//		};
//		options.Events.OnRedirectToLogout = context =>
//		{
//			context.Response.StatusCode = 200;
//			return Task.CompletedTask;
//		};
//	});

// add localhost:4200 to cors policy
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


builder.Services.AddSwaggerGen(option =>
	option.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
	{
		BearerFormat = "",
		Description = "token auth",
		Scheme = CookieAuthenticationDefaults.AuthenticationScheme,

	}));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.UseCors(builder =>
//	   builder
//		   .AllowAnyOrigin()
//		   .AllowAnyMethod()
//		   .AllowAnyHeader());
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();







app.Run();
