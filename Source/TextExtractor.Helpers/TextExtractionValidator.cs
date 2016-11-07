using System;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers
{
	public class TextExtractionValidator : ITextExtractionValidator
	{
		public bool Validate(string textSource, string marker, ITargetRule targetRule)
		{
			if (textSource == null)
			{
				throw new ArgumentNullException("textSource");
			}

			if (marker == null)
			{
				throw new ArgumentNullException("matchingText");
			}

            if (IsNegative(targetRule.CharacterLength))
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.CHARACTER_LENGTH_IS_NEGATIVE);
			}

            if (IsNegative(targetRule.Occurrence))
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.OCCURRENCE_LENGTH_IS_NEGATIVE);
			}

			if (targetRule.MarkerEnum == Constant.MarkerEnum.PlainText && marker.Length > textSource.Length)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.MATCHING_TEXT_LENGTH_GREATER_THAN_TEXT_SOURCE_LENGTH);
			}

			return true;
		}

		private Boolean IsNegative(int integer)
		{
			return integer < 0;
		}
	}
}