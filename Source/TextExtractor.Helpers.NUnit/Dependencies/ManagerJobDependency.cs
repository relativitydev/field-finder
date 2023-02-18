using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextExtractor.Agents;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class ManagerJobDependency : ADependency
	{
		public ManagerJob ManagerJob;

		public override void SharedExecute()
		{
			var sqlQuery = Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var artQuery = Pull<ArtifactQueriesDependency>().Queries;
			var factory = Pull<ArtifactFactoryDependency>().ArtifactFactory;
			var helper = Pull<FakeHelper>().Helper;

			ManagerJob = new ManagerJob(sqlQuery, artQuery, helper.GetServicesManager(), helper.GetDBContext(-1), factory, TestConstants.MANAGER_AGENT_ID);
		}
	}
}
