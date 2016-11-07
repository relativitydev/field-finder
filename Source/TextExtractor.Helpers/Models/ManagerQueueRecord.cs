using System;
using System.Data;
using System.Linq;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.Helpers.Models
{
	/// <summary>
	/// Represents a single row from the manager queue 
	/// </summary>
	public class ManagerQueueRecord
	{
		public Int32 RowId { get; private set; }
		public Int32 ExtractorSetArtifactId { get; private set; }
		public Int32 WorkspaceArtifactId { get; private set; }
		public Int32 SavedSearchArtifactId { get; private set; }
		public Int32 ExtractorProfileArtifactId { get; private set; }
		public Int32 SourceLongTextFieldArtifactId { get; private set; }
		public Int32 Status { get; private set; }
		public Boolean Exists { get; private set; }

		private readonly ISqlQueryHelper SqlQueryHelper;
		private readonly IDBContext Context;

		/// <summary>
		/// Constructor that populates the model from a DataRow
		/// </summary>
		/// <param name="sqlQueryHelper"></param>
		/// <param name="context"></param>
		/// <param name="row"></param>
		public ManagerQueueRecord(ISqlQueryHelper sqlQueryHelper, IDBContext context, DataRow row)
		{
			SqlQueryHelper = sqlQueryHelper;
			Context = context;
			Populate(row);
		}

		/// <summary>
		/// Populates job from a DataRow.
		/// </summary>
		/// <param name="row"></param>
		private void Populate(DataRow row)
		{
			if (IsValidRow(row))
			{
				RowId = row.Field<Int32>("ID");
				ExtractorSetArtifactId = row.Field<Int32>("ExtractorSetArtifactID");
				WorkspaceArtifactId = row.Field<Int32>("WorkspaceArtifactID");
				SavedSearchArtifactId = row.Field<Int32>("SavedSearchArtifactID");
				ExtractorProfileArtifactId = row.Field<Int32>("ExtractorProfileArtifactID");
				SourceLongTextFieldArtifactId = row.Field<Int32>("SourceLongTextFieldArtifactID");
				Status = row.Field<Int32>("QueueStatus");
				Exists = true;
			}
		}

		/// <summary>
		/// Removes job from the queue based on its row ID
		/// </summary>
		/// <returns></returns>
		public Boolean Remove()
		{
			return SqlQueryHelper.RemoveRecordFromTableById(Context, Constant.Tables.ManagerQueue, RowId);
		}

		public void Update(Int32 queueStatus)
		{
			SqlQueryHelper.UpdateManagerRecordStatus(Context, queueStatus, RowId);
		}

		private Boolean IsValidRow(DataRow row)
		{
			return (row != null && row.ItemArray.Any());
		}
	}
}