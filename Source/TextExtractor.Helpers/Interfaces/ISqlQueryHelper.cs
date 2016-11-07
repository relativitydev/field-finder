using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Relativity.API;
using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers.Interfaces
{
	public interface ISqlQueryHelper
	{
		#region AnyQueue

		DataTable RetrieveCommonRow(IDBContext eddsDbContext, Int32 agentId, String queueTableName);

		void ResetUnfishedJobs(IDBContext eddsDbContext, Int32 agentId, String queueTableName);

		Boolean RemoveRecordFromTableById(IDBContext eddsDbContext, String queueTableName, Int32 rowId);

		void RemoveRecordFromTableByAgentId(IDBContext eddsDbContext, String queueTableName, Int32 agentId);

		void RemoveBatchFromQueue(IDBContext eddsDbContext, String uniqueTableName);

		void DropTable(IDBContext dbContext, String tableName);

		void BulkInsertIntoTable(IDBContext dbContext, DataTable sourceDataTable, List<SqlBulkCopyColumnMapping> columnMappings, String destinationTableName);

		Int32 RetrieveJobCountInQueue(IDBContext eddsDbContext, String extractorSetArtifactIds, String queueTableName);

		Int32 RetrieveExtractorProfileCountInQueue(IDBContext dbContext, String extractorProfileArtifactIds, String queueTableName);

		Int32 GetResourceServerByAgentId(IDBContext eddsDbContext, Int32 agentArtifactId);

		Int32 RetrieveProfileCountFromSetTableByTempTable(IDBContext dbContext, String tempTableName);

		Int32 RetrieveRegExCountFromTargetTextTableByTempTable(IDBContext dbContext, String tempTable);

		String RetrieveExtractorSetStatusBySetArtifactId(IDBContext dbContext, Int32 activeArtifactId);

		Int32 RetrieveExtractorSetStatusCountByTempTable(IDBContext dbContext, String tempTableName);

		#endregion AnyQueue

		#region ManagerQueue

		void CreateManagerQueueTable(IDBContext eddsDbContext);

		DataTable RetrieveNextBatchInManagerQueue(IDBContext eddsDbContext, Int32 batchSize, Int32 agentId);

		DataTable RetrieveNextBatchInManagerQueue( IDBContext eddsDbContext, Int32 agentId, Int32 batchSize, String uniqueTableName);

		void UpdateManagerRecordStatus(IDBContext eddsDbContext, Int32 statusId, Int32 recordID);

		DataRow RetrieveSingleInManagerQueueByArtifactId(IDBContext dbContext, Int32 artifactId, Int32 workspaceArtifactId);

		void InsertRowIntoManagerQueue(IDBContext eddsDbContext, Int32 workspaceArtifactId, Int32? savedSearchId, Int32 extractorSetArtifactId, Int32? extractorProfileArtifactId, Int32? sourceLongTextFieldArtifactId);

		#endregion ManagerQueue

		#region WorkerQueue

		void CreateWorkerQueueTable(IDBContext eddsDbContext);

		void UpdateStatusInWorkerQueue(IDBContext eddsDbContext, Int32 statusId, String uniqueTableName);

		DataTable RetrieveNextBatchInWorkerQueue(IDBContext eddsDbContext, Int32 agentId, Int32 batchSize, String uniqueTableName, Int32 resourceServerArtifactId);

		Boolean RemoveRecordFromWorkerQueue(IDBContext eddsDbContext, String queueTableName, Int32 queueId);

		List<SqlBulkCopyColumnMapping> GetMappingsForWorkerQueue(List<String> columnNameList);

		Boolean VerifyIfWorkerQueueContainsDocumentsForJob(IDBContext eddsDbContext, Int32 workspaceArtifactId, Int32 extractorSetArtifactId);

		void DeleteRecordsInWorkerQueueForCancelledExtractorSet(IDBContext eddsDbContext, Int32 workspaceArtifactId, Int32 extractorSetArtifactId);

		void DeleteRecordsInWorkerQueueForCancelledExtractorSetAndAgentId(IDBContext eddsDbContext, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, Int32 agentId);

		#endregion WorkerQueue

		#region ErrorQueue

		void CreateErrorLogTable(IDBContext eddsDbContext);

		void InsertRowIntoErrorLog(
			IDBContext eddsDbContext,
			Int32 workspaceArtifactId,
			String queueTableName,
			Int32 queueRecordId,
			Int32 agentId,
			String errorMessage);

		#endregion ErrorQueue

		#region Extractor Set Profiles

		List<Int32> RetrieveExtractorProfilesForField(IDBContext eddsDbContext, String fieldGuid1, String fieldGuid2, Int32 fieldArtifactId);

		#endregion Extractor Set Profiles

		#region SMTP Settings
		SmtpSettings GetSmptSettings(IDBContext context);
		#endregion
	}
}