using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers.Interfaces
{
	public interface ITextExtractionUtility
	{
		bool IsTextTruncated { get; set; }
		bool IsMarkerFound { get; set; }
        string StopMarker { get; set; }
		string ExtractText(string textSource, string matchingText, string stopMarker, ITargetRule targetRule);
	}
}
