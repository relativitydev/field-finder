using System;
using System.Data;
using global::NUnit.Framework;

using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	public class ManagerQueueRecordTests : FakesFixture
	{
		[Description("When the constructor receives a valid row, should exist")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor()
		{
			var query = Dependencies.Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var context = Dependencies.Pull<FakeDBContext>().DBContext;
			var table = Dependencies.Pull<SqlQueryHelperReturns>().NextJobInManagerQueue;

			var record = new ManagerQueueRecord(query, context, table.Rows[0]);

			Assert.IsTrue(record.Exists);
		}

		[Description("When the constructor receives a null row, should not exist")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor_Null()
		{
			var query = Dependencies.Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var context = Dependencies.Pull<FakeDBContext>().DBContext;

			var record = new ManagerQueueRecord(query, context, null);

			Assert.IsFalse(record.Exists);
		}

		[Description("When the constructor receives an incomplete row, should throw an argument exception")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor_IncompleteRowThrows()
		{
			var query = Dependencies.Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var context = Dependencies.Pull<FakeDBContext>().DBContext;
			var table = new DataTable("Incomplete table");
			table.Columns.Add("Test Column", typeof(String));
			table.Rows.Add("Test Row");

			Assert.Throws<ArgumentException>(() => new ManagerQueueRecord(query, context, table.Rows[0]));
		}
	}
}

