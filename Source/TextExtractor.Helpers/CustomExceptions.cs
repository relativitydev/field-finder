namespace TextExtractor.Helpers
{
	public class CustomExceptions
	{
		[System.Serializable]
		public class TextExtractorException : System.Exception
		{
			public TextExtractorException()
				: base()
			{
			}

			public TextExtractorException(string message)
				: base(message)
			{
			}

			public TextExtractorException(string message, System.Exception inner)
				: base(message, inner)
			{
			}

			// A constructor is needed for serialization when an
			// exception propagates from a remoting server to the client. 
			protected TextExtractorException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
			}
		}

		public class TextExtractorSetConsoleCancelException : System.Exception
		{
			public TextExtractorSetConsoleCancelException()
				: base()
			{
			}

			public TextExtractorSetConsoleCancelException(string message)
				: base(message)
			{
			}

			public TextExtractorSetConsoleCancelException(string message, System.Exception inner)
				: base(message, inner)
			{
			}

			// A constructor is needed for serialization when an
			// exception propagates from a remoting server to the client. 
			protected TextExtractorSetConsoleCancelException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
			}
		}

		public class TextExtractorSetMissingFieldsException : System.Exception
		{
			public TextExtractorSetMissingFieldsException()
				: base()
			{
			}

			public TextExtractorSetMissingFieldsException(string message)
				: base(message)
			{
			}

			public TextExtractorSetMissingFieldsException(string message, System.Exception inner)
				: base(message, inner)
			{
			}

			// A constructor is needed for serialization when an
			// exception propagates from a remoting server to the client. 
			protected TextExtractorSetMissingFieldsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
			}
		}

		public class SmtpServerErrorException : System.Exception
		{
			public SmtpServerErrorException() : base() { }

			public SmtpServerErrorException(string message) : base(message) { }

			public SmtpServerErrorException(string message, System.Exception innerException) : base(message, innerException) { }
		}

		public class IncorrectSmtpSettingsException : System.Exception
		{
			public IncorrectSmtpSettingsException() : base() { }

			public IncorrectSmtpSettingsException(string message) : base(message) { }

			public IncorrectSmtpSettingsException(string message, System.Exception innerException) : base(message, innerException) { }
		}
	}
}