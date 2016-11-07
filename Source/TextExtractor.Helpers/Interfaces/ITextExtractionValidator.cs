using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers.Interfaces
{
	public interface ITextExtractionValidator
	{
		bool Validate(string textSource, string matchingText, ITargetRule targetRule);
	}
}
