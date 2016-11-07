using System;
using kCura.Relativity.Client.DTOs;
using Relativity.API;

namespace TextExtractor.Helpers.Rsapi
{
	public interface IArtifactQueries
	{
		void UpdateRdoStringFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Guid objectGuid, Guid fieldGuid, Int32 objectArtifactId, String fieldValue);

		String GetExtractorSetStatus(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId);

		String GetExtractorSetDetails(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId);

		void UpdateTotalExpectedUpdates(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, Int32 totalExpectedUpdates);

		void UpdateNumberOfUpdatesWithValues(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, Int32 numberOfUpdatesWithValues);

		void UpdateExtractorSetStatus(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, String status);

		void UpdateExtractorSetDetails(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, String details);

		RDO GetExtractorTargetTextRdo(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 fieldArtifactId);

		RDO GetExtractorRegularExpressionRdo(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 fieldArtifactId);

		RDO GetExtractorProfileRdo(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorProfileArtifactId);

		RDO GetExtractorSetRdo(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId);

		QueryResultSet<Document> GetFirstBatchOfDocuments(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 batchSize, Query<Document> query, Int32 workspaceArtifactId);

		QueryResultSet<Document> GetSubsequentBatchOfDocuments(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 batchSize, Int32 startIndex, String token, Query<Document> query, Int32 workspaceArtifactId);

		String GetDocumentTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Int32 fieldArtifactId);

		void UpdateDocumentTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Int32 fieldArtifactId, String fieldValue);

		String GetDocumentLongTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Guid fieldGuid);

		void UpdateDocumentLongTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Guid documentFieldGuid, String fieldValue);

		void AppendToDocumentLongTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Guid documentFieldGuid, String fieldValue);

		void CreateExtractorSetHistoryRecord(IServicesMgr svcMgr, ExecutionIdentity identity, String name, Int32 workspaceArtifactId, Int32? extractorSetArtifactId, Int32? documentArtifactId, Int32? destinationFieldArtifactId, String status, String details, String targetName, String startMarker, String stopMarker, String markerType);

		void CreateExtractorSetHistoryRecord(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32? extractorSetArtifactId, Int32? documentArtifactId, String status, String details);

		String GetDocumentIdentifierValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId);

		String GetFieldNameForArtifactId(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 fieldArtifactId);

		String GetExtractorSetNameForArtifactId(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId);

		void CreateExtractorRegularExpressionRecord(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, String regExName, String regEx, String description);
	}
}