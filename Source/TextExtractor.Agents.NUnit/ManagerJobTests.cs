using NUnit.Framework;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Agents.NUnit
{
	[TestFixture]
	public class ManagerJobTests : FakesFixture
	{
		//Note: this test is below one second because all fake data (and there's alot) is
		//generated recursively in the AgentDependencies instantiation
		[Category(TestCategory.UNIT)]
		[Test(Description = "Executes entire agent with fake connection to Relativity, should complete without exceptions")]
		public void Execute()
		{
			var managerJob = GetSystemUnderTest();

			Assert.DoesNotThrow(() => managerJob.Execute());
		}

		[Category(TestCategory.UNIT)]
        [Test(Description = "Executes entire agent with fake connection to Relativity and Extractor Sets are cancelled, should end execution without exceptions")]
		public void Cancelled()
		{
			var managerJob = GetSystemUnderTest();
			Dependencies.Pull<ArtifactQueriesDependency>()
				.WhenTheExtractorSetsAreCancelled();

			Assert.DoesNotThrow(() => managerJob.Execute());
		}

		[Category(TestCategory.UNIT)]
        [Test(Description = "Executes entire agent with fake connection to Relativity and No Manager Queue Records are found, should complete without exceptions")]
		public void NoManagerQueueRecordsFound()
		{
			var managerJob = GetSystemUnderTest();
			Dependencies.Pull<SqlQueryHelperDependency>()
				.WhenThereAreNoRecordsInTheManagerQueue();

			Assert.DoesNotThrow(() => managerJob.Execute());
		}

		private ManagerJob GetSystemUnderTest()
		{
			// Relativity Access Helpers 
			var helper = Dependencies.Pull<FakeHelper>().AgentHelper;
			var query = Dependencies.Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var artifactQuery = Dependencies.Pull<ArtifactQueriesDependency>().Queries;
			var factory = Dependencies.Pull<ArtifactFactoryDependency>().ArtifactFactory;
			var servicesMgr = helper.GetServicesManager();
			var eddsDBContext = helper.GetDBContext(-1);

			var managerJob = new ManagerJob(query, artifactQuery, servicesMgr, eddsDBContext, factory, TestConstants.MANAGER_AGENT_ID);

			managerJob.TextExtractorLog = new TextExtractorLog();

			return managerJob;
		}
	}
}

