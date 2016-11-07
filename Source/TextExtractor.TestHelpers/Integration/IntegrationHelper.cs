using System;

using Relativity.API;

using TextExtractor.TestHelpers.Integration.Settings;

namespace TextExtractor.TestHelpers.Integration
{
	public class IntegrationHelper : ADependency, IEHHelper, IAgentHelper
	{
		public IntegrationSettings Settings;

		public IntegrationHelper(IntegrationSettings settings)
		{
			this.Settings = settings;

			var context = new IntegrationDBContext(this.Settings.DBContextSettings);
			var servicesMgr = new IntegrationServicesMgr(this.Settings.RsapiSettings);

			base.Add(context);
			base.Add(servicesMgr);
		}

		public IDBContext GetDBContext(int caseID)
		{
			var context = base.Pull<IntegrationDBContext>();

			if (caseID == -1)
			{
				context.Settings.Table = "EDDS";
			}
			else if (caseID != 0)
			{
				context.Settings.Table = String.Concat("EDDS", caseID);
			}

			return context;
		}

		public IServicesMgr GetServicesManager()
		{
			return base.Pull<IntegrationServicesMgr>();
		}

		public IUrlHelper GetUrlHelper()
		{
			throw new System.NotImplementedException();
		}

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}

		public IAuthenticationMgr GetAuthenticationManager()
		{
			throw new NotImplementedException();
		}

		public int GetActiveCaseID()
		{
			return this.Settings.WorkspaceArtifactID;
		}
	}
}