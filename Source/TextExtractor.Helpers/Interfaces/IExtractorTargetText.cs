using System;

using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers.Interfaces
{
	public interface IExtractorTargetText
	{
		Boolean Process(ExtractorSetDocument extractorSetDocument, ExtractorSet extractorSet);
	}
}