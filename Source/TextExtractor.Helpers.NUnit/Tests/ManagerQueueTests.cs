using System;
using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;
using Relativity.API;

using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.INTEGRATION)]
	[Category(TestCategory.UNIT)]
	public class ManagerQueueTests : FakesFixture
	{
		#region HasRecords

		[Description("When records have been queried, should have records")]
		[Test]
		public void HasRecords_True()
		{
			Dependencies.Pull<SqlQueryHelperDependency>()
				.WhenThereAreRecordsInTheManagerQueue();

			var queue = GetSystemUnderTest();

			queue.GetNextBatchOfRecords();

			Assert.IsTrue(queue.HasRecords);
		}

		[Description("When records haven't been queried yet, should not have records")]
		[Test]
		public void HasRecords_False()
		{
			var queue = GetSystemUnderTest();

			Assert.IsFalse(queue.HasRecords);
		}

		[Description("When no records return, should not have records")]
		[Test]
		public void HasRecords_False_NoRecords()
		{
			Dependencies.Pull<SqlQueryHelperDependency>()
				.WhenThereAreNoRecordsInTheManagerQueue();

			var queue = GetSystemUnderTest();

			queue.GetNextBatchOfRecords();

			Assert.IsFalse(queue.HasRecords);
		}

		#endregion HasRecords

		#region GetNextBatchOfRecords

		[Description("When valid rows are fed from the queue, should convert into an enumeration of ManagerQueueJob")]
		[Test]
		public void GetNextBatchOfRecords()
		{
			var queue = GetSystemUnderTest();

			var actual = queue.GetNextBatchOfRecords();

			Assert.IsNotNull(actual);
			Assert.IsTrue(queue.HasRecords);
			Assert.Greater(actual.Count(), 0);
		}

		[Description("When a table without rows returns from the queue, should indicate there are no jobs")]
		[Test]
		public void GetNextBatchOfRecords_NoRecords()
		{
			var mock = Dependencies.Pull<SqlQueryHelperDependency>().MockSqlQueryHelper;

			// Moq replaces previous stubs! 
			mock.Setup(
				query =>
					query.RetrieveNextBatchInManagerQueue(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>()))
					.Returns(new DataTable("Nothing in this table"));

			var queue = GetSystemUnderTest();

			var actual = queue.GetNextBatchOfRecords();

			Assert.IsNull(actual);
			Assert.IsFalse(queue.HasRecords);
		}

		#endregion GetNextBatchOfRecords

		#region AddRecordsToWorkerQueue

		[Description("When a null table is passed to the AddRecordsToWorkerQueue method, should throw argument null exception")]
		[Test]
		public void AddRecordsToWorkerQueue_Throws()
		{
			var queue = GetSystemUnderTest();

			Assert.Throws<ArgumentNullException>(() => queue.AddRecordsToWorkerQueue(null));
		}

		[Description("When there are no rows to insert into the worker queue, should return false")]
		[Test]
		public void AddRecordsToWorkerQueue_NoRows()
		{
			var queue = GetSystemUnderTest();

			var addedToWorkerQueue = queue.AddRecordsToWorkerQueue(new DataTable("Table with no records"));

			Assert.IsFalse(addedToWorkerQueue);
		}

		[Description("When there are rows to insert into the worker queue, should return true")]
		[Test]
		public void AddRecordsToWorkerQueue_Rows()
		{
			var table = Dependencies.Pull<SqlQueryHelperReturns>().NextJobInManagerQueue;
			var queue = GetSystemUnderTest();

			var addedToWorkerQueue = queue.AddRecordsToWorkerQueue(table);

			Assert.IsTrue(addedToWorkerQueue);
		}

		#endregion AddRecordsToWorkerQueue

		private ManagerQueue GetSystemUnderTest()
		{
			var sqlQuery = Dependencies.Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var helper = Dependencies.Pull<FakeHelper>().Helper;

			var queue = new ManagerQueue(sqlQuery, helper.GetDBContext(-1), TestConstants.MANAGER_AGENT_ID);

			return queue;
		}
	}
}