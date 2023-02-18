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
	[Category(TestCategory.INTEGRATION)]
	public class AllAgentIntegrationTests : IntegrationFixture
	{
		// Note: Relativity Access helpers come from extended IntegrationFixture Injected Objects 
		private AgentJobExceptionWrapper AgentJobExceptionWrapper;
		private ManagerJob ManagerJob;
		private WorkerJob WorkerJob;

		[TearDown]
		public void TearDown()
		{
			var context = Helper.GetDBContext(-1);

			// Delete all records from the queues 
			var sql = String.Format(@"DELETE FROM [EDDSDBO].[TextExtractor_ManagerQueue]
									WHERE [ExtractorSetArtifactID] = {0}
								  DELETE FROM [EDDSDBO].[TextExtractor_WorkerQueue]
									WHERE [ExtractorSetArtifactID] = {0}"
				, TestConstants.EXTRACTOR_SET_ARTIFACT_ID);

			context.ExecuteNonQuerySQLStatement(sql);
		}

		[Test]
		public void WrapperExecute()
		{
			GivenTheAgentJobExceptionWrapper();

			WhenTheAgentJobExceptionWrapperExecutesTheManager();
		}

		[Test(Description = "Executes manager agent with a real connection to Relativity, should complete without exceptions")]
		public void ManagerExecute()
		{
			GivenTheManagerJob();

			var moreThanAManagerBatch = Constant.Sizes.SAVED_SEARCH_BATCH_SIZE + 1;
			WhenSeveralRecordsExistInTheManagerQueue(records: moreThanAManagerBatch);
			WhenTheExtractorSetHasThisStatus(Constant.ExtractorSetStatus.SUBMITTED);
			WhenTheManagerJobExecutes();

			ShouldInsertRecordsIntoTheWorkerQueue();
			ShouldRemoveRecordsInManagerQueue();
			ShouldHaveExtractorSetStatusOf(Constant.ExtractorSetStatus.IN_PROGRESS_MANAGER_COMPLETE);
		}

		[Test(Description = "When the agent has been cancelled by the user, should end execution")]
		public void ManagerExecute_Cancelled()
		{
			GivenTheManagerJob();

			WhenARecordExistsInTheManagerQueue();
			WhenTheExtractorSetHasThisStatus(Constant.ExtractorSetStatus.CANCELLED);
			WhenTheManagerJobExecutes();

			ShouldRemoveRecordsInManagerQueue();
		}

        [Test(Description = "When executed should remove records in worker queue")]
		public void WorkerExecute()
		{
			GivenTheWorkerJob();

			var moreThanAWorkerBatch = Constant.Sizes.WORKER_BATCH_SIZE + 1;
			WhenSeveralRecordsExistInTheWorkerQueue(records: moreThanAWorkerBatch);
			WhenTheWorkerJobExecutes();

			ShouldRemoveRecordsInWorkerQueue();
		}

        [Test(Description = "When the ExtractorSet has been cancelled, should remove records in queue")]
		public void WorkerExecute_Cancelled()
		{
			GivenTheWorkerJob();

			WhenARecordExistsInTheWorkerQueue();
			WhenTheExtractorSetHasThisStatus(Constant.ExtractorSetStatus.CANCELLED);
			WhenTheWorkerJobExecutes();

			ShouldRemoveRecordsInWorkerQueue();
		}

        [Test(Description = "When all agents are executed, should complete with no exceptions")]
		public void AllAgentsExecute()
		{
			GivenTheManagerJob();
			GivenTheWorkerJob();

			WhenARecordExistsInTheManagerQueue();
			WhenTheExtractorSetHasThisStatus(Constant.ExtractorSetStatus.SUBMITTED);
			WhenTheManagerJobExecutes();

			WhenTheWorkerJobExecutes();
		}

		#region Arrange

		private void GivenTheAgentJobExceptionWrapper()
		{
			var wrapper = new AgentJobExceptionWrapper(
				SqlQueryHelper,
				ArtifactQueries,
				Helper.GetServicesManager(),
				Helper.GetDBContext(-1),
				TestConstants.MANAGER_AGENT_ID);

			AgentJobExceptionWrapper = wrapper;
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

		private void GivenTheWorkerJob()
		{
			var factory = new ArtifactFactory(
				ArtifactQueries,
				Helper.GetServicesManager(),
				new ErrorLogModel(SqlQueryHelper, Helper.GetDBContext(-1), TestConstants.WORKER_AGENT_ID, Constant.Tables.WorkerQueue)
				);

			WorkerJob = new WorkerJob(
				TestConstants.WORKER_AGENT_ID,
				Helper.GetServicesManager(),
				ExecutionIdentity.CurrentUser,
				SqlQueryHelper,
				ArtifactQueries,
				factory,
				Constant.AgentType.Worker,
				TestConstants.UNIQUE_BATCH_TABLE_NAME,
				Helper.GetDBContext(-1));

			WorkerJob.TextExtractorLog = new TextExtractorLog();
		}

		#endregion Arrange

		#region Act

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

		private void WhenARecordExistsInTheWorkerQueue()
		{
			var sql = String.Format(@"
			INSERT INTO [EDDSDBO].[TextExtractor_WorkerQueue] 
			([TimeStampUTC],
			 [WorkspaceArtifactID],
			 [QueueStatus],
			 [AgentID],
			 [ExtractorSetArtifactID],
			 [DocumentArtifactID],
			 [ExtractorProfileArtifactID],
			 [SourceLongTextFieldArtifactID]) 
			 VALUES 
			( '{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7} ) ",
			 DateTime.UtcNow,
			 TestConstants.WORKSPACE_ARTIFACT_ID,
			 Constant.QueueStatus.NotStarted,
			 "NULL",
			 TestConstants.EXTRACTOR_SET_ARTIFACT_ID,
			 TestConstants.DOCUMENT_ARTIFACT_ID,
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

		private void WhenSeveralRecordsExistInTheWorkerQueue(int records = 10)
		{
			for (int i = 0; i < records; i++)
			{
				WhenARecordExistsInTheWorkerQueue();
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

		private void WhenTheAgentJobExceptionWrapperExecutesTheManager()
		{
			AgentJobExceptionWrapper.Execute(ManagerJob);
		}

		private void WhenTheManagerJobExecutes()
		{
			ManagerJob.Execute();
		}

		private void WhenTheWorkerJobExecutes()
		{
			WorkerJob.Execute();
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

		private void ShouldRemoveRecordsInWorkerQueue()
		{
			var sql = String.Format(@"
				SELECT TOP 1 [ID] 
				FROM [EDDSDBO].[TextExtractor_WorkerQueue] 
				WHERE [AgentID] = {0}"
				, TestConstants.WORKER_AGENT_ID);

			var context = Helper.GetDBContext(-1);

			var id = context.ExecuteSqlStatementAsScalar<int>(sql);

			Assert.IsTrue(id == 0);
		}

		private void ShouldNotRemoveRecordsInWorkerQueue()
		{
			var sql = String.Format(@"
				SELECT TOP 1 [ExtractorSetArtifactID] 
				FROM [EDDSDBO].[TextExtractor_WorkerQueue] 
				WHERE [AgentID] = {0}"
				, TestConstants.WORKER_AGENT_ID);

			var context = Helper.GetDBContext(-1);

			var extractorSetArtifactId = context.ExecuteSqlStatementAsScalar<int>(sql);

			Assert.AreEqual(TestConstants.EXTRACTOR_SET_ARTIFACT_ID, extractorSetArtifactId);
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