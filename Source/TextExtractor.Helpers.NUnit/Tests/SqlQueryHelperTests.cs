using TextExtractor.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Moq;
using Relativity.API;
using TextExtractor.TestHelpers;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.INTEGRATION)]
	[Category(TestCategory.UNIT)]
	public class SqlQueryHelperTests
	{
		public ISqlQueryHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			this.Sut = new SqlQueryHelper();
		}

		[TearDown]
		public void TearDown()
		{
			this.Sut = null;
		}

		[Test(Description = "Get instance of a batch datatable, column count and that there is next batch in worker queue")]
		public void RetrieveNextBatchInWorkerQueueTests()
		{
			//Arrange
			var mockEddsDbContext = new Mock<IDBContext>();
			mockEddsDbContext
				.Setup(x => x.ExecuteSqlStatementAsDataTable(It.IsAny<String>(), It.IsAny<IEnumerable<SqlParameter>>()))
				.Returns(this.GetWorkerQueueDataTableWithData);

			//Act
			var workerQueueBatch = this.Sut.RetrieveNextBatchInWorkerQueue(mockEddsDbContext.Object, 1, 2, "TempTable", 0);

			//Assert
			Assert.That(workerQueueBatch, Is.InstanceOf(typeof(DataTable)));
			Assert.That(workerQueueBatch.Columns.Count, Is.EqualTo(8));
			foreach (var column in Constant.SqlTableColumns.WorkerQueue.ColumnList)
			{
				Assert.That(workerQueueBatch.Columns.Contains(column), Is.True);
			}
		}

		private DataTable GetWorkerQueueDataTableWithoutData()
		{
			var workerQueue = new DataTable();
			foreach (var column in Constant.SqlTableColumns.WorkerQueue.ColumnList)
			{
				workerQueue.Columns.Add(column, typeof(Int32));
			}

			return workerQueue;
		}

		private DataTable GetWorkerQueueDataTableWithData()
		{
			var workerQueue = this.GetWorkerQueueDataTableWithoutData();

			DataRow dataRow = workerQueue.NewRow();
			dataRow["QueueID"] = 1;
			dataRow["WorkspaceArtifactID"] = 2;
			dataRow["QueueStatus"] = 0;
			dataRow["AgentID"] = 3;
			dataRow["ExtractorSetArtifactID"] = 4;
			dataRow["DocumentArtifactID"] = 5;
			dataRow["ExtractorProfileArtifactID"] = 6;
			dataRow["SourceLongTextFieldArtifactID"] = 7;

			workerQueue.Rows.Add(dataRow);
			return workerQueue;
		}
	}
}
