using NUnit.Framework;
using Relativity.API;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	public class ExtractorSetTests : FakesFixture
	{
		[Description("When a valid Rdo is returned from Relativity, set should exist")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor_Exists()
		{
			// Arrange 
			var set = GetSystemUnderTest();

			// Assert 
			Assert.IsTrue(set.Exists);
		}

		[Description("When the set has been cancelled, should return cancelled == true")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void IsCancellationRequested()
		{
			//Arrange
			Dependencies.Pull<ArtifactQueriesDependency>()
				.WhenTheExtractorSetsAreCancelled();

			var set = GetSystemUnderTest();

			//Act
			var actual = set.IsCancellationRequested();

			//Assert
			Assert.IsTrue(actual);
		}

		[Description("When the set's status has been updated, its property should be updated")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void UpdateStatus()
		{
			var set = GetSystemUnderTest();

			set.UpdateStatus(Constant.ExtractorSetStatus.COMPLETE);

			Assert.AreEqual(Constant.ExtractorSetStatus.COMPLETE, set.Status);
		}

		[Description("When the set's details have been updated, its property should be updated")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void UpdateDetails()
		{
			var set = GetSystemUnderTest();

			set.UpdateDetails("My Test Details");

			Assert.AreEqual("My Test Details", set.Details);
		}

		private ExtractorSet GetSystemUnderTest()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var query = Dependencies.Pull<ArtifactQueriesDependency>().Queries;
			var reporting = Dependencies.Pull<ExtractorSetReportingDependency>().ExtractorSetReporting;
			var helper = Dependencies.Pull<FakeHelper>().Helper;
			var extractorSetRdo = Dependencies.Pull<ArtifactQueriesReturns>().ExtractorSetRdo;

			var set = new ExtractorSet(query, helper.GetServicesManager(), ExecutionIdentity.CurrentUser, random.Number(), reporting, extractorSetRdo);
			return set;
		}
	}
}