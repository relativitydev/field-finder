using kCura.Relativity.Client;

using Moq;

namespace TextExtractor.TestHelpers.Fakes
{
	public class FakeRsapiClient : ADependency
	{
		public IRSAPIClient RsapiClient;

		public FakeRsapiClient()
		{
			this.SharedExecute();
		}

		public override void SharedExecute()
		{
			var mock = new Mock<IRSAPIClient>();

			// Set stubs here 

			// This may not be necessary anymore 
			mock.Setup(client => client.APIOptions).Returns(new APIOptions());

			this.RsapiClient = mock.Object;
		}
	}
}