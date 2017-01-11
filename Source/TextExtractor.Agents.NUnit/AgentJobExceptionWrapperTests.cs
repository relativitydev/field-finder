using NUnit.Framework;
using TextExtractor.Helpers.NUnit;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Agents.NUnit
{
	[TestFixture]
	public class AgentJobExceptionWrapperTests : FakesFixture
	{
		[Description("When the manager job executes within the exception wrapper, should not throw")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Execute()
		{
			var managerJob = Dependencies.Pull<ManagerJobDependency>().ManagerJob;
			var wrapper = GetSystemUnderTest();

			Assert.DoesNotThrow(() => wrapper.Execute(managerJob));
		}

		[Description("When the DBContext throws, should not escape the exception wrapper")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Execute_DBContextThrows()
		{
			var managerJob = Dependencies.Pull<ManagerJobDependency>().ManagerJob;
			var wrapper = GetSystemUnderTest();

			Dependencies.Pull<SqlQueryHelperDependency>()
				.WhenTheDbContextThrowsRetrievingAManagerQueueBatch();

			Assert.DoesNotThrow(() => wrapper.Execute(managerJob));
		}

		[Description("When the Rsapi throws, should not escape the exception wrapper")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Execute_RsapiThrows()
		{
			var managerJob = Dependencies.Pull<ManagerJobDependency>().ManagerJob;
			var wrapper = GetSystemUnderTest();

			Dependencies.Pull<ArtifactQueriesDependency>()
				.WhenTheRsapiThrowsWhileRetrievingAnExtractorSet();

			Assert.DoesNotThrow(() => wrapper.Execute(managerJob));
		}

		public AgentJobExceptionWrapper GetSystemUnderTest()
		{
			var sqlHelper = Dependencies.Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var artQueries = Dependencies.Pull<ArtifactQueriesDependency>().Queries;
			var helper = Dependencies.Pull<FakeHelper>().Helper;

			var wrapper = new AgentJobExceptionWrapper(
				sqlHelper,
				artQueries,
				helper.GetServicesManager(),
				helper.GetDBContext(-1),
				TestConstants.MANAGER_AGENT_ID);

			return wrapper;
		}
	}
}

