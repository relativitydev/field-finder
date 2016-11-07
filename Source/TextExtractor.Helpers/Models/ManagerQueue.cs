using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.Helpers.Models
{
	/// <summary>
	///   Represents the Manager Queue in EDDS
	/// </summary>
	public class ManagerQueue
	{
		private readonly Int32 AgentId;
		private readonly IDBContext EddsDbContext;
		public Boolean HasRecords;

		private readonly String TableName = Constant.Tables.ManagerQueue;
		private readonly ISqlQueryHelper SqlQueryHelper;

		public ManagerQueue(ISqlQueryHelper sqlQueryHelper, IDBContext eddsDbContext, Int32 agentId)
		{
			SqlQueryHelper = sqlQueryHelper;
			EddsDbContext = eddsDbContext;
			AgentId = agentId;
		}

		/// <summary>
		/// The batch of records in the manager queue
		/// </summary>
		public IEnumerable<ManagerQueueRecord> Records { get; private set; }

		/// <summary>
		///   Retrieves a batch of records from the manager queue
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ManagerQueueRecord> GetNextBatchOfRecords()
		{
			var jobs = new List<ManagerQueueRecord>();

			var table = SqlQueryHelper.RetrieveNextBatchInManagerQueue(EddsDbContext, Constant.Sizes.MANAGER_QUEUE_BATCH_SIZE, AgentId);

			HasRecords = TableHasRows(table);

			if (HasRecords)
			{
				foreach (DataRow row in table.Rows)
				{
					// Gives the record a sqlhelper, context and agentID so it may execute on the DB itself
					var record = new ManagerQueueRecord(SqlQueryHelper, EddsDbContext, row);

					// Confirm there is a job
					if (record.Exists)
					{
						jobs.Add(record);
					}
				}

				Records = jobs.AsEnumerable();
			}
			else
			{
				// Return null if there are no jobs
				jobs = null;
			}

			return jobs;
		}

		/// <summary>
		///   Resets all records in the queue that this agent is assigned to
		/// </summary>
		public void ResetUnfinishedRecords()
		{
			SqlQueryHelper.ResetUnfishedJobs(EddsDbContext, AgentId, TableName);
		}

		/// <summary>
		/// Will remove all records assigned to this agent
		/// </summary>
		/// <returns></returns>
		public void RemoveRecordsByAgent()
		{
			SqlQueryHelper.RemoveRecordFromTableByAgentId(EddsDbContext, TableName, AgentId);
		}

		/// <summary>
		///   Bulk adds all the worker queue rows
		/// </summary>
		/// <param name="table"></param>
		public Boolean AddRecordsToWorkerQueue(DataTable table)
		{
			if (table == null || table.Rows == null) { throw new ArgumentNullException("table"); }

			if (table.Rows.Count == 0) { return false; }

			var columns = new List<String> {
				                               "TimeStampUTC",
				                               "WorkspaceArtifactID",
				                               "QueueStatus",
				                               "AgentID",
				                               "ExtractorSetArtifactID",
				                               "DocumentArtifactID",
				                               "ExtractorProfileArtifactID",
				                               "SourceLongTextFieldArtifactID" };

			var mappings = SqlQueryHelper.GetMappingsForWorkerQueue(columns);

			SqlQueryHelper.BulkInsertIntoTable(EddsDbContext, table, mappings, Constant.Tables.WorkerQueue);

			return true;
		}

		private Boolean TableHasRows(DataTable table)
		{
			return (table != null && table.Rows != null && table.Rows.Count > 0);
		}
	}
}