using System.Linq;
using NUnit.Framework;
using Relativity.API;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.INTEGRATION)]
	[Category(TestCategory.UNIT)]
	public class WorkerQueueTests : FakesFixture
	{
		[Test(Description = "Retrieve instance of worker queue")]
		public void Constructor()
		{
			WorkerQueue queue = null;

			Assert.DoesNotThrow(() => queue = GetSystemUnderTest());
			Assert.IsNotNull(queue);
			Assert.IsTrue(queue.HasRecords);
			Assert.Greater(queue.Records.Count(), 0);
		}

        [Test(Description = "Retrieve instance of worker queue with no records")]
		public void Constructor_NoRecords()
		{
			Dependencies.Pull<SqlQueryHelperDependency>()
				.WhenThereAreNoRecordsInTheWorkerQueue();

			var queue = GetSystemUnderTest();

			Assert.IsFalse(queue.HasRecords);
			Assert.AreEqual(0, queue.Records.Count());
		}

        [Test(Description = "Worker queue process all records does not throw")]
		public void ProcessAllRecords()
		{
			var queue = GetSystemUnderTest();

			Assert.DoesNotThrow(() => queue.ProcessAllRecords());
		}

		public WorkerQueue GetSystemUnderTest()
		{
			var sqlQuery = Dependencies.Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var artQueries = Dependencies.Pull<ArtifactQueriesDependency>().Queries;
			var factory = Dependencies.Pull<ArtifactFactoryDependency>().ArtifactFactory;
			var helper = Dependencies.Pull<FakeHelper>().Helper;
			var random = Dependencies.Pull<RandomGenerator>();
			var reporting = Dependencies.Pull<ExtractorSetReportingDependency>().ExtractorSetReporting;
			var log = new TextExtractorLog();

			var queue = new WorkerQueue(
				sqlQuery,
				artQueries,
				factory,
				helper.GetDBContext(-1),
				helper.GetServicesManager(),
				ExecutionIdentity.CurrentUser,
				random.Number(),
				random.Number(),
				random.Word(),
				log,
				reporting);

			return queue;
		}
	}
}