//using kCura.Talos.Utility;
using NUnit.Framework;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Dependencies.TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Tests
{
    //[ReportingSuite("KCD")]
	[TestFixture]
	public class ExtractorProfileTests : FakesFixture
	{
		[Description("When the profile is created, should set its properties")]
        //[ReportingTest("34fa8b2d-7b20-4a56-9e33-e1cff49822e1")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor()
		{
			var profile = GetSystemUnderTest();

			Assert.IsNotNullOrEmpty(profile.ProfileName);
			Assert.IsFalse(profile.ArtifactId == 0);
			Assert.Greater(profile.FieldCount, 0);
		}

		[Description("When processing all fields, should not throw")]
        //[ReportingTest("49d406f1-43a8-4a86-a3f7-a5a4774c6464")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void ProcessAllFields()
		{
			var document = Dependencies.Pull<TextExtractorDocumentDependency>().Document;
			var extractorSet = Dependencies.Pull<ExtractorSetDependency>().ExtractorSet;

			var profile = GetSystemUnderTest();

			Dependencies.Pull<ArtifactFactoryDependency>().WhenThereAreExtractorTargetTexts();

			Assert.DoesNotThrow(() => profile.ProcessAllTargetTexts(document, extractorSet));
		}

		[Description("When there are no extractor target texts, should not call process field")]
        //[ReportingTest("49d406f1-43a8-4a86-a3f7-a5a4774c6464")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void ProcessAllFields_NoTargetTexts()
		{
			var document = Dependencies.Pull<TextExtractorDocumentDependency>().Document;
			var extractorSet = Dependencies.Pull<ExtractorSetDependency>().ExtractorSet;

			var profile = GetSystemUnderTest();

			profile.ProcessAllTargetTexts(document, extractorSet);

			Dependencies.Pull<ExtractorTargetTextDependency>()
				.VerifyProcessWasNeverCalled();
		}

		[Description("When the job has been cancelled, should not call process field")]
        //[ReportingTest("06a25d58-29d8-4f32-b0aa-ca2279519a54")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void ProcessAllFields_Cancelled()
		{
			var document = Dependencies.Pull<TextExtractorDocumentDependency>().Document;
			var extractorSet = Dependencies.Pull<ExtractorSetDependency>().ExtractorSet;

			Dependencies.Pull<ArtifactQueriesDependency>()
				.WhenTheExtractorSetsAreCancelled();

			var profile = GetSystemUnderTest();

			profile.ProcessAllTargetTexts(document, extractorSet);

			Dependencies.Pull<ExtractorTargetTextDependency>()
				.VerifyProcessWasNeverCalled();
		}

		public ExtractorProfile GetSystemUnderTest()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var factory = Dependencies.Pull<ArtifactFactoryDependency>().MockArtifactFactory.Object;
			var profileRdo = Dependencies.Pull<ArtifactQueriesReturns>().ExtractorProfileRdo;

			return new ExtractorProfile(factory, random.Number(), profileRdo, null);
		}
	}
}

