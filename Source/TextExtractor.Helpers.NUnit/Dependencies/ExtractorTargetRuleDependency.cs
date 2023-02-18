using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.TestHelpers;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class ExtractorTargetRuleDependency : ADependency
	{
		public TargetRule TargetRule;

		public override void SharedExecute()
		{
			var extractorTargetTextRdo = Pull<ArtifactQueriesReturns>().ExtractorTargetTextRdo;

			TargetRule = new TargetRule(extractorTargetTextRdo);
		}
	}
}