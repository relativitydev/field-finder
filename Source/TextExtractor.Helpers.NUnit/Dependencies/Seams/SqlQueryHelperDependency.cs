using System;
using System.Data;
using Moq;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.TestHelpers;

namespace TextExtractor.Helpers.NUnit.Dependencies.Seams
{
	public class SqlQueryHelperDependency : ADependency
	{
		public ISqlQueryHelper SqlQueryHelper;

		public Mock<ISqlQueryHelper> MockSqlQueryHelper;

		private readonly SqlQueryHelperReturns Returns;

		public SqlQueryHelperDependency()
		{
			Returns = new SqlQueryHelperReturns();
			Add(Returns);
			SharedExecute();
		}

		public override void SharedExecute()
		{
			MockSqlQueryHelper = new Mock<ISqlQueryHelper>();

			// Standard stubs
			WhenTheAgentHasAResourceServerId();
			WhenThereAreRecordsInTheManagerQueue();
			WhenThereAreRecordsInTheWorkerQueue();
			WhenARecordIsRemovedFromTheQueue();

			SqlQueryHelper = MockSqlQueryHelper.Object;
		}

		#region Standard

		private void WhenTheAgentHasAResourceServerId()
		{
			MockSqlQueryHelper.Setup(
				query =>
					query.GetResourceServerByAgentId(
						It.IsAny<IDBContext>(),
						It.IsAny<Int32>()
					)).Returns(TestConstants.WORKER_AGENT_ID);
		}

		public void WhenThereAreRecordsInTheManagerQueue()
		{
			MockSqlQueryHelper.Setup(
				query =>
					query.RetrieveNextBatchInManagerQueue(
						It.IsAny<IDBContext>(),
						It.IsAny<int>(),
						It.IsAny<int>()
					)).Returns(Returns.AllJobsInManagerQueue);
		}

		public void WhenThereAreRecordsInTheWorkerQueue()
		{
			MockSqlQueryHelper.Setup(
				query =>
					query.RetrieveNextBatchInWorkerQueue(
						It.IsAny<IDBContext>(),
						It.IsAny<int>(),
						It.IsAny<int>(),
						It.IsAny<string>(),
						It.IsAny<int>()
					)).Returns(Returns.AllJobsInWorkerQueue);
		}

		public void WhenThereAreNoRecordsInTheWorkerQueue()
		{
			MockSqlQueryHelper.Setup(
				query =>
					query.RetrieveNextBatchInWorkerQueue(
						It.IsAny<IDBContext>(),
						It.IsAny<int>(),
						It.IsAny<int>(),
						It.IsAny<string>(),
						It.IsAny<int>()
					)).Returns(new DataTable("Empty Table"));
		}

		public void WhenARecordIsRemovedFromTheQueue()
		{
			MockSqlQueryHelper.Setup(
				query =>
					query.RemoveRecordFromTableByAgentId(
						It.IsAny<IDBContext>(),
						It.IsAny<String>(),
						It.IsAny<Int32>()
						));
		}

		#endregion Standard

		#region Edge Cases

		public void WhenThereAreNoRecordsInTheManagerQueue()
		{
			var emptyTable = new DataTable("Empty Test Table");

			MockSqlQueryHelper.Setup(
				query =>
					query.RetrieveNextBatchInManagerQueue(
						It.IsAny<IDBContext>(),
						It.IsAny<int>(),
						It.IsAny<int>()
					)).Returns(emptyTable);
		}

		public void WhenTheDbContextThrowsRetrievingAManagerQueueBatch()
		{
			MockSqlQueryHelper.Setup(
				query =>
					query.RetrieveNextBatchInManagerQueue(
						It.IsAny<IDBContext>(),
						It.IsAny<int>(),
						It.IsAny<int>()
					)).Throws<Exception>();
		}

		#endregion Edge Cases
	}
}