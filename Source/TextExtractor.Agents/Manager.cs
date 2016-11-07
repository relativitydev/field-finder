using System;
using System.Globalization;
using TextExtractor.Helpers;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.Agents
{
	[kCura.Agent.CustomAttributes.Name("TextExtractor - Manager")]
	[System.Runtime.InteropServices.Guid("2487a3f3-d172-4b4f-ae22-01bd7ec8c20a")]
	public class Manager : kCura.Agent.AgentBase
	{
        public Manager()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        }

		public override void Execute()
		{
			// Relativity Access Helpers 
			var sqlQueryHelper = new SqlQueryHelper();
			var artifactQueries = new ArtifactQueries();
			var eddsDbContext = Helper.GetDBContext(-1);
			var servicesMgr = Helper.GetServicesManager();
			var errorLogModel = new ErrorLogModel(sqlQueryHelper, eddsDbContext, AgentID, Constant.Tables.ManagerQueue);
			var artifactFactory = new ArtifactFactory(artifactQueries, servicesMgr, errorLogModel);

			var agentManagerJob = new ManagerJob(sqlQueryHelper, artifactQueries, servicesMgr, eddsDbContext, artifactFactory, AgentID);

			var agentJobExceptionWrapper = new AgentJobExceptionWrapper(sqlQueryHelper, artifactQueries, servicesMgr, eddsDbContext, AgentID);
			agentJobExceptionWrapper.TextExtractorLog.OnUpdate += MessageRaised;
			agentJobExceptionWrapper.TextExtractorLog.OnError += ErrorRaised;

			agentJobExceptionWrapper.Execute(agentManagerJob);
		}

		public override string Name
		{
			get { return "TextExtractor - Manager"; }
		}

		private void MessageRaised(object sender, string message)
		{
			RaiseMessage(message, 10);
		}

		private void ErrorRaised(object sender, Exception exception)
		{
			RaiseError(exception.Message, exception.StackTrace);
		}
	}
}