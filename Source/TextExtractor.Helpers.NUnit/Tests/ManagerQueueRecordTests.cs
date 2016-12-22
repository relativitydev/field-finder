using System;
using System.Data;
using global::NUnit.Framework;

//using kCura.Talos.Utility;

using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Tests
{
    //[ReportingSuite("KCD")]
	[TestFixture]
	public class ManagerQueueRecordTests : FakesFixture
	{
		[Description("When the constructor receives a valid row, should exist")]
        //[ReportingTest("cc85e57f-9ab0-4373-b14b-45872566f9df")]
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
        //[ReportingTest("ab1acb43-bdec-4bfb-9944-9fa430e1e02e")]
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
        //[ReportingTest("9efdd08d-2e5b-4580-8778-07b1958b780b")]
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

