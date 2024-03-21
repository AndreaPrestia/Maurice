using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Maurice.Reader.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddReader(this IServiceCollection services)
    {
        services.AddScoped<ReaderProcessor>();
    }
}