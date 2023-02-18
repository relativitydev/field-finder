using System;
using NUnit.Framework;
using Relativity.API;
using TextExtractor.Helpers;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;

namespace TextExtractor.Agents.NUnit.Integration
{
	[TestFixture]
	public class ManagerAgentIntegrationTests : IntegrationFixture
	{
		// Note: Relativity Access helpers come from extended IntegrationFixture Injected Objects 
		private ManagerJob ManagerJob;
		private AgentJobExceptionWrapper ExceptionWrapper;

		[SetUp]
		public void SetUp()
		{
			ClearManagerQueueOfRecords();
		}

		[TearDown]
		public void TearDown()
		{
			ClearManagerQueueOfRecords();
		}

		[Description("Executes manager agent with a real connection to Relativity, should complete without exceptions")]
		[Category(TestCategory.INTEGRATION)]
		[Test]
		public void ManagerExecute()
		{
			GivenTheManagerJob();

			var moreThanAManagerBatch = Constant.Sizes.MANAGER_QUEUE_BATCH_SIZE + 1;
			WhenSeveralRecordsExistInTheManagerQueue(records: moreThanAManagerBatch);
			WhenTheExtractorSetHasThisStatus(Constant.ExtractorSetStatus.SUBMITTED);
			WhenTheManagerJobExecutes();

			ShouldInsertRecordsIntoTheWorkerQueue();
			ShouldRemoveRecordsInManagerQueue();
		}

		[Description("When the agent has been cancelled by the user, should end execution")]
		[Category(TestCategory.INTEGRATION)]
		[Test]
		public void ManagerExecute_Cancelled()
		{
			GivenTheManagerJob();

			WhenARecordExistsInTheManagerQueue();
			WhenTheExtractorSetHasThisStatus(Constant.ExtractorSetStatus.CANCELLED);
			WhenTheManagerJobExecutes();

			ShouldNotInsertJobsIntoTheWorkerQueue();
		}

        [Description("When Saved Search is missing, should not insert records into worker queue")]
		[Category(TestCategory.INTEGRATION)]
		[Test]
		public void ManagerExecute_MissingSavedSearch()
		{
			GivenTheExceptionWrapper();
			GivenTheManagerJob();

			WhenAFaultySavedSearchArtifactIDExistsInTheManagerQueue();
			WhenTheExtractorSetHasThisStatus(Constant.ExtractorSetStatus.SUBMITTED);
			WhenTheManagerJobExecutesInsideExceptionWrapper();

			ShouldNotInsertJobsIntoTheWorkerQueue();
		}

		#region Arrange

		private void GivenTheExceptionWrapper()
		{
			ExceptionWrapper = new AgentJobExceptionWrapper(
				SqlQueryHelper,
				ArtifactQueries,
				Helper.GetServicesManager(),
				Helper.GetDBContext(-1),
				TestConstants.MANAGER_AGENT_ID);
		}

		private void GivenTheManagerJob()
		{
			var factory = new ArtifactFactory(
				ArtifactQueries,
				Helper.GetServicesManager(),
				new ErrorLogModel(SqlQueryHelper, Helper.GetDBContext(-1), TestConstants.WORKER_AGENT_ID, Constant.Tables.WorkerQueue)
				);

			ManagerJob = new ManagerJob(
				SqlQueryHelper,
				ArtifactQueries,
				Helper.GetServicesManager(),
				Helper.GetDBContext(-1),
				factory,
				TestConstants.MANAGER_AGENT_ID);

			ManagerJob.TextExtractorLog = new TextExtractorLog();
		}

		#endregion Arrange

		#region Act

