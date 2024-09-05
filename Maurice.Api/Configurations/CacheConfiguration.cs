namespace Maurice.Api.Configurations
{
	public class CacheConfiguration
	{
		public const string Cache = nameof(Cache);
		public double SlidingExpiration { get; set; }
		public double AbsoluteExpiration { get; set; }
		public long MaxSizeBytes { get; set; }
	}
}
