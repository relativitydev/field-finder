using Relativity.API;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class ExtractorSetDependency : ADependency
	{
		public ExtractorSet ExtractorSet;

		public ExtractorSetDependency()
		{
			this.Add(new ExtractorSetHistoryDependency());
			this.Add(new ExtractorSetReportingDependency());
		}

		public override void SharedExecute()
		{
			var random = Pull<RandomGenerator>();
			var jobReportingDependency = Pull<ExtractorSetReportingDependency>().ExtractorSetReporting;
			var artifactQueries = Pull<ArtifactQueriesDependency>().Queries;
			var servicesMgr = Pull<FakeServicesMgr>().ServicesMgr;
			var extractorSetRdo = Pull<ArtifactQueriesReturns>().ExtractorSetRdo;

			ExtractorSet = new ExtractorSet(artifactQueries, servicesMgr, ExecutionIdentity.CurrentUser, random.Number(), jobReportingDependency, extractorSetRdo);
		}
	}
}