using System;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.Helpers.Models
{
	/// <summary>
	///   Represents a single row in the worker queue table
	/// </summary>
	public class WorkerQueueRecord
	{
		public Int32 QueueId;
		public Int32 WorkspaceArtifactId;
		public Int32 QueueStatus;
		public Int32 AgentId;
		public Int32 ExtractorSetArtifactId;
		public Int32 DocumentArtifactId;
		public Int32 ExtractorProfileArtifactId;
		public Int32 SourceLongTextFieldArtifactId;
		public String QueueTableName;
		public ExtractorSetReporting TextExtractorJobReporting { get; set; }
		public ExtractorSet ExtractorSet { get; set; }
		public ExtractorProfile ExtractorProfile { get; set; }

		public ISqlQueryHelper SqlQueryHelper { get; private set; }
		public IDBContext EddsDbContext { get; private set; }
		public IServicesMgr ServicesMgr { get; private set; }
		public ExecutionIdentity ExecutionIdentity { get; private set; }
		public IArtifactQueries ArtifactQueries { get; private set; }
		public ArtifactFactory ArtifactFactory { get; private set; }

		public String Errors;
		public Boolean Exists;

		public WorkerQueueRecord(ISqlQueryHelper sqlSqlQueryHelper, IArtifactQueries artifactArtifactQueries, IDBContext eddsDbContext, IServicesMgr servicesMgr, ArtifactFactory artifactArtifactFactory, ExecutionIdentity executionExecutionIdentity, Int32 queueId, Int32 workspaceArtifactId, Int32 queueStatus, Int32 agentId, Int32 extractorSetArtifactId, Int32 documentArtifactId, Int32 extractorProfileArtifactId, Int32 sourceLongTextFieldArtifactId, ExtractorSetReporting textExtractorJobReporting, ExtractorSet extractorSet, ExtractorProfile extractorProfile)
		{
			SqlQueryHelper = sqlSqlQueryHelper;
			ArtifactQueries = artifactArtifactQueries;

			QueueId = queueId;
			WorkspaceArtifactId = workspaceArtifactId;
			QueueStatus = queueStatus;
			AgentId = agentId;
			ExtractorSetArtifactId = extractorSetArtifactId;
			DocumentArtifactId = documentArtifactId;
			ExtractorProfileArtifactId = extractorProfileArtifactId;
			SourceLongTextFieldArtifactId = sourceLongTextFieldArtifactId;
			TextExtractorJobReporting = textExtractorJobReporting;
			ArtifactFactory = artifactArtifactFactory;

			QueueTableName = Constant.Tables.WorkerQueue;
			EddsDbContext = eddsDbContext;
			ServicesMgr = servicesMgr;
			ExecutionIdentity = executionExecutionIdentity;

			ExtractorSet = extractorSet;
			ExtractorProfile = extractorProfile;
		}

		public WorkerQueueRecord(ISqlQueryHelper sqlSqlQueryHelper, IDBContext eddsDbContext, Int32 agentId)
		{
			EddsDbContext = eddsDbContext;
			AgentId = agentId;
			QueueTableName = Constant.Tables.WorkerQueue;
			SqlQueryHelper = sqlSqlQueryHelper;
		}

		public void Process()
		{
			var errorContext = "An error occured when extracting text for field.";

			try
			{
				errorContext += String.Format(" [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}]", WorkspaceArtifactId, DocumentArtifactId);

				//retreive text source on Document object
				String textSource = ArtifactQueries.GetDocumentTextFieldValue(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, DocumentArtifactId, SourceLongTextFieldArtifactId);

				if (textSource == null)
				{
					//update TextExtractorDetails field on Document object
					String extractorSetName = ArtifactQueries.GetExtractorSetNameForArtifactId(ServicesMgr, ExecutionIdentity.CurrentUser, WorkspaceArtifactId, ExtractorSet.ArtifactId);
					var fieldValue = string.Format(Constant.ErrorMessages.DOCUMENT_ERROR_ENCOUNTERED, Constant.ErrorMessages.EXTRACTOR_SET_SOURCE_LONG_TEXT_FIELD_IS_EMPTY, extractorSetName);
					ArtifactQueries.AppendToDocumentLongTextFieldValue(ServicesMgr, ExecutionIdentity.CurrentUser, WorkspaceArtifactId, DocumentArtifactId, Constant.Guids.Fields.Document.TextExtractorErrors, fieldValue);

					//Update ExtractorSet Details field
					ArtifactQueries.UpdateExtractorSetDetails(ServicesMgr, ExecutionIdentity.CurrentUser, WorkspaceArtifactId, ExtractorSet.ArtifactId, Constant.ExtractorSetStatus.DetailMessages.COMPLETE_WITH_ERRORS_DETAILS);
				}
                else if (textSource.Length > Constant.Sizes.EXTRACTOR_TARGET_TEXT_SOURCE_LENGTH_MAXIMUM)
                {
                    String extractorSetName = ArtifactQueries.GetExtractorSetNameForArtifactId(ServicesMgr, ExecutionIdentity.CurrentUser, WorkspaceArtifactId, ExtractorSet.ArtifactId);

                    string sourceLengthMaximumExceeded = string.Format(Constant.ErrorMessages.TARGET_TEXT_SOURCE_LENGTH_EXCEEDS_MAXIMUM, Constant.Sizes.EXTRACTOR_TARGET_TEXT_SOURCE_LENGTH_MAXIMUM);

                    var fieldValue = string.Format(Constant.ErrorMessages.DOCUMENT_ERROR_ENCOUNTERED, sourceLengthMaximumExceeded , extractorSetName);
                    ArtifactQueries.AppendToDocumentLongTextFieldValue(ServicesMgr, ExecutionIdentity.CurrentUser, WorkspaceArtifactId, DocumentArtifactId, Constant.Guids.Fields.Document.TextExtractorErrors, fieldValue);

                    //Update ExtractorSet Details field
                    ArtifactQueries.UpdateExtractorSetDetails(ServicesMgr, ExecutionIdentity.CurrentUser, WorkspaceArtifactId, ExtractorSet.ArtifactId, Constant.ExtractorSetStatus.DetailMessages.COMPLETE_WITH_ERRORS_DETAILS);
                }
				else
				{
					var textExtractorDocument = new ExtractorSetDocument(DocumentArtifactId, textSource).GetInstance();

					//extract text and update fields.
					ExtractorProfile.ProcessAllTargetTexts(textExtractorDocument, ExtractorSet);

					//update the value for reporting
					ExtractorSet.ExtractorSetReporting.SetNumberOfUpdatesWithValues(WorkspaceArtifactId, ExtractorSetArtifactId, ExtractorProfile.NumberOfTargetTextsWithValues);
				}

				// Reset the count because it's being done on one instance of ExtractorProfile
				ExtractorProfile.ResetNumberOfTargetTextsWithValues();
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}
	}
}