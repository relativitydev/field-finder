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
	[TestFixture]
	public class ExtractorProfileTests : FakesFixture
	{
		[Description("When the profile is created, should set its properties")]
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

