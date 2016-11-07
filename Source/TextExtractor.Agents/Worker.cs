using System;
using Relativity.API;
using TextExtractor.Helpers;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.Rsapi;
using System.Globalization;

namespace TextExtractor.Agents
{
	[kCura.Agent.CustomAttributes.Name("TextExtractor - Worker ")]
	[System.Runtime.InteropServices.Guid("eccc5e02-263f-4e30-9d25-da5be4389e8e")]
	internal class Worker : kCura.Agent.AgentBase
	{
        public Worker()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        }

		public override void Execute()
		{
			var sqlQueryHelper = new SqlQueryHelper();
			var artifactQueries = new ArtifactQueries();

			var errorLogModel = new ErrorLogModel(sqlQueryHelper, Helper.GetDBContext(-1), AgentID, Constant.Tables.WorkerQueue);
			var uniqueBatchTableName = "[" + Constant.Names.TablePrefix + "Worker_" + Guid.NewGuid() + "_" + AgentID + "]";
			var artifactFactory = new ArtifactFactory(artifactQueries, Helper.GetServicesManager(), errorLogModel);

			var agentWorkerJob = new WorkerJob(AgentID, Helper.GetServicesManager(), ExecutionIdentity.CurrentUser, new SqlQueryHelper(), new ArtifactQueries(), artifactFactory, Constant.AgentType.Worker, uniqueBatchTableName, Helper.GetDBContext(-1));
			var textExtractorLog = new TextExtractorLog();
			textExtractorLog.OnUpdate += MessageRaised;
			agentWorkerJob.TextExtractorLog = textExtractorLog;

			try
			{
				RaiseMessage("Enter Agent", 10);
				agentWorkerJob.Execute();
				RaiseMessage("Exit Agent", 10);
			}
			catch (Exception ex)
			{
				//Raise an error on the agents tab and event viewer
				RaiseError(ex.ToString(), ex.ToString());
				//Add the error to our custom Errors table
				sqlQueryHelper.InsertRowIntoErrorLog(Helper.GetDBContext(-1), agentWorkerJob.WorkspaceArtifactId, Constant.Tables.WorkerQueue, agentWorkerJob.RecordId, agentWorkerJob.AgentId, ex.ToString());
				//Add the error to the Relativity Errors tab
				//this second try catch is in case we have a problem connecting to the RSAPI
				try
				{
					ErrorQueries.WriteError(Helper.GetServicesManager(), ExecutionIdentity.CurrentUser, agentWorkerJob.WorkspaceArtifactId, ex);
				}
				catch (Exception rsapiException)
				{
					RaiseError(rsapiException.ToString(), rsapiException.ToString());
				}
				//Set the status in the queue to error
				if (uniqueBatchTableName != string.Empty)
				{
					sqlQueryHelper.UpdateStatusInWorkerQueue(Helper.GetDBContext(-1), Constant.QueueStatus.Error, uniqueBatchTableName);
				}
			}
			finally
			{
				textExtractorLog.RaiseUpdate("Dropping Worker Queue Batch table.");
				//drop temp table
				sqlQueryHelper.DropTable(Helper.GetDBContext(-1), uniqueBatchTableName);
			}
		}

		public override string Name
		{
			get { return "TextExtractor - Worker "; }
		}

		private void MessageRaised(object sender, string message)
		{
			RaiseMessage(message, 10);
		}
	}
}