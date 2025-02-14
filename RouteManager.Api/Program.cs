
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using RouteManager.Database;
using RouteManager.OpenApiTransformers;

namespace RouteManager.Api
{
    public class Program
    {
        public static void Main(string[] args) => MainAync(args).Wait();

        private static async Task MainAync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<ServerTransformer>();
            });

            builder.Services.AddDbContext<RoutingContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("RouteManagerDb"));
            });

            builder.Services.AddScoped<Managers.RouteManager>();

            var app = builder.Build();

            await using (var scope = app.Services.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RoutingContext>();
                await context.Database.MigrateAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Route Manager API");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            await app.RunAsync();
        }
    }
}
