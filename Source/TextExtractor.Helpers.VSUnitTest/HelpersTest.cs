using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextExtractor.TestHelpers;
using TextExtractor.Helpers.Rsapi;
using Relativity.API;
using TextExtractor.TestHelpers.Integration;
using TextExtractor.TestHelpers.Integration.Settings;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers.VSUnitTest
{
	[TestClass]
	public class HelpersTest
	{
		[TestMethod]
		public void GetInstanceOfExtractorRegullarExpressionTest()
		{
			IntegrationSettings testSettings = new IntegrationSettings();

			IntegrationServicesMgr testServiceManager = new IntegrationServicesMgr(testSettings.RsapiSettings);

			ArtifactQueries artifactQueries = new ArtifactQueries();
			ArtifactFactory artifactFactory = new ArtifactFactory(artifactQueries, testServiceManager, null);

			var extractorRegEx = artifactFactory.GetInstanceOfExtractorRegullarExpression(ExecutionIdentity.System, 1016201, 1043028);
		}

		[TestMethod]
		public void ProcessAllRecordsTest()
		{
			IntegrationSettings testSettings = new IntegrationSettings();
			IntegrationDBContext testDBContext = new IntegrationDBContext(testSettings.DBContextSettings);
			IntegrationServicesMgr testServiceManager = new IntegrationServicesMgr(testSettings.RsapiSettings);
			SqlQueryHelper testSqlQueryHelpers = new SqlQueryHelper();
			ArtifactQueries artifactQueries = new ArtifactQueries();
			ArtifactFactory artifactFactory = new ArtifactFactory(artifactQueries, testServiceManager, null);

			Int32 agentId = 1016595;
			Int32 resourceServerId = 1016158;
			string batchTableName = "[" + Constant.Names.TablePrefix + "Worker_" + Guid.NewGuid() + "_" + agentId + "]";
			TextExtractorLog textExtractorLog = new TextExtractorLog();
			ExtractorSetReporting extractorSetReporting = new ExtractorSetReporting(artifactQueries, testServiceManager);

			WorkerQueue workerQueue = new WorkerQueue(testSqlQueryHelpers, artifactQueries, artifactFactory, testDBContext, testServiceManager, ExecutionIdentity.System, 
				agentId, resourceServerId, batchTableName, textExtractorLog, extractorSetReporting);

			workerQueue.ProcessAllRecords();
		}

		[TestMethod]
		public void CreateExtractorRegularExpressionRecordTest()
		{
			IntegrationSettings testSettings = new IntegrationSettings();
			IntegrationDBContext testDBContext = new IntegrationDBContext(testSettings.DBContextSettings);
			IntegrationServicesMgr testServiceManager = new IntegrationServicesMgr(testSettings.RsapiSettings);
			SqlQueryHelper testSqlQueryHelpers = new SqlQueryHelper();
			ArtifactQueries artifactQueries = new ArtifactQueries();

			Int32 workspaceArtifactId = 1016201;
			string regExName = "RegEx created from Unit Test";
			string regEx = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
			string description = "Matches emails";

			artifactQueries.CreateExtractorRegularExpressionRecord(testServiceManager, ExecutionIdentity.CurrentUser, workspaceArtifactId, regExName, regEx, description);
		}
	}
}
