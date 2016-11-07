using kCura.Relativity.Client;

using Moq;

using Relativity.API;

namespace TextExtractor.TestHelpers.Fakes
{
	public class FakeServicesMgr : ADependency
	{
		public IServicesMgr ServicesMgr;

		public FakeServicesMgr()
		{
			this.Add(new FakeRsapiClient());
			this.SharedExecute();
		}

		public override void SharedExecute()
		{
			var mock = new Mock<IServicesMgr>();
			var fakeRsapi = this.Pull<FakeRsapiClient>();

			// Add stubs here 
			mock.Setup(service => service.CreateProxy<IRSAPIClient>(It.IsAny<ExecutionIdentity>())).Returns(fakeRsapi.RsapiClient);

			this.ServicesMgr = mock.Object;
		}
	}
}