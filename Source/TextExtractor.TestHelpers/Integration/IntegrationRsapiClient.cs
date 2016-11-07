using kCura.Relativity.Client;

using TextExtractor.TestHelpers.Integration.Settings;

namespace TextExtractor.TestHelpers.Integration
{
	public class IntegrationRsapiClient : ADependency
	{
		public IRSAPIClient Client;

		public RsapiSettings Settings;

		public IntegrationRsapiClient(RsapiSettings settings)
		{
			var authType = new UsernamePasswordCredentials(settings.User, settings.Password);

			this.Client = new RSAPIClient(settings.ServicesUri, authType);
		}
	}
}