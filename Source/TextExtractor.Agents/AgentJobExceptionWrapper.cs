using System;
using Relativity.API;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.Agents
{
	public class AgentJobExceptionWrapper
	{
		private readonly ISqlQueryHelper SqlQueryHelper;
		public IArtifactQueries ArtifactQueries { get; private set; }
		public IServicesMgr ServicesMgr { get; private set; }
		private readonly IDBContext EddsDbContext;
		private readonly int AgentId;
		private Int32 WorkspaceArtifactId;
		private Int32 ExtractorSetArtifactId;
		public TextExtractorLog TextExtractorLog;

		public AgentJobExceptionWrapper(ISqlQueryHelper sqlQueryHelper, IArtifactQueries artifactQueries, IServicesMgr servicesMgr, IDBContext eddsDbContext, int agentId)
		{
			SqlQueryHelper = sqlQueryHelper;
			ArtifactQueries = artifactQueries;
			ServicesMgr = servicesMgr;
			EddsDbContext = eddsDbContext;
			AgentId = agentId;

			TextExtractorLog = new TextExtractorLog();
		}

		public void Execute(ManagerJob managerAgentJob)
		{
			try
			{
				// Passes on the log
				managerAgentJob.TextExtractorLog = TextExtractorLog;

				// catch and convert to custom exception
				try
				{
					managerAgentJob.Execute();
				}
				catch (Exception exception)
				{
					ThrowAsTextExtractorException(exception);
				}
			}
			catch (CustomExceptions.TextExtractorException exception)
			{
				//Raise an update on the agents tab and event viewer
				TextExtractorLog.RaiseUpdate(exception.Message);

				GetNecessaryInformation();
				AddToErrorQueue(exception, Constant.Tables.ManagerQueue);
			}
		}

		private void GetNecessaryInformation()
		{
			try
			{
				var table = SqlQueryHelper.RetrieveCommonRow(EddsDbContext, AgentId, Constant.Tables.ManagerQueue);
				if (table != null && table.Rows != null && table.Rows.Count > 0)
				{
					WorkspaceArtifactId = Convert.ToInt32(table.Rows[0]["WorkspaceArtifactID"]);
					ExtractorSetArtifactId = Convert.ToInt32(table.Rows[0]["ExtractorSetArtifactID"]);
				}
			}
			catch (Exception retrievalException)
			{
				TextExtractorLog.RaiseError(retrievalException);
			}
		}

		private void AddToErrorQueue(CustomExceptions.TextExtractorException exception, string queueTableName)
		{
			try
			{
				var errorLogModel = new ErrorLogModel(SqlQueryHelper, EddsDbContext, AgentId, queueTableName);
				errorLogModel.InsertRecord(exception, ExtractorSetArtifactId, WorkspaceArtifactId);
			}
			catch (Exception queueException)
			{
				TextExtractorLog.RaiseError(queueException);
			}
		}

		private void ThrowAsTextExtractorException(Exception exception)
		{
			throw new CustomExceptions.TextExtractorException(exception.Message, exception);
		}
	}
}