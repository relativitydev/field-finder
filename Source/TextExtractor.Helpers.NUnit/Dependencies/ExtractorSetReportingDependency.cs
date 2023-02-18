using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class ExtractorSetReportingDependency : ADependency
	{
		public ExtractorSetReporting ExtractorSetReporting;

		public override void SharedExecute()
		{
			var query = Pull<ArtifactQueriesDependency>().Queries;
			var mgr = Pull<FakeServicesMgr>().ServicesMgr;

			this.ExtractorSetReporting = new ExtractorSetReporting(query, mgr);
		}
	}
}