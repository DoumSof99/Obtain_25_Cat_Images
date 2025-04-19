using Obtain_25_Cat_Images.Data;
using Microsoft.EntityFrameworkCore;
using Obtain_25_Cat_Images.Interfaces;
using Obtain_25_Cat_Images.Services;

namespace Obtain_25_Cat_Images.Extentions {

    public static class ApplicationServiceExtensions {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
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
