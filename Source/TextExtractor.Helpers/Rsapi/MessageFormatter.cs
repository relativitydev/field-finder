﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TextExtractor.Helpers.Rsapi
{
	public class MessageFormatter
	{
		public static String FormatMessage(List<String> results, String message, Boolean success)
		{
			String messageList = "";

			if (!success)
			{
				messageList = message;
				results.ToList().ForEach(w => messageList += (w));
			}

			return messageList;
		}
	}
}
