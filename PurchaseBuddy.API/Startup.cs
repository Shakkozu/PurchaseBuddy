using Microsoft.OpenApi.Models;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;

namespace PurchaseBuddy.API;

public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(c =>
		{
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Please enter session id in field below",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
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

		services.AddSingleton<IUserRepository, InMemoryUserRepository>();
		services.AddSingleton<IUserAuthorizationService, AuthorizationService>();

		services.AddAuthentication("CustomHeaderAuthentication")
			.AddScheme<CustomHeaderAuthenticationOptions, CustomHeaderAuthenticationHandler>(
				"CustomHeaderAuthentication", options =>
				{
					options.HeaderName = "Authorization";
				});


		var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
		services.AddCors(options =>
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
	}

	public void Configure(IApplicationBuilder app)
	{
		
	}
}
