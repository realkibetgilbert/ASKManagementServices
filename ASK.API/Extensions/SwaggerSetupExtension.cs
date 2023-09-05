using Microsoft.OpenApi.Models;

namespace ASK.API.Extensions
{
    public static class SwaggerSetupExtension
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASK SERVICE API",
                    Description = "This API provides all endpoints for ASK services",

                });

            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ASK Api v1");

                // Hides the swagger models
                options.DefaultModelsExpandDepth(-1);
            });


            return app;
        }
    }
}
