using Maurice.Api.Extensions;
using Maurice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Maurice.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.MapPost("/api/{topic}/events", async (CancellationToken cancellationToken, ILoggerFactory loggerFactory, HttpContext httpContext, [FromServices] IEnqueueService enqueueService, [FromRoute] string topic) =>
                {
                    var logger = loggerFactory.CreateLogger("Start");

                    var dispatcher = httpContext.GetDispatcherFromHttpContext();

                    logger.LogInformation($"Starting processing request for {dispatcher.IpAddress} - {dispatcher.Hostname} for topic {topic}");

                    try
                    {
                        var queued = await enqueueService.EnqueueAsync(dispatcher, await new StreamReader(httpContext.Request.Body).ReadToEndAsync(cancellationToken), cancellationToken)
                            .ConfigureAwait(false);

                        logger.LogInformation($"Request for {dispatcher.IpAddress} - {dispatcher.Hostname} for topic {topic} queued: {queued}");

                        return Results.NoContent();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);

                        return Results.BadRequest(ex.Message);
                    }
                })
                .WithName("EnqueueEvent")
                .WithOpenApi();

            app.Run();
        }
    }
}
