namespace Maurice.Application.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TopicAttribute : Attribute
	{
		public string Value { get; }

        public TopicAttribute(string value)
        {
            Value = value;
        }
    }
}
