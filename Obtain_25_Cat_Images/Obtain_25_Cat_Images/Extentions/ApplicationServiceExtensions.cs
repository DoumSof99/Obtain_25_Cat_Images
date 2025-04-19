using Obtain_25_Cat_Images.Data;
using Microsoft.EntityFrameworkCore;
using Obtain_25_Cat_Images.Interfaces;
using Obtain_25_Cat_Images.Services;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Obtain_25_Cat_Images.Extentions {

    public static class ApplicationServiceExtensions {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Steal 25 Cats API",
                    Version = "v1",
                    Description = "REST API for fetching and managing cats from TheCatAPI.",
                    Contact = new OpenApiContact {
                        Name = "Sofia Doumani",
                        Url = new Uri("https://github.com/DoumSof99")
                    }
                });

            });

            services.AddDbContext<AppDbContext>(o => 
                o.UseSqlServer(config.GetConnectionString("DefaultConnections")));
            services.AddHttpClient<ICatService, CatService>("TheCatApi", client => {
                client.BaseAddress = new Uri("https://api.thecatapi.com/v1/");
                client.DefaultRequestHeaders.Add("x-api-key", config["TheCatApi:ApiKey"]);
            }); 
           
            // services.AddScoped<ICatService, CatService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
