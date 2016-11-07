using System;
using System.Collections.Generic;
using System.Data;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.Helpers.Models
{
	public class WorkerQueue
	{
		#region Properties

		public ExtractorSetReporting TextExtractorJobReporting { get; set; }
		private readonly Int32 AgentId;
		private readonly IArtifactQueries ArtifactQueries;
		public Int32 BatchSize;
		public String BatchTableName;
		private readonly IDBContext EddsDbContext;
		private readonly ExecutionIdentity ExecutionIdentity;
		public Boolean HasRecords;
		public String QueueTableName;
		public IEnumerable<WorkerQueueRecord> Records;
		private readonly IServicesMgr ServicesMgr;
		private readonly ISqlQueryHelper SqlQueryHelper;
		private readonly TextExtractorLog TextExtractorLog;
		public Int32 ResourceServerId { get; set; }
		private readonly ArtifactFactory ArtifactFactory;
		public Int32 WorkspaceArtifactId;
		private ErrorLogModel ErrorLogModel { get; set; }
		public Int32 ExtractorSetArtifactId { get; set; }
		public Int32 ExtractorProfileArtifactId { get; set; }

		#endregion Properties

		public WorkerQueue(ISqlQueryHelper sqlQueryHelper, IArtifactQueries artifactQueries, ArtifactFactory artifactFactory, IDBContext eddsDbContext, IServicesMgr servicesMgr, ExecutionIdentity executionIdentity, Int32 agentId, Int32 resourceServerId, String batchTableName, TextExtractorLog textExtractorLog, ExtractorSetReporting textExtractorJobReporting)
		{
			InitializeDefaults();

			SqlQueryHelper = sqlQueryHelper;
			ArtifactQueries = artifactQueries;
			EddsDbContext = eddsDbContext;
			ServicesMgr = servicesMgr;
			ExecutionIdentity = executionIdentity;
			AgentId = agentId;
			ResourceServerId = resourceServerId;
			BatchTableName = batchTableName;
			TextExtractorLog = textExtractorLog;
			Records = GetWorkerQueueRecords(textExtractorJobReporting, artifactFactory);
			TextExtractorJobReporting = textExtractorJobReporting;
			ArtifactFactory = artifactFactory;
			ErrorLogModel = new ErrorLogModel(sqlQueryHelper, eddsDbContext, agentId, QueueTableName);
		}

		private void InitializeDefaults()
		{
			QueueTableName = Constant.Tables.WorkerQueue;
			BatchSize = Constant.Sizes.WORKER_BATCH_SIZE;
			HasRecords = false;
		}

		private IEnumerable<WorkerQueueRecord> GetWorkerQueueRecords(ExtractorSetReporting textExtractorJobReporting, ArtifactFactory artifactFactory)
		{
			var records = new List<WorkerQueueRecord>();

			try
			{
				//reset unfinished jobs
				TextExtractorLog.RaiseUpdate(String.Format("Resetting records which failed. [Table = {0}]", QueueTableName));
				ResetUnfinishedJobs();

				//Retrieve records for worker queue
				var workerQueueBatch = SqlQueryHelper.RetrieveNextBatchInWorkerQueue(EddsDbContext, AgentId, BatchSize, BatchTableName, ResourceServerId);
				if (workerQueueBatch != null && workerQueueBatch.Rows.Count > 0)
				{
					HasRecords = true;
					TextExtractorLog.RaiseUpdate("Retrieved record(s) in the worker queue.");

					//Extracting WorkspaceArtifactId and ExtractorSetArtifactID because our worker batch will contain documents only from a unique workspace and TextExtractorJob combination
					WorkspaceArtifactId = Convert.ToInt32(workerQueueBatch.Rows[0]["WorkspaceArtifactID"]);
					ExtractorSetArtifactId = Convert.ToInt32(workerQueueBatch.Rows[0]["ExtractorSetArtifactID"]);
					ExtractorProfileArtifactId = Convert.ToInt32(workerQueueBatch.Rows[0]["ExtractorProfileArtifactID"]);

					foreach (DataRow dataRow in workerQueueBatch.Rows)
					{
						var queueId = Convert.ToInt32(dataRow["QueueID"]);
						var queueStatus = Convert.ToInt32(dataRow["QueueStatus"]);
						var agentId = Convert.ToInt32(dataRow["AgentID"]);
						var documentArtifactId = Convert.ToInt32(dataRow["DocumentArtifactID"]);
						var sourceLongTextFieldArtifactId = Convert.ToInt32(dataRow["SourceLongTextFieldArtifactID"]);

						try
						{
							var extractorSet = artifactFactory.GetInstanceOfExtractorSet(ExecutionIdentity.CurrentUser, WorkspaceArtifactId, ExtractorSetArtifactId);
							var extractorProfile = artifactFactory.GetInstanceOfExtractorProfile(ExecutionIdentity.CurrentUser, WorkspaceArtifactId, ExtractorProfileArtifactId);

							var workerQueueRecord = new WorkerQueueRecord(SqlQueryHelper, ArtifactQueries, EddsDbContext, ServicesMgr, artifactFactory, ExecutionIdentity, queueId, WorkspaceArtifactId, queueStatus, agentId, ExtractorSetArtifactId, documentArtifactId, ExtractorProfileArtifactId, sourceLongTextFieldArtifactId, textExtractorJobReporting, extractorSet, extractorProfile);
							records.Add(workerQueueRecord);
						}
						catch (Exception)
						{
							//Update ExtractorSet details
							ArtifactQueries.UpdateExtractorSetDetails(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, ExtractorSetArtifactId, Constant.ErrorMessages.REQUIRED_FIELDS_ARE_MISSING);

							//delete current record from worker queue
							SqlQueryHelper.RemoveRecordFromWorkerQueue(EddsDbContext, Constant.Tables.WorkerQueue, queueId);

							//delete any record for this Extractor Set in Worker Queue where Agents are not assigned
							SqlQueryHelper.DeleteRecordsInWorkerQueueForCancelledExtractorSet(EddsDbContext, WorkspaceArtifactId, ExtractorSetArtifactId);
						}
					}
				}
				else
				{
					HasRecords = false;
					TextExtractorLog.RaiseUpdate("No records in the worker queue.");
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException("An error occured when retrieving records in the worker queue.", ex);
			}

			return records;
		}

		public void ProcessAllRecords()
		{
			string[] emailRecepients = null;

			try
			{
				if (HasRecords)
				{
					//update job status to worker - in progress
					TextExtractorLog.RaiseUpdate(String.Format("Updating record status to {0}. [WorkspaceArtifactId: {1}, ExtractorSetArtifactID: {2}]", Constant.ExtractorSetStatus.IN_PROGRESS_WORKER_PROCESSING, WorkspaceArtifactId, ExtractorSetArtifactId));
					ArtifactQueries.UpdateRdoStringFieldValue(ServicesMgr, ExecutionIdentity.CurrentUser, WorkspaceArtifactId, Constant.Guids.ObjectType.ExtractorSet, Constant.Guids.Fields.ExtractorSet.Status, ExtractorSetArtifactId, Constant.ExtractorSetStatus.IN_PROGRESS_WORKER_PROCESSING);

					var countForLogging = 1;
					foreach (var workerQueueRecord in Records)
					{
						TextExtractorLog.RaiseUpdate(String.Format("Processing record - {0}. [WorkspaceArtifactId: {1}, ExtractorSetArtifactID: {2}, DocumentArtifactId: {3}]", countForLogging++, WorkspaceArtifactId, ExtractorSetArtifactId, workerQueueRecord.DocumentArtifactId));

						//check for ExtractorSet cancellation
						var extractorSet = ArtifactFactory.GetInstanceOfExtractorSet(ExecutionIdentity.CurrentUser, WorkspaceArtifactId, ExtractorSetArtifactId);
						emailRecepients = extractorSet.EmailRecepients;

						if (extractorSet.IsCancellationRequested())
						{
							TextExtractorLog.RaiseUpdate("Cancellation Requested.");

							//delete all remaining records in current batch worker queue because the ExtractorSet is cancelled
							TextExtractorLog.RaiseUpdate("Deleting all the remaining records in current worker queue batch.");
							SqlQueryHelper.RemoveBatchFromQueue(EddsDbContext, BatchTableName);

							return;
						}

						try
						{
							workerQueueRecord.Process();
						}
						catch (Exception ex)
						{
							var errorContext = String.Format("An error occured when processing the document. [WorkspaceArtifactId: {0}, ExtractorSetArtifactId: {1}, DocumentArtifactId: {2}]", workerQueueRecord.WorkspaceArtifactId, workerQueueRecord.ExtractorSetArtifactId, workerQueueRecord.DocumentArtifactId);

							//log error message to ErrorLog table
							ErrorLogModel.InsertRecord(errorContext, ex, workerQueueRecord.QueueId, workerQueueRecord.WorkspaceArtifactId);

							//Update TextExtractorErrors field on the Document object incase of error
							var exceptionMessage = ExceptionMessageFormatter.GetInnerMostExceptionMessage(ex);
							String extractorSetName = ArtifactQueries.GetExtractorSetNameForArtifactId(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, workerQueueRecord.ExtractorSetArtifactId);
							var fieldValue = String.Format(Constant.ErrorMessages.DOCUMENT_ERROR_ENCOUNTERED, exceptionMessage, extractorSetName);
							ArtifactQueries.AppendToDocumentLongTextFieldValue(ServicesMgr, ExecutionIdentity, workerQueueRecord.WorkspaceArtifactId, workerQueueRecord.DocumentArtifactId, Constant.Guids.Fields.Document.TextExtractorErrors, fieldValue);

							//Update ExtractorSet Details field
							ArtifactQueries.UpdateExtractorSetDetails(ServicesMgr, ExecutionIdentity, workerQueueRecord.WorkspaceArtifactId, workerQueueRecord.ExtractorSetArtifactId, Constant.ExtractorSetStatus.DetailMessages.COMPLETE_WITH_ERRORS_DETAILS);
						}
						finally
						{
							//delete current record from worker queue
							SqlQueryHelper.RemoveRecordFromWorkerQueue(EddsDbContext, workerQueueRecord.QueueTableName, workerQueueRecord.QueueId);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException("An error occured when processing records in the worker queue.", ex);
			}
			finally
			{
				VerifyAndUpdateWorkerStatusToComplete();
				if (emailRecepients != null)
				{
					VerifyAndSendEmailForExtractorSetComplete(emailRecepients);
				}
			}
		}

		/// <summary>
		///   Resets all jobs in the queue that this agent is assigned to
		/// </summary>
		private void ResetUnfinishedJobs()
		{
			SqlQueryHelper.ResetUnfishedJobs(EddsDbContext, AgentId, QueueTableName);
		}

		public void VerifyAndUpdateWorkerStatusToComplete()
		{
			//update status of text extractor job to complete if no records exists for job in worker queue
			var workerQueueHasRecords = SqlQueryHelper.VerifyIfWorkerQueueContainsDocumentsForJob(EddsDbContext, WorkspaceArtifactId, ExtractorSetArtifactId);
			if (!workerQueueHasRecords)
			{
				//update job status to complete except in case when it is cancelled
				string extractorSetStatus = ArtifactQueries.GetExtractorSetStatus(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, ExtractorSetArtifactId);
				if (extractorSetStatus != null && !extractorSetStatus.Equals(Constant.ExtractorSetStatus.CANCELLED))
				{
					//update job status to complete or complete with errors
					string extractorSetDetails = ArtifactQueries.GetExtractorSetDetails(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, ExtractorSetArtifactId);
					if (extractorSetDetails != null)
					{
						//determine new status
						var newExtractorSetStatus = extractorSetDetails.Length > 0 ? Constant.ExtractorSetStatus.COMPLETE_WITH_ERRORS : Constant.ExtractorSetStatus.COMPLETE;

						//update status
						ArtifactQueries.UpdateRdoStringFieldValue(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, Constant.Guids.ObjectType.ExtractorSet, Constant.Guids.Fields.ExtractorSet.Status, ExtractorSetArtifactId, newExtractorSetStatus);
					}
				}
			}
		}

		//Check whether Ectractor set completed (w/ or w/o errors) and send email
		private void VerifyAndSendEmailForExtractorSetComplete(string[] emailRecepients)
		{
			string extractorSetStatus = ArtifactQueries.GetExtractorSetStatus(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, ExtractorSetArtifactId);
			if (extractorSetStatus.Equals(Constant.ExtractorSetStatus.COMPLETE) || extractorSetStatus.Equals(Constant.ExtractorSetStatus.COMPLETE_WITH_ERRORS))
			{
				SmtpSettings smtpSettings = SqlQueryHelper.GetSmptSettings(EddsDbContext);
				if (smtpSettings != null)
				{
					EmailUtility emailUtility = new EmailUtility(smtpSettings);
					String extractorSetNameForEmail = ArtifactQueries.GetExtractorSetNameForArtifactId(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, ExtractorSetArtifactId);
					String extractorSetEmailBody = String.Format("This is an automatic notification from Field Finder application.\r\nExtraction set [{0}] completed with status [{1}].", extractorSetNameForEmail, extractorSetStatus);
					emailUtility.SendEmailNotificationForExtractionSet(extractorSetEmailBody, extractorSetNameForEmail, emailRecepients);
				}
			}
		}
	}
}