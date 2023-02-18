using Relativity.API;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class ExtractorSetHistoryDependency : ADependency
	{
		public ExtractorSetHistory ExtractorSetHistory;

		public override void SharedExecute()
		{
			var random = Pull<RandomGenerator>();
			var queries = Pull<ArtifactQueriesDependency>().Queries;
			var mgr = Pull<FakeServicesMgr>().ServicesMgr;

			ExtractorSetHistory = new ExtractorSetHistory(
				mgr,
				ExecutionIdentity.CurrentUser,
				queries,
				random.Number(),
				random.Number(),
				random.Number(),
				random.Number(),
				"Test target name",
				"Test Start marker",
                "Test Stop marker",
                "PlainText"
                );
		}
	}
}