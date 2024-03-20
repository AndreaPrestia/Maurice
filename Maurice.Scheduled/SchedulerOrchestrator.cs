using Maurice.Scheduler.Abstractions.Repository;
using Microsoft.Extensions.Logging;

namespace Maurice.Scheduler
{
    public sealed class SchedulerOrchestrator
    {
        private readonly IEventEntityRepository _eventEntityRepository;
        private readonly IEventTypeEntityRepository _eventTypeEntityRepository;
        private readonly ILogger<SchedulerOrchestrator> _logger;

        public SchedulerOrchestrator(IEventEntityRepository eventEntityRepository, IEventTypeEntityRepository eventTypeEntityRepository, ILogger<SchedulerOrchestrator> logger)
        {
            ArgumentNullException.ThrowIfNull(eventEntityRepository);
            ArgumentNullException.ThrowIfNull(eventTypeEntityRepository);
            ArgumentNullException.ThrowIfNull(logger);

            _eventEntityRepository = eventEntityRepository;
            _eventTypeEntityRepository = eventTypeEntityRepository;
            _logger = logger;
        }

        public async Task ScheduleAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting scheduling");

                var entityTypes = await _eventTypeEntityRepository.ReadAsync(cancellationToken);

                if (entityTypes == null || entityTypes.Count == 0)
                {
                    _logger.LogWarning("No entity types configured. No processing will be done.");
                    return;
                }

                foreach (var entityType in entityTypes)
                {
                    foreach (var scheduleRule in entityType.ScheduleRules)
                    {
                        //TODO for every schedule rule initialize the implementations for scheduling
                    }
                }

                _logger.LogInformation("Ended scheduling");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
