using Maurice.Reader.Abstractions.Repository;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Maurice.Domain.Entities;

namespace Maurice.Reader;

public sealed class ReaderProcessor
{
    private readonly IEventEntityRepository _eventEntityRepository;
    private readonly IEventTypeEntityRepository _eventTypeEntityRepository;
    private readonly ILogger<ReaderProcessor> _logger;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    public ReaderProcessor(IEventEntityRepository eventEntityRepository, IEventTypeEntityRepository eventTypeEntityRepository, ILogger<ReaderProcessor> logger)
    {
        ArgumentNullException.ThrowIfNull(eventEntityRepository);
        ArgumentNullException.ThrowIfNull(eventTypeEntityRepository);
        ArgumentNullException.ThrowIfNull(logger);

        _eventEntityRepository = eventEntityRepository;
        _eventTypeEntityRepository = eventTypeEntityRepository;
        _logger = logger;
    }

    public async Task<ReaderProcessorResult<T>> ReadAsync<T>(long start, long end, bool descending, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var typeName = typeof(T).Name;

            _logger.LogInformation($"Start processing eventType {typeName}");

            var eventTypeEntity = await _eventTypeEntityRepository.GetAsync<T>(cancellationToken);

            if (eventTypeEntity == null)
            {
                throw new InvalidOperationException(
                    $"EventType '{typeName}' not configured. Cannot proceed with processing.");
            }

            var eventEntities = await _eventEntityRepository.ReadAsync(start, end, eventTypeEntity, descending, cancellationToken);

            if (eventEntities == null)
            {
                return ReaderProcessorResult<T>.Ok(new List<T>());
            }

            var items = eventEntities.Where(e => !string.IsNullOrWhiteSpace(e.Body)).Select(x => JsonSerializer.Deserialize<T>(x.Body, _serializerOptions)).ToList();

            return ReaderProcessorResult<T>.Ok(items!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return ReaderProcessorResult<T>.Ko(ex.Message);
        }
    }

    public async Task<ReaderProcessorResult<EventEntity>> ReadAsync(long start, long end, IList<string> tags, bool descending, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Start processing events for tags {string.Join(',', tags)}");

            var eventTypeEntities = await _eventTypeEntityRepository.ReadAsync(tags, cancellationToken);

            if (eventTypeEntities == null || eventTypeEntities.Count == 0)
            {
                throw new InvalidOperationException(
                    $"EventTypes for tags {string.Join(',', tags)} not configured. Cannot proceed with processing.");
            }

            var eventEntities = await _eventEntityRepository.ReadAsync(start, end, eventTypeEntities, descending, cancellationToken);

            if (eventEntities == null)
            {
                return ReaderProcessorResult<EventEntity>.Ok(new List<EventEntity>());
            }

            return ReaderProcessorResult<EventEntity>.Ok(eventEntities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return ReaderProcessorResult<EventEntity>.Ko(ex.Message);
        }
    }
}

public class ReaderProcessorResult<T>
{
    public bool Success { get; }
    public string? ErrorMessage { get; }

    public IList<T> Items { get; }

    private ReaderProcessorResult(IList<T> items)
    {
        Success = true;
        Items = items;
    }

    private ReaderProcessorResult(string? errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
        Items = new List<T>();
    }

    public static ReaderProcessorResult<T> Ok(IList<T> items) => new(items);

    public static ReaderProcessorResult<T> Ko(string? errorMessage) => new(errorMessage);
}