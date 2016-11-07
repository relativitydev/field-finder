using System;

using Moq;

using Relativity.API;

namespace TextExtractor.TestHelpers.Fakes
{
	public class FakeHelper : ADependency
	{
		public IHelper Helper;

		public IAgentHelper AgentHelper;

		public IEHHelper EHHelper;

		public FakeHelper()
		{
			// Add returns at top if necessary 
			this.Add(new FakeDBContext());
			this.Add(new FakeServicesMgr());
			this.SharedExecute();
		}

		public override void SharedExecute()
		{
			var fakeContext = this.Pull<FakeDBContext>();
			var fakeMgr = this.Pull<FakeServicesMgr>();

			// Setting fake IHelper 
			var mock = new Mock<IHelper>();

			// Stubs here 
			mock.Setup(helper => helper.GetDBContext(It.IsAny<Int32>())).Returns(fakeContext.DBContext);
			mock.Setup(helper => helper.GetServicesManager()).Returns(fakeMgr.ServicesMgr);

			this.Helper = mock.Object;

			// Setting fake IAgentHelper 
			var agentMock = new Mock<IAgentHelper>();

			// Stubs here 
			agentMock.Setup(ahelper => ahelper.GetDBContext(It.IsAny<Int32>())).Returns(fakeContext.DBContext);
			agentMock.Setup(helper => helper.GetServicesManager()).Returns(fakeMgr.ServicesMgr);

			AgentHelper = agentMock.Object;

			// Setting fake IEHHelper 
			var ehMock = new Mock<IEHHelper>();

			// Stubs here 
			ehMock.Setup(ahelper => ahelper.GetDBContext(It.IsAny<Int32>())).Returns(fakeContext.DBContext);
			ehMock.Setup(helper => helper.GetServicesManager()).Returns(fakeMgr.ServicesMgr);

			EHHelper = ehMock.Object;
		}
	}
}