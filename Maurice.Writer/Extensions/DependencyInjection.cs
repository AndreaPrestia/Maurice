using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Maurice.Writer.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddWriter(this IServiceCollection services)
    {
        services.AddScoped<WriterProcessor>();
    }
}