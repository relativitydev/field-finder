﻿using kCura.Relativity.Client.DTOs;
//using kCura.Talos.Utility;
using Moq;
using NUnit.Framework;
using Relativity.API;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Data;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Tests
{
    //[ReportingSuite("KCD")]
	[TestFixture]
	public class DocumentBatchTests : FakesFixture
	{

		#region AreMoreBatchesTests

		[Description("When there is a token, should have more batches")]
        //[ReportingTest("1F6738C6-E756-41DE-BDF4-DDE5F6E591B1")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void AreMoreBatches_True()
		{
			// Arrange
			// The query will return a token if there are more results
			var returns = this.Dependencies.Pull<ArtifactQueriesReturns>();
			returns.FirstBatchOfDocuments.QueryToken = "BA5BE5DA-2593-43D1-8C1D-C78162B22CB3";
			returns.Push();

			var batch = this.GetSystemUnderTest();

			// Act 
			batch.GetNext(this.Random.Number(), this.Random.Number());

			// Assert 
			Assert.IsTrue(batch.AreMoreBatches);
		}

		[Description("When there is no token, should not have more batches")]
        //[ReportingTest("F9BBADBE-5C16-4252-926C-EC58A9B6E314")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void AreMoreBatches_False()
		{
			// Arrange
			var batch = this.GetSystemUnderTest();

			// Act 
			batch.GetNext(this.Random.Number(), this.Random.Number());

			// Assert 
			Assert.IsFalse(batch.AreMoreBatches);
		}

		#endregion AreMoreBatchesTests

		#region IndexTests

		[Description("When Index is retrieved, should increment by the size of the batch plus one")]
        //[ReportingTest("EC46430F-C435-4DDF-A652-DCBD97F232C8")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Index_IncrementPlusOne()
		{
			// Arrange 
			var batch = this.GetSystemUnderTest();

			// Act 
			batch.Index += 5;

			// Assert 
			Assert.AreEqual(batch.BatchSize + 1, batch.Index);
		}

		[Description("When Index is retrieved again, should increment by the size of the batch only")]
        //[ReportingTest("B419383E-2007-4699-AA7F-B3C19CB3DB57")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Index_IncrementsOnlyByBatch()
		{
			// Arrange 
			var batch = this.GetSystemUnderTest();

			// Act 
			batch.Index += 5;
			batch.Index++;

			// Assert 
			Assert.AreEqual(2 * batch.BatchSize + 1, batch.Index);
		}

		#endregion IndexTests

		#region GetBatchTests

		[Description("After retrieving the first batch of documents, should not be first batch")]
        //[ReportingTest("BA5BE5DA-2593-43D1-8C1D-C78162B22CB3")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void GetNext_AfterFirst()
		{
			// Arrange 
			var batch = this.GetSystemUnderTest();

			// Act 
			batch.GetNext(this.Random.Number(), this.Random.Number());

			// Assert 
			Assert.IsFalse(batch.IsFirstBatch());
		}

		[Description("When it's not the first batch, should not get first batch again")]
        //[ReportingTest("BA5BE5DA-2593-43D1-8C1D-C78162B22CB3")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void GetNext_NotFirstAgain()
		{
			// Arrange 
			var batch = this.GetSystemUnderTest();

			// Index must not be 0 and token must be set for FirstBatch to be false 
			batch.Index++;

			// Return resultset with querytoken 
			var returns = this.Dependencies.Pull<ArtifactQueriesReturns>();
			returns.FirstBatchOfDocuments.QueryToken = "BA5BE5DA-2593-43D1-8C1D-C78162B22CB3";
			returns.Push(); // To reset stub

			// Act 
			var actual = batch.GetNext(this.Random.Number(), this.Random.Number());

			// Assert : Verify it never calls for first batch of documents 
			var mockQuery = this.Dependencies.Pull<ArtifactQueriesDependency>().MockQueries;
			mockQuery.Verify(
				query =>
					query.GetFirstBatchOfDocuments(
						It.IsAny<IServicesMgr>(),
						It.IsAny<ExecutionIdentity>(),
						It.IsAny<int>(),
						It.IsAny<Query<Document>>(),
						It.IsAny<int>()),
				Times.Never());
		}

		#endregion GetBatchTests

		#region ConversionTests

		[Description("When trying to convert a failed result set, should return null")]
        //[ReportingTest("810BB741-B7C9-4CFF-AD18-47E2D31B1E1D")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void ConvertToWorkerQueueTable_FailedResults()
		{
			// Arrange 
			var resultSet = this.Dependencies.Pull<ArtifactGenerator>().GetDocumentResultSet();
			var managerQueue = this.Dependencies.Pull<ManagerQueueRecordDependency>().ManagerRecord;
			var batch = this.GetSystemUnderTest();
			resultSet.Success = false;

			// Act 
			var actual = batch.ConvertToWorkerQueueTable(resultSet, managerQueue);

			// Assert 
			Assert.IsNull(actual);
		}

		#endregion ConversionTests

		#region TotalTests

		[Description("When several batches are retrieved, should total all batches")]
        //[ReportingTest("006175E8-FA9E-42F7-BDBD-71DD4F3BC65E")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Total()
		{
			// Arrange 
			var count = Dependencies.Pull<ArtifactQueriesReturns>().NumberOfDocumentsInFirstBatch;
			var batch = this.GetSystemUnderTest();

			// Act 
			batch.GetNext(Random.Number(), Random.Number());
			batch.GetNext(Random.Number(), Random.Number());
			batch.GetNext(Random.Number(), Random.Number());

			// Assert 
			Assert.AreEqual(count * 3, batch.Total);
		}

		#endregion TotalTests

		private DocumentBatch GetSystemUnderTest()
		{
			var query = this.Dependencies.Pull<ArtifactQueriesDependency>().Queries;
			var mgr = this.Dependencies.Pull<FakeServicesMgr>().ServicesMgr;
			var batch = new DocumentBatch(query, mgr);
			return batch;
		}
	}
}