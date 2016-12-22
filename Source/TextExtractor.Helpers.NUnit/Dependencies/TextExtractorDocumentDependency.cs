using TextExtractor.Helpers.Models;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class TextExtractorDocumentDependency : ADependency
	{
		public ExtractorSetDocument Document;

		public override void SharedExecute()
		{
			var random = Pull<RandomGenerator>();

			Document = new ExtractorSetDocument(random.Number(), random.Paragraph());
		}
	}
}