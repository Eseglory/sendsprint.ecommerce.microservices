using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace sendsprint.ecommerce.Common.Swagger
{
    public static class Extensions
    {
        public static IServiceCollection AddSwaggerService(this IServiceCollection services, IConfiguration configuration)
        {
            SwaggerOptions options = new SwaggerOptions
            {
                Title = configuration["Swagger:Title"] ?? "sendsprint.ecommerce.microservices",
                Version = configuration["Swagger:Version"] ?? "v1",
                Build = configuration["BuildNumber"] ?? "1.0.0"
            };

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    options.Version,
                    new OpenApiInfo
                    { Title = $"{options.Title}", Version = $"{options.Version}", Description = $"Build {options.Build}" }
                    );

                c.SwaggerDoc(
                    "partnerapi",
                    new OpenApiInfo
                    { Title = $"Partners Api", Version = $"{options.Version}" });

                try
                {
                    c.IncludeXmlComments(Path.ChangeExtension(Assembly.GetEntryAssembly()?.Location, "xml"));
                }
                catch
                {

                }
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.CustomSchemaIds(x => x.FullName);
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
                        new string[] { }
                    }
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();
            return services;
        }

        public static IApplicationBuilder UseSwaggerService(this IApplicationBuilder builder, IConfiguration configuration)
        {
            SwaggerOptions options = new SwaggerOptions()
            {
                Title = configuration["Swagger:Title"] ?? "sendsprint.ecommerce.microservices",
                Version = configuration["Swagger:Version"] ?? "v1",
                Build = configuration["BuildNumber"] ?? "1.0.0"
            };

            builder.UseSwagger(c => { c.RouteTemplate = "/_swagger/{documentName}/swagger.json"; });
            builder.UseSwaggerUI(c =>

            {
                c.RoutePrefix = "_swagger";
                c.SwaggerEndpoint($"/_swagger/{options.Version}/swagger.json", $"{options.Title} API {options.Version}");
            });

            return builder;
        }
    }
}
