using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class DocumentBatchDependency : ADependency
	{
		public DocumentBatch Batch;

		public override void SharedExecute()
		{
			var servicesMgr = Pull<FakeServicesMgr>().ServicesMgr;
			var artQueries = Pull<ArtifactQueriesDependency>().Queries;

			Batch = new DocumentBatch(artQueries, servicesMgr);
		}
	}
}
