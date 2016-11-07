using System;

namespace TextExtractor.Helpers
{
	public class ExceptionMessageFormatter
	{
		public static string GetInnerMostExceptionMessage(Exception exception)
		{
			String retVal;

			if (exception == null)
			{
				retVal = String.Empty;
			}
			else
			{
				Exception currentException = exception;
				while (currentException.InnerException != null)
				{
					currentException = currentException.InnerException;
				}

				retVal = currentException.Message;
			}
			return retVal;
		}
	}
}
