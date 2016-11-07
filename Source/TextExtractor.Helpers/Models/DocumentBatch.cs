using System;
using System.Data;

using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;

using Relativity.API;

using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.Helpers.Models
{
	/// <summary>
	///   Represents a batch of documents returned from a saved search query
	/// </summary>
	public class DocumentBatch
	{
		private Int32 _index;

		private IArtifactQueries ArtQueries;

		//Setting constant here so we can change it for tests 
		public Int32 BatchSize = Constant.Sizes.SAVED_SEARCH_BATCH_SIZE;

		private QueryResultSet<Document> resultSet;

		private IServicesMgr ServicesMgr;

		public Int32 Total;

		public DocumentBatch(IArtifactQueries artifactQueries, IServicesMgr servicesMgr)
		{
			ServicesMgr = servicesMgr;
			ArtQueries = artifactQueries;

			resultSet = new QueryResultSet<Document>();
		}

		public Int32 Index
		{
			get
			{
				return _index;
			}
			set
			{
				if (_index == 0)
				{
					_index = 1 + BatchSize;
				}
				else
				{
					_index += BatchSize;
				}
			}
		}

		public String CurrentQueryToken
		{
			get
			{
				return resultSet.QueryToken;
			}
		}

		public Boolean AreMoreBatches
		{
			get
			{
				if (String.IsNullOrEmpty(CurrentQueryToken))
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		///   Retrieves the next batch of documents from Relativity. Handles QuerySubSet
		/// </summary>
		/// <param name="workspaceArtifactID"></param>
		/// <param name="savedSearchID"></param>
		/// <returns></returns>
		public QueryResultSet<Document> GetNext(Int32 workspaceArtifactID, Int32 savedSearchID)
		{
			var query = new Query<Document> { Condition = new SavedSearchCondition(savedSearchID) };
			query.Fields.Add(new FieldValue("ArtifactID"));

			if (IsFirstBatch())
			{
				resultSet = ArtQueries.GetFirstBatchOfDocuments(
					ServicesMgr,
					ExecutionIdentity.CurrentUser,
					BatchSize,
					query,
					workspaceArtifactID);
			}
			else
			{
				resultSet = ArtQueries.GetSubsequentBatchOfDocuments(
					ServicesMgr,
					ExecutionIdentity.CurrentUser,
					BatchSize,
					Index,
					CurrentQueryToken,
					query,
					workspaceArtifactID);
			}

			Total += resultSet.Results.Count;

			Index++;
			return resultSet;
		}

		/// <summary>
		///   Converts the batch of documents and the manager queue into a worker queue data table
		///   ready for Sql Bulk Insert.
		/// </summary>
		/// <param name="results"></param>
		/// <param name="managerQueueJob"></param>
		/// <returns></returns>
		public DataTable ConvertToWorkerQueueTable(QueryResultSet<Document> results, ManagerQueueRecord managerQueueJob)
		{
			if (HasNoResults(results))
			{
				return null;
			}

			var batchTable = new DataTable();
			batchTable.Columns.Add("TimeStampUTC", typeof(DateTime));
			batchTable.Columns.Add("WorkspaceArtifactID", typeof(Int32));
			batchTable.Columns.Add("QueueStatus", typeof(Int32));
			batchTable.Columns.Add("AgentID", typeof(Int32));
			batchTable.Columns.Add("ExtractorSetArtifactID", typeof(Int32));
			batchTable.Columns.Add("DocumentArtifactID", typeof(Int32));
			batchTable.Columns.Add("ExtractorProfileArtifactID", typeof(Int32));
			batchTable.Columns.Add("SourceLongTextFieldArtifactID", typeof(Int32));

			foreach (var result in results.Results)
			{
				batchTable.Rows.Add(
					DateTime.UtcNow,
					// TimeStampUTC
					managerQueueJob.WorkspaceArtifactId,
					// WorkspaceArtifactID
					Constant.QueueStatus.NotStarted,
					// QueueStatus
					null,
					// AgentID
					managerQueueJob.ExtractorSetArtifactId,
					// ExtractorSetArtifactID
					result.Artifact.ArtifactID,
					// DocumentArtifactID
					managerQueueJob.ExtractorProfileArtifactId,
					// ExtractorProfileArtifactID
					managerQueueJob.SourceLongTextFieldArtifactId // ExtractorProfileArtifactID
					);
			}

			return batchTable;
		}

		public Boolean IsFirstBatch()
		{
			return (Index == 0 && String.IsNullOrEmpty(CurrentQueryToken));
		}

		private Boolean HasNoResults(QueryResultSet<Document> results)
		{
			return (results == null || results.Success == false || results.Results == null || results.Results.Count == 0);
		}
	}
}