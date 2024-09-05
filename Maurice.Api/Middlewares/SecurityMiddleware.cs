using Maurice.Api.Configurations;
using Maurice.Api.Extensions;
using Maurice.Application.Interfaces.Services;
using Maurice.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security;

namespace Maurice.Api.Middlewares
{
	public class SecurityMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<SecurityMiddleware> _logger;
		private readonly IMemoryCache _cache;
		private readonly MemoryCacheEntryOptions _cacheEntryOptions;
		private readonly IDispatcherService? _dispatcherService;

		public SecurityMiddleware(RequestDelegate next, ILogger<SecurityMiddleware> logger, IMemoryCache cache, IOptionsMonitor<CacheConfiguration> configuration, IDispatcherService dispatcherService)
		{
			ArgumentNullException.ThrowIfNull(next);
			ArgumentNullException.ThrowIfNull(logger);
			ArgumentNullException.ThrowIfNull(cache);
			ArgumentNullException.ThrowIfNull(configuration.CurrentValue);
			ArgumentNullException.ThrowIfNull(dispatcherService);
			_next = next;
			_logger = logger;
			_cache = cache;

			_cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(configuration.CurrentValue.SlidingExpiration))
				.SetAbsoluteExpiration(TimeSpan.FromSeconds(configuration.CurrentValue.AbsoluteExpiration))
				.SetSize(configuration.CurrentValue.MaxSizeBytes)
				.SetPriority(CacheItemPriority.High);

			_dispatcherService = dispatcherService;
		}

		public async Task Invoke(HttpContext context, CancellationToken cancellationToken)
		{
			var url = context.Request.Path.Value!;

			var method = context.Request.Method;

			_logger.LogInformation($"Start processing auth request {url} {method}");

			try
			{
				var apiKey = context.GetApiKey();

				if(string.IsNullOrWhiteSpace(apiKey))
				{
					_logger.LogError("x-api-key header not provided.");
					throw new SecurityException();
				}

				_logger.LogInformation($"Processing auth request for client: {apiKey}");

				var dispatcher = await GetDispatcherAsync(apiKey, cancellationToken).ConfigureAwait(false);

				var ipAddress = context.Connection.RemoteIpAddress?.ToString() ??
											throw new InvalidOperationException("No ip address provided");

				var host = context.Request.Host.Value;

				if (!dispatcher!.IpAddresses.Any(ip => string.Equals(ipAddress, ip)))
				{
					_logger.LogError($"{ipAddress} not authorized to perform requests for dispatcher {dispatcher?.Id} with apiKey {apiKey}");
					throw new UnauthorizedAccessException(ipAddress);
				}

				_logger.LogInformation($"Start processing request for dispatcher {dispatcher?.Id} with apiKey {apiKey}");

				context.Items["Dispatcher"] = dispatcher;

				await _next.Invoke(context).ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				throw;
			}
			finally
			{
				_logger.LogInformation($"Finished processing auth request {url} {method}");
			}
		}

		private async Task<DispatcherEntity?> GetDispatcherAsync(string apiKey, CancellationToken cancellationToken)
		{
			var dispatcher = GetFromCache(apiKey);

			if (dispatcher != null) return dispatcher;

			dispatcher = await _dispatcherService!.GetDispatcherAsync(apiKey, cancellationToken).ConfigureAwait(false);

			if (dispatcher == null)
			{
				_logger.LogWarning($"Dispatcher {apiKey} not found. No request will be processed");
				throw new SecurityException();
			}

			SetInCache(dispatcher);

			return dispatcher;
		}

		private void SetInCache(DispatcherEntity? dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			_cache.Remove($"d_{dispatcher.ApiKey}");
			_cache.Set($"d_{dispatcher.ApiKey}", dispatcher, _cacheEntryOptions);
		}

		private DispatcherEntity? GetFromCache(string apiKey)
		{
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				throw new ArgumentException(nameof(apiKey));
			}

			var found = _cache.TryGetValue($"d_{apiKey}", out DispatcherEntity? result);

			return !found ? default : result;
		}
	}
}