		private void ClearManagerQueueOfRecords()
		{
			var context = Helper.GetDBContext(-1);

			// Delete all records from the queues 
			var sql = String.Format(@"DELETE FROM [EDDSDBO].[TextExtractor_ManagerQueue]
								  DELETE FROM [EDDSDBO].[TextExtractor_WorkerQueue]");

			context.ExecuteNonQuerySQLStatement(sql);
		}

		private void WhenAFaultySavedSearchArtifactIDExistsInTheManagerQueue()
		{
			var sql = String.Format(@"
			INSERT INTO [EDDSDBO].[TextExtractor_ManagerQueue]
			([TimeStampUTC],
			 [WorkspaceArtifactID],
			 [QueueStatus],
			 [AgentID],
			 [SavedSearchArtifactID],
			 [ExtractorSetArtifactID],
			 [ExtractorProfileArtifactID],
			 [SourceLongTextFieldArtifactID])
			 VALUES
			( '{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7} ) ",
			 DateTime.UtcNow,
			 TestConstants.WORKSPACE_ARTIFACT_ID,
			 Constant.QueueStatus.NotStarted,
			 TestConstants.MANAGER_AGENT_ID,
			 123456789, // Faulty Saved Search Artifact ID
			 TestConstants.EXTRACTOR_SET_ARTIFACT_ID,
			 TestConstants.EXTRACTOR_PROFILE_ARTIFACT_ID,
			 TestConstants.SOURCE_LONG_TEXT_FIELD_ARTIFACT_ID
				);

			var context = Helper.GetDBContext(-1);

			context.ExecuteNonQuerySQLStatement(sql);
		}

		private void WhenARecordExistsInTheManagerQueue()
		{
			var sql = String.Format(@"
			INSERT INTO [EDDSDBO].[TextExtractor_ManagerQueue]
			([TimeStampUTC],
			 [WorkspaceArtifactID],
			 [QueueStatus],
			 [AgentID],
			 [SavedSearchArtifactID],
			 [ExtractorSetArtifactID],
			 [ExtractorProfileArtifactID],
			 [SourceLongTextFieldArtifactID])
			 VALUES
			( '{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7} ) ",
			 DateTime.UtcNow,
			 TestConstants.WORKSPACE_ARTIFACT_ID,
			 Constant.QueueStatus.NotStarted,
			 "NULL",
			 TestConstants.SAVED_SEARCH_ARTIFACT_ID,
			 TestConstants.EXTRACTOR_SET_ARTIFACT_ID,
			 TestConstants.EXTRACTOR_PROFILE_ARTIFACT_ID,
			 TestConstants.SOURCE_LONG_TEXT_FIELD_ARTIFACT_ID
				);

			var context = Helper.GetDBContext(-1);

			context.ExecuteNonQuerySQLStatement(sql);
		}

		private void WhenSeveralRecordsExistInTheManagerQueue(int records = 10)
		{
			for (int i = 0; i < records; i++)
			{
				WhenARecordExistsInTheManagerQueue();
			}
		}

		private void WhenTheExtractorSetHasThisStatus(string status)
		{
			var factory = new ArtifactFactory(
				ArtifactQueries,
				Helper.GetServicesManager(),
				new ErrorLogModel(SqlQueryHelper, Helper.GetDBContext(-1), TestConstants.MANAGER_AGENT_ID, Constant.Tables.ManagerQueue));

			var set = factory.GetInstanceOfExtractorSet(ExecutionIdentity.CurrentUser, TestConstants.WORKSPACE_ARTIFACT_ID, TestConstants.EXTRACTOR_SET_ARTIFACT_ID);

			set.UpdateStatus(status);
		}

		private void WhenTheManagerJobExecutesInsideExceptionWrapper()
		{
			ExceptionWrapper.Execute(ManagerJob);
		}

		private void WhenTheManagerJobExecutes()
		{
			ManagerJob.Execute();
		}

		#endregion Act

		#region Assert

		private void ShouldNotInsertJobsIntoTheWorkerQueue()
		{
			var sql = String.Format(@"
				SELECT TOP 1 [ID]
				FROM [EDDSDBO].[TextExtractor_WorkerQueue]
				WHERE [ExtractorSetArtifactID] = {0}"
				, TestConstants.EXTRACTOR_SET_ARTIFACT_ID);

			var context = Helper.GetDBContext(-1);

			var id = context.ExecuteSqlStatementAsScalar<int>(sql);

			Assert.IsTrue(id == 0);
		}

		private void ShouldInsertRecordsIntoTheWorkerQueue()
		{
			// Retrieves the first record in the queue, confirming it's got the same ExtractorSet
			var sql = @"SELECT TOP 1 [ExtractorSetArtifactID] FROM [EDDSDBO].[TextExtractor_WorkerQueue]";

			var context = Helper.GetDBContext(-1);

			var extractorSetArtifactId = context.ExecuteSqlStatementAsScalar<int>(sql);

			Assert.AreEqual(TestConstants.EXTRACTOR_SET_ARTIFACT_ID, extractorSetArtifactId);
		}

		private void ShouldRemoveRecordsInManagerQueue()
		{
			var sql = String.Format(@"
				SELECT TOP 1 [ID]
				FROM [EDDSDBO].[TextExtractor_ManagerQueue]
				WHERE [AgentID] = {0}"
				, TestConstants.MANAGER_AGENT_ID);

			var context = Helper.GetDBContext(-1);

			var id = context.ExecuteSqlStatementAsScalar<int>(sql);

			Assert.IsTrue(id == 0);
		}

		private void ShouldNotRemoveRecordsInManagerQueue()
		{
			var sql = String.Format(@"
				SELECT TOP 1 [ExtractorSetArtifactID]
				FROM [EDDSDBO].[TextExtractor_ManagerQueue]
				WHERE [AgentID] = {0}"
				, TestConstants.MANAGER_AGENT_ID);

			var context = Helper.GetDBContext(-1);

			var extractorSetArtifactId = context.ExecuteSqlStatementAsScalar<int>(sql);

			Assert.AreEqual(TestConstants.EXTRACTOR_SET_ARTIFACT_ID, extractorSetArtifactId);
		}

		private void ShouldSetRecordStatusTo(int queueStatus)
		{
			var sql = String.Format(@"
				SELECT TOP 1 [QueueStatus]
				FROM [EDDSDBO].[TextExtractor_ManagerQueue]
				WHERE [AgentID] = {0}"
				, TestConstants.MANAGER_AGENT_ID);

			var context = Helper.GetDBContext(-1);

			var status = context.ExecuteSqlStatementAsScalar<int>(sql);

			Assert.AreEqual(queueStatus, status);
		}

		private void ShouldHaveExtractorSetStatusOf(string status)
		{
			var factory = new ArtifactFactory(
				ArtifactQueries,
				Helper.GetServicesManager(),
				new ErrorLogModel(SqlQueryHelper, Helper.GetDBContext(-1), TestConstants.MANAGER_AGENT_ID, Constant.Tables.ManagerQueue));

			var set = factory.GetInstanceOfExtractorSet(ExecutionIdentity.CurrentUser, TestConstants.WORKSPACE_ARTIFACT_ID, TestConstants.EXTRACTOR_SET_ARTIFACT_ID);

			Assert.AreEqual(status, set.Status);
		}

		#endregion Assert
	}
}