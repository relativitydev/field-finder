using System;
using Relativity.API;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.Rsapi;
using System.Globalization;

namespace TextExtractor.Agents
{
	public class WorkerJob : AgentJobBase
	{
		public int AgentId;
		private readonly IServicesMgr ServicesMgr;
		private readonly ExecutionIdentity ExecutionIdentity;
		public Constant.AgentType AgentType;
		public string QueueTable;
		public string BatchTableName;
		public int WorkspaceArtifactId;
		public int ResourceServerId { get; set; }
		public int RecordId;
		public IDBContext EddsDbContext;
		public ISqlQueryHelper SqlQueryHelper;
		public IArtifactQueries ArtifactQueries;
		public ArtifactFactory ArtifactFactory;
		public ExtractorSetReporting TextExtractorJobReporting;

		public WorkerJob(int agentId, IServicesMgr servicesMgr, ExecutionIdentity executionIdentity, ISqlQueryHelper sqlQueryHelper, IArtifactQueries artifactQueries, ArtifactFactory artifactFactory, Constant.AgentType agentType, string uniqueBatchTableName, IDBContext eddsDbContext)
		{
			RecordId = 0;
			WorkspaceArtifactId = -1;
			AgentId = agentId;
			ServicesMgr = servicesMgr;
			ExecutionIdentity = executionIdentity;
			AgentType = agentType;
			EddsDbContext = eddsDbContext;
			QueueTable = Constant.Tables.WorkerQueue;
			BatchTableName = uniqueBatchTableName;
			SqlQueryHelper = sqlQueryHelper;
			ArtifactQueries = artifactQueries;
			ArtifactFactory = artifactFactory;
			TextExtractorJobReporting = new ExtractorSetReporting(artifactQueries, servicesMgr);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
		}

		public override void Execute()
		{
			//retrieve ResourceServerId for agent for checking resource pool. this value will be used when retrieving records from queue
			ResourceServerId = SqlQueryHelper.GetResourceServerByAgentId(EddsDbContext, AgentId);
			if (ResourceServerId == 0)
			{
				TextExtractorLog.RaiseUpdate(string.Format("Resource Server for Agent ID {0} cannot be detected.", AgentId));
				return;
			}

			TextExtractorLog.RaiseUpdate("Processing Worker Queue Batch.");
			var workerQueue = new WorkerQueue(SqlQueryHelper, ArtifactQueries, ArtifactFactory, EddsDbContext, ServicesMgr, ExecutionIdentity, AgentId, ResourceServerId, BatchTableName, TextExtractorLog, TextExtractorJobReporting);

			if (workerQueue.HasRecords)
			{
				WorkspaceArtifactId = workerQueue.WorkspaceArtifactId;
				var extractorSet = ArtifactFactory.GetInstanceOfExtractorSet(ExecutionIdentity.CurrentUser, workerQueue.WorkspaceArtifactId, workerQueue.ExtractorSetArtifactId);

				//check for ExtractorSet cancellation
				Boolean isCancelled = CheckForExtractorSetCancellation(extractorSet, true);
				if (!isCancelled)
				{
					//process worker queue records in current batch
					workerQueue.ProcessAllRecords();

					//check for ExtractorSet cancellation
					CheckForExtractorSetCancellation(extractorSet, false);
				}
			}

			TextExtractorLog.RaiseUpdate("Worker Queue Batch processed.");
		}

		private Boolean CheckForExtractorSetCancellation(ExtractorSet extractorSet, Boolean deleteCurrentWorkerQueueBatch)
		{
			Boolean retVal = false;

			if (extractorSet.IsCancellationRequested())
			{
				TextExtractorLog.RaiseUpdate("Cancellation Requested.");

				//Delete records in worker queue for current ExtractorSet which is cancelled and are assigned to current agent
				if (deleteCurrentWorkerQueueBatch)
				{
					SqlQueryHelper.DeleteRecordsInWorkerQueueForCancelledExtractorSetAndAgentId(EddsDbContext, WorkspaceArtifactId, extractorSet.ArtifactId, AgentId);
				}

				//Delete records in worker queue for current ExtractorSet which is cancelled and are not assigned to other agents
				SqlQueryHelper.DeleteRecordsInWorkerQueueForCancelledExtractorSet(EddsDbContext, WorkspaceArtifactId, extractorSet.ArtifactId);
				retVal = true;
			}
			return retVal;
		}
	}
}