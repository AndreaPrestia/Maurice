using Maurice.Writer.Abstractions.Repositories;
using Microsoft.Extensions.Logging;

namespace Maurice.Writer;

public sealed class WriterProcessor
{
    private readonly IEventEntityRepository _eventEntityRepository;
    private readonly IEventTypeEntityRepository _eventTypeEntityRepository;
    private readonly ILogger<WriterProcessor> _logger;

    public WriterProcessor(IEventEntityRepository eventEntityRepository, IEventTypeEntityRepository eventTypeEntityRepository, ILogger<WriterProcessor> logger)
    {
        ArgumentNullException.ThrowIfNull(eventEntityRepository);
        ArgumentNullException.ThrowIfNull(eventTypeEntityRepository);
        ArgumentNullException.ThrowIfNull(logger);

        _eventEntityRepository = eventEntityRepository;
        _eventTypeEntityRepository = eventTypeEntityRepository;
        _logger = logger;
    }

    public async Task<WriterProcessorResult> WriteAsync<T>(T value, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var typeName = typeof(T).Name;

            _logger.LogInformation($"Start processing eventType {typeName}");

            var eventTypeEntity = await _eventTypeEntityRepository.GetAsync<T>(cancellationToken);

            if (eventTypeEntity == null)
            {
                throw new InvalidOperationException(
                    $"EventType {typeName} not configured. Cannot proceed with processing.");
            }

            await _eventEntityRepository.InsertAsync(value, eventTypeEntity, cancellationToken);

            return WriterProcessorResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return WriterProcessorResult.Ko(ex.Message);
        }
    }
}

public class WriterProcessorResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }

    private WriterProcessorResult()
    {
        Success = true;
    }

    private WriterProcessorResult(string? errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
    }

    public static WriterProcessorResult Ok() => new();

    public static WriterProcessorResult Ko(string? errorMessage) => new(errorMessage);
}