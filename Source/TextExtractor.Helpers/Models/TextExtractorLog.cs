using System;

namespace TextExtractor.Helpers.Models
{
	/// <summary>
	///   Represents the Agent Log, allowing it's events to be captured throughout the codebase
	/// </summary>
	public class TextExtractorLog
	{
		public event RaiseUpdateEvent OnUpdate;
		public event RaiseErrorEvent OnError;

		public virtual void RaiseUpdate(String message)
		{
			var handler = OnUpdate;
			if (handler != null)
			{
				handler(this, message);
			}
		}

		public virtual void RaiseError(Exception exception)
		{
			var handler = OnError;
			if (handler != null)
			{
				handler(this, exception);
			}
		}

		public delegate void RaiseUpdateEvent(object sender, String message);
		public delegate void RaiseErrorEvent(object sender, Exception exception);
	}
}