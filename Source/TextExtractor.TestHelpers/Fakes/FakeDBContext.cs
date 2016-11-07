using Moq;

using Relativity.API;

namespace TextExtractor.TestHelpers.Fakes
{
	public class FakeDBContext : ADependency
	{
		public IDBContext DBContext;

		public FakeDBContext()
		{
			this.SharedExecute();
		}

		public override void SharedExecute()
		{
			var mock = new Mock<IDBContext>();

			// Stubs here 

			this.DBContext = mock.Object;
		}
	}
}