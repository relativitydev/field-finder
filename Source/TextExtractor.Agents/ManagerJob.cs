using System;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.Rsapi;
using System.Globalization;

namespace TextExtractor.Agents
{
	public class ManagerJob : AgentJobBase
	{
		private readonly ISqlQueryHelper SqlQueryHelper;
		private readonly IArtifactQueries ArtifactQueries;
		public readonly IServicesMgr ServicesMgr;
		private readonly IDBContext EddsDbContext;
		private readonly ArtifactFactory ArtifactFactory;
		private readonly int AgentId;

		public ManagerJob(ISqlQueryHelper sqlQueryHelper, IArtifactQueries artifactQueries, IServicesMgr servicesMgr, IDBContext eddsDbContext, ArtifactFactory artifactFactory, int agentId)
		{
			SqlQueryHelper = sqlQueryHelper;
			ArtifactQueries = artifactQueries;
			ServicesMgr = servicesMgr;
			EddsDbContext = eddsDbContext;
			ArtifactFactory = artifactFactory;

			AgentId = agentId;
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
		}

		public override void Execute()
		{
			var managerQueue = GetManagerQueue();

			// Reset unfinished jobs 
			TextExtractorLog.RaiseUpdate(string.Format("Resetting records which failed. [Table = {0}]", Constant.Tables.ManagerQueue));
			managerQueue.ResetUnfinishedRecords();

			// Retrieve the records in the manager queue 
			TextExtractorLog.RaiseUpdate("Retrieving next batch of records in the manager queue.");
			managerQueue.GetNextBatchOfRecords();

			if (managerQueue.HasRecords == false)
			{
				TextExtractorLog.RaiseUpdate("No jobs found in the manager queue.");
				return;
			}

			foreach (ManagerQueueRecord record in managerQueue.Records)
			{
				try
				{
					var extractorSet = GetExtractorSet(record.ExtractorSetArtifactId, record.WorkspaceArtifactId);

					// Checks to see if the user's cancelled the job between the time it was entered
					// and agent picked it up
					if (extractorSet.IsCancellationRequested())
					{
						TextExtractorLog.RaiseUpdate("Job was cancelled by user.");
						record.Remove();
						continue;
					}

					// Process the record 
					TextExtractorLog.RaiseUpdate(String.Format("Processing record. [Table = {0}, ExtractorSetArtifactID = {1}, Workspace Artifact ID = {2}]", Constant.Tables.ManagerQueue, record.ExtractorSetArtifactId, record.WorkspaceArtifactId));

					if (extractorSet.Exists == false)
					{
						TextExtractorLog.RaiseUpdate("No Text Extraction Job found.");
						continue;
					}

					// Sets the job status to processing 
					extractorSet.UpdateStatus(Constant.ExtractorSetStatus.IN_PROGRESS_MANAGER_PROCESSING);

					var documentBatch = GetDocumentBatch();

					// Gets document artifact IDs from the saved search and bulk copies them into
					// the worker queue
					do
					{
						if (HasReachedInterval(documentBatch.Index))
						{
							// Check for cancellation 
							if (extractorSet.IsCancellationRequested())
							{
								// Stop inserting any more records into the Worker queue 
								record.Remove();
								break;
							}
						}

						var nextBatch = documentBatch.GetNext(record.WorkspaceArtifactId, record.SavedSearchArtifactId);

						// End execution if there's a failure, so the job will be re-tried when it's
						// picked up next
						if (nextBatch == null || nextBatch.TotalCount == 0)
						{
							//Saved Search is emnpty
							TextExtractorLog.RaiseUpdate((String.Format("Failed to retrieve a batch of documents for saved search {0}", record.SavedSearchArtifactId)));
							extractorSet.UpdateStatus(Constant.ExtractorSetStatus.COMPLETE_WITH_ERRORS);
							extractorSet.UpdateDetails(Constant.ErrorMessages.SAVED_SEARCH_IS_EMPTY);
							record.Remove();
							break;
						}

						var batchTable = documentBatch.ConvertToWorkerQueueTable(nextBatch, record);

						// End execution if there's a failure, so the job will be re-tried when it's
						// picked up next
						if (batchTable == null)
						{
							TextExtractorLog.RaiseUpdate("Failed to convert values into worker queue table");
							extractorSet.UpdateStatus(Constant.ExtractorSetStatus.ERROR);
							break;
						}

						var recordsAddedToWorkerQueue = managerQueue.AddRecordsToWorkerQueue(batchTable);

						if (!recordsAddedToWorkerQueue)
						{
							TextExtractorLog.RaiseUpdate("Failed to add records to the worker queue");
						}
					}
					while (documentBatch.AreMoreBatches);

					// If the extractorSet was cancelled while processing, will break to this and
					// continue to the next record
					if (extractorSet.IsCancellationRequested())
					{
						TextExtractorLog.RaiseUpdate("Job was cancelled by user.");
						record.Remove();
						continue;
					}

					if (documentBatch.Total == 0)
					{
						continue; //Saved Search is emnpty
					}

					TextExtractorLog.RaiseUpdate(String.Format("Processed record. [Table = {0}, ExtractorSetArtifactID = {1}, Workspace Artifact ID = {2}]", Constant.Tables.ManagerQueue, record.ExtractorSetArtifactId, record.WorkspaceArtifactId));

					// Update Total Expected Update Count on Reporting RDO 
					var extractorProfileRdo = ArtifactQueries.GetExtractorProfileRdo(ServicesMgr, ExecutionIdentity.CurrentUser, record.WorkspaceArtifactId, record.ExtractorProfileArtifactId);
				var targetTexts = extractorProfileRdo.Fields.Get(Constant.Guids.Fields.ExtractorProfile.TargetText).GetValueAsMultipleObject<Artifact>();
				var extractorProfileTargetCount = (targetTexts != null && targetTexts.Count > 0) ? targetTexts.Count : 0;

					extractorSet.ExtractorSetReporting.UpdateTotalExpectedUpdates(record.WorkspaceArtifactId, record.ExtractorSetArtifactId, documentBatch.Total, extractorProfileTargetCount);

					//Remove the record from the manager queue
					TextExtractorLog.RaiseUpdate(String.Format("Removing record from the queue. [Table = {0}, ExtractorSetArtifactID = {1}, Workspace Artifact ID = {2}]", Constant.Tables.ManagerQueue, record.ExtractorSetArtifactId, record.WorkspaceArtifactId));

					// Remove all records associated to this agent 
					managerQueue.RemoveRecordsByAgent();

					TextExtractorLog.RaiseUpdate(String.Format("Removed record from the queue. [Table = {0}, ExtractorSetArtifactID = {1}, Workspace Artifact ID = {2}]", Constant.Tables.ManagerQueue, record.ExtractorSetArtifactId, record.WorkspaceArtifactId));

					// Update the status of job in Relativity 
					extractorSet.UpdateStatus(Constant.ExtractorSetStatus.IN_PROGRESS_MANAGER_COMPLETE);
				}
				// Catches any exception thrown in this record, sets it to error, and processes the next
				catch (Exception exception)
				{
					TextExtractorLog.RaiseUpdate(exception.Message);
					record.Update(Constant.QueueStatus.Error);
				}
			}
		}

		private Boolean HasReachedInterval(Int32 index)
		{
			// Removing one because the index goes up by batchsize + 1 
			return ((index - 1) % Constant.Sizes.MANAGER_CHECK_CANCELLATION_INTERVAL == 0);
		}

		#region Initializers

		private ManagerQueue GetManagerQueue()
		{
			return new ManagerQueue(SqlQueryHelper, EddsDbContext, AgentId);
		}

		private ExtractorSet GetExtractorSet(int extractorSetArtifactId, int workspaceArtifactId)
		{
			return ArtifactFactory.GetInstanceOfExtractorSet(ExecutionIdentity.CurrentUser, workspaceArtifactId, extractorSetArtifactId);
		}

		private DocumentBatch GetDocumentBatch()
		{
			return new DocumentBatch(ArtifactQueries, ServicesMgr);
		}

		#endregion Initializers
	}
}