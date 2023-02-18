using NUnit.Framework;
using Relativity.API;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Agents.NUnit
{
	[TestFixture]
	[Category(TestCategory.INTEGRATION)]
	[Category(TestCategory.UNIT)]
	public class WorkerJobTests : FakesFixture
	{
		[Test(Description = "Executes entire agent with fake connection to Relativity, should complete without exceptions")]
		public void Execute()
		{
			var workerJob = GetSystemUnderTest();

			Assert.DoesNotThrow(() => workerJob.Execute());
		}

        [Test(Description = "Executes entire agent with fake connection to Relativity and No Records in Queue are found, should complete without exceptions")]
		public void Execute_NoRecords()
		{
			Dependencies.Pull<SqlQueryHelperDependency>()
				.WhenThereAreNoRecordsInTheWorkerQueue();

			var workerJob = GetSystemUnderTest();

			Assert.DoesNotThrow(() => workerJob.Execute());
		}

		private WorkerJob GetSystemUnderTest()
		{
			// Relativity Access Helpers 
			var helper = Dependencies.Pull<FakeHelper>().AgentHelper;
			var queryHelper = Dependencies.Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var artQueries = Dependencies.Pull<ArtifactQueriesDependency>().Queries;
			var factory = Dependencies.Pull<ArtifactFactoryDependency>().ArtifactFactory;
			var eddsDbContext = helper.GetDBContext(-1);

			var workerJob = new WorkerJob(Random.Number(), helper.GetServicesManager(), ExecutionIdentity.CurrentUser, queryHelper, artQueries, factory, Constant.AgentType.Worker, "Test Batch Table Name", eddsDbContext);
			workerJob.TextExtractorLog = new TextExtractorLog();

			return workerJob;
		}
	}
}

