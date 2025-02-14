using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace RouteManager.OpenApiTransformers
{
    public class ServerTransformer : IOpenApiDocumentTransformer
    {
        private IWebHostEnvironment Env { get; }

        public ServerTransformer(IWebHostEnvironment environment)
        {
            Env = environment;
        }

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            if (Env.IsDevelopment())
            {
                document.Servers = [new() { Url = "https://localhost:32769" }];
            }

            return Task.CompletedTask;
        }
    }
}
