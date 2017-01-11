using NUnit.Framework;
using Relativity.API;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	class ArtifactFactoryTests : FakesFixture
	{
		#region ExtractorSet

		[Description("When retrieving an instance of ExtractorSet, should not throw")]
        //[ReportingTest("ee515c91-bfe3-42c6-92e5-7405b908b7d7")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void GetInstanceOfExtractorSet_NoThrow()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var factory = GetSystemUnderTest();

			Assert.DoesNotThrow(() => factory.GetInstanceOfExtractorSet(ExecutionIdentity.CurrentUser, random.Number(), random.Number()));
		}

		[Description("When retrieving an instance of ExtractorSet, should return a valid object")]
        //[ReportingTest("412dac49-7c73-405e-8719-790f76fb9c87")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void GetInstanceOfExtractorSet_ReturnsObj()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var factory = GetSystemUnderTest();

			var actual = factory.GetInstanceOfExtractorSet(ExecutionIdentity.CurrentUser, random.Number(), random.Number());

			Assert.IsTrue(actual.Exists);
			Assert.IsNotNullOrEmpty(actual.SetName);
			Assert.IsFalse(actual.ArtifactId == 0);
			Assert.IsFalse(actual.ExtractorProfileArtifactId == 0);
			Assert.IsFalse(actual.SavedSearchArtifactId == 0);
			Assert.IsFalse(actual.SourceLongTextFieldArtifactId == 0);
		}

		#endregion ExtractorSet

		#region ExtractorProfile

		[Description("When retrieving an instance of ExtractorProfile, should not throw")]
        //[ReportingTest("316303ab-8fdf-4d6a-95c8-3c43e9bc62ee")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void GetInstanceOfExtractorProfile_NoThrow()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var factory = GetSystemUnderTest();

			Assert.DoesNotThrow(() => factory.GetInstanceOfExtractorProfile(ExecutionIdentity.CurrentUser, random.Number(), random.Number()));
		}

		[Description("When retrieving an instance of ExtractorProfile, should return a valid object")]
        //[ReportingTest("c95fa74b-a2a8-4d9c-9fb0-3b28d596ba30")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void GetInstanceOfExtractorProfile_ReturnsObj()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var factory = GetSystemUnderTest();

			var actual = factory.GetInstanceOfExtractorProfile(ExecutionIdentity.CurrentUser, random.Number(), random.Number());

			Assert.IsNotNullOrEmpty(actual.ProfileName);
			Assert.IsFalse(actual.ArtifactId == 0);
			Assert.Greater(actual.FieldCount, 0);
		}

		#endregion ExtractorProfile

		#region ExtractorTargetText

		[Description("When retrieving an instance of ExtractorTargetText, should not throw")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void GetInstanceOfExtractorTargetText_NoThrow()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var factory = GetSystemUnderTest();

			Assert.DoesNotThrow(() => factory.GetInstanceOfExtractorTargetText(ExecutionIdentity.CurrentUser, random.Number(), random.Number()));
		}

		[Description("When retrieving an instance of ExtractorTargetText, should return a valid object")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void GetInstanceOfExtractorTargetText_ReturnsObj()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var factory = GetSystemUnderTest();

			var actual = factory.GetInstanceOfExtractorTargetText(ExecutionIdentity.CurrentUser, random.Number(), random.Number());

			Assert.IsNotNullOrEmpty(actual.TargetName);
			Assert.IsNotNullOrEmpty(actual.PlainTextStartMarker);
			Assert.IsFalse(actual.ArtifactId == 0);
		}

		#endregion ExtractorTargetText

		public ArtifactFactory GetSystemUnderTest()
		{
			var artQuery = Dependencies.Pull<ArtifactQueriesDependency>().Queries;
			var errorQueue = Dependencies.Pull<ErrorQueueDependency>().ErrorLogModel;
			var helper = Dependencies.Pull<FakeHelper>().Helper;

			return new ArtifactFactory(artQuery, helper.GetServicesManager(), errorQueue);
		}
	}
}

