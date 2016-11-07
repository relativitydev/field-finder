using System;
using Relativity.API;
using TextExtractor.EventHandlers.Interfaces;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.EventHandlers.ExtractorSet
{
	public class ConsoleJob : IConsoleJob
	{
		public IServicesMgr SvcMgr { get; set; }
		public ExecutionIdentity ExecutionCurrentUserIdentity { get; set; }
		public IDBContext EddsDbContext { get; set; }
		public IArtifactQueries ArtifactQueries { get; set; }
		public ISqlQueryHelper SqlQueryHelper { get; set; }
		public int WorkspaceArtifactId { get; set; }
		public int ActiveArtifactId { get; set; }
		public int? SavedSearchArtifactId { get; set; }
		public int? ExtractorProfileArtifactId { get; set; }
		public int? SourceLongTextFieldArtifactId { get; set; }
		public string ButtonName { get; set; }

		public ConsoleJob(IServicesMgr svcMgr, IArtifactQueries artifactQueries, ISqlQueryHelper sqlQueryHelper, ExecutionIdentity executionCurrentUserIdentity, IDBContext eddsDbContext, int workspaceArtifactId, int activeArtifactId, int? savedSearchArtifactId, int? extractorProfileArtifactId, int? sourceLongTextFieldArtifactId, string buttonName)
		{
			SvcMgr = svcMgr;
			ArtifactQueries = artifactQueries;
			SqlQueryHelper = sqlQueryHelper;
			ExecutionCurrentUserIdentity = executionCurrentUserIdentity;
			EddsDbContext = eddsDbContext;
			WorkspaceArtifactId = workspaceArtifactId;
			ActiveArtifactId = activeArtifactId;
			SavedSearchArtifactId = savedSearchArtifactId;
			ExtractorProfileArtifactId = extractorProfileArtifactId;
			SourceLongTextFieldArtifactId = sourceLongTextFieldArtifactId;
			ButtonName = buttonName;
		}

		public void Execute()
		{
			try
			{
				switch (ButtonName)
				{
					case Constant.Names.Console.SUBMIT:
						//verify all necessary fields are provided
						if (SavedSearchArtifactId == null || ExtractorProfileArtifactId == null || SourceLongTextFieldArtifactId == null)
						{
							throw new CustomExceptions.TextExtractorSetMissingFieldsException(Constant.ErrorMessages.EXTRACTION_SET_MISSING_FIELDS);
						}

						var dataRow = SqlQueryHelper.RetrieveSingleInManagerQueueByArtifactId(EddsDbContext, ActiveArtifactId, WorkspaceArtifactId);

						if (dataRow == null)
						{
							//Insert record into Text Extractor Manager table
							SqlQueryHelper.InsertRowIntoManagerQueue(EddsDbContext, WorkspaceArtifactId, SavedSearchArtifactId, ActiveArtifactId, ExtractorProfileArtifactId, SourceLongTextFieldArtifactId);

							//Call RSAPI and update Status of Job to Submitted
							ArtifactQueries.UpdateRdoStringFieldValue(SvcMgr, ExecutionCurrentUserIdentity, WorkspaceArtifactId, Constant.Guids.ObjectType.ExtractorSet, Constant.Guids.Fields.ExtractorSet.Status, ActiveArtifactId, Constant.ExtractorSetStatus.SUBMITTED);
						}
						break;
					case Constant.Names.Console.CANCEL:
						//Check if status of Extractor Set is not Complete
						var extractorSetStatus = ArtifactQueries.GetExtractorSetStatus(SvcMgr, ExecutionCurrentUserIdentity, WorkspaceArtifactId, ActiveArtifactId);

						if (extractorSetStatus != Constant.ExtractorSetStatus.COMPLETE)
						{
							//Update status of Extractor Set to Cancelled
							ArtifactQueries.UpdateRdoStringFieldValue(SvcMgr, ExecutionCurrentUserIdentity, WorkspaceArtifactId, Constant.Guids.ObjectType.ExtractorSet, Constant.Guids.Fields.ExtractorSet.Status, ActiveArtifactId, Constant.ExtractorSetStatus.CANCELLED);
						}
						else
						{
							throw new CustomExceptions.TextExtractorSetConsoleCancelException(Constant.ErrorMessages.EXTRACTION_SET_RECORD_COMPLETE);
						}
						break;
				}
			}
			catch (CustomExceptions.TextExtractorSetConsoleCancelException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("{0}, Error Message: {1}", Constant.ErrorMessages.DEFAULT_ERROR_PREPEND, ex));
			}
		}
	}
}
