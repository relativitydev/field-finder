namespace TextExtractor.Helpers
{
	public class Enums
	{
		public enum QueueStatus
		{
			Error = -1,
			Nothing = 0,
			New = 1,
			Progressing = 2,
			Complete = 3,
			Cancelled = 4
		}
	}
}