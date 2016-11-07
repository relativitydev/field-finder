using System;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.Helpers.Models
{
	/// <summary>
	/// Represents the database queue where errors are stored 
	/// </summary>
	public class ErrorLogModel
	{
		private readonly ISqlQueryHelper SqlQueryHelper;
		private readonly IDBContext EddsDbContext;
		private readonly Int32 AgentId;
		private readonly String QueueTableName;

		public ErrorLogModel(ISqlQueryHelper sqlQueryHelper, IDBContext eddsDbContext, Int32 agentId, String queueTableName)
		{
			SqlQueryHelper = sqlQueryHelper;
			EddsDbContext = eddsDbContext;
			AgentId = agentId;
			QueueTableName = queueTableName;
		}

		/// <summary>
		/// Inserts a single error into the error queue 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="recordId"></param>
		/// <param name="workspaceArtifactId"></param>
		public void InsertRecord(Exception exception, Int32 recordId, Int32 workspaceArtifactId)
		{
			InsertRecord(String.Empty, exception, recordId, workspaceArtifactId);
		}

		/// <summary>
		/// Inserts a single error into the error queue 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="recordId"></param>
		/// <param name="workspaceArtifactId"></param>
		public void InsertRecord(String message, Exception exception, Int32 recordId, Int32 workspaceArtifactId)
		{
			var errorMessage = String.Format("{0}. {1}", message, exception);
			SqlQueryHelper.InsertRowIntoErrorLog(EddsDbContext, workspaceArtifactId, QueueTableName, recordId, AgentId, errorMessage);
		}
	}
}