using System;

namespace TextExtractor.TestHelpers.Integration.Settings
{
	public class IntegrationSettings
	{
		public Int32 WorkspaceArtifactID;

		public RsapiSettings RsapiSettings;

		public DBContextSettings DBContextSettings;

		public IntegrationSettings()
		{
			this.WorkspaceArtifactID = -1;
			this.RsapiSettings = new RsapiSettings()
			{
                ServicesUri = new Uri("http://tsdrelatest01.cloudapp.net/Relativity.Services/"), //Update servername
					User = "Relativity.Admin@kcura.com", //Update user
					Password = "Test1234!" //Update user password
			};
			this.DBContextSettings = new DBContextSettings()
			{
                Server = "tsdrelatest01.cloudapp.net", //Update SQLserver
				Table = "EDDS",
				User = "eddsdbo",
				Password = "edds" //Update eddsdbo password
			};
		}
	}

	public class RsapiSettings
	{
		public String User;

		public String Password;

		public Uri ServicesUri;
	}

	public class DBContextSettings
	{
		public String Server;

		public String Table;

		public String User;

		public String Password;
	}
}