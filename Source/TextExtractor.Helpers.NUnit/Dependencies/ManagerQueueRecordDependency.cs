using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class ManagerQueueRecordDependency : ADependency
	{
		public ManagerQueueRecord ManagerRecord;

		public override void SharedExecute()
		{
			var random = Pull<RandomGenerator>();

			var query = Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var context = Pull<FakeDBContext>().DBContext;
			var table = Pull<SqlQueryHelperReturns>().NextJobInManagerQueue;

			ManagerRecord = new ManagerQueueRecord(query, context, table.Rows[0]);
		}
	}
}