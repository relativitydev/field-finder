using System;

using global::NUnit.Framework;

using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Rsapi;
using TextExtractor.TestHelpers.Integration;
using TextExtractor.TestHelpers.Integration.Settings;
using System.Globalization;

namespace TextExtractor.Helpers.NUnit.Fixtures
{
	public class IntegrationFixture
	{
		public IArtifactQueries ArtifactQueries;
		public ISqlQueryHelper SqlQueryHelper;
		public IntegrationHelper Helper;
		public ModelDependencies Models;

        public IntegrationFixture()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        }

		[TestFixtureSetUp]
		public void RunBeforeAnyTests()
		{
			var clientSettings = new RsapiSettings()
			{
				ServicesUri = new Uri("http://servername/Relativity.Services/"), //Update servername
				User = "user", //Update user
				Password = "password" //Update user password
			};

			var contextSettings = new DBContextSettings()
			{
				Server = "SQLserver", //Update SQLserver
				Table = "EDDS",
				User = "eddsdbo",
				Password = "password" //Update eddsdbo password
			};

			var settings = new IntegrationSettings()
											 {
												 RsapiSettings = clientSettings,
												 DBContextSettings = contextSettings
											 };
            
			Helper = new IntegrationHelper(settings);
			SqlQueryHelper = new SqlQueryHelper();
			ArtifactQueries = new ArtifactQueries();
			Models = new ModelDependencies();
		}

		[TestFixtureTearDown]
		public void RunAfterAnyTests()
		{
			Helper = null;
			SqlQueryHelper = null;
		}
	}
}