using System;
using Relativity.API;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;

namespace TextExtractor.Agents
{
	public abstract class AgentJobBase
	{
		public TextExtractorLog TextExtractorLog { get; set; }

		public virtual void ResetUnfishedJobs(IDBContext eddsDbContext, Int32 agentId, String queueTable)
		{
			ISqlQueryHelper sqlQueryHelper = new SqlQueryHelper();
			sqlQueryHelper.ResetUnfishedJobs(eddsDbContext, agentId, queueTable);
		}

		public abstract void Execute();
	}
}
