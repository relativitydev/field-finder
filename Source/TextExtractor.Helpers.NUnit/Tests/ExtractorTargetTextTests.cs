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
	public class ExtractorTargetTextTests : FakesFixture
	{
		[Category(TestCategory.UNIT)]
		[Test(Description = "When retrieving an instance of ExtractorTargetText, should not throw")]
		public void Constructor()
		{
			var target = GetSystemUnderTest();

			Assert.IsFalse(target.ArtifactId == 0);
			Assert.IsNotNullOrEmpty(target.TargetName);
			Assert.IsNotNullOrEmpty(target.PlainTextStartMarker);
			Assert.IsNotNull(target.DestinationField);
		}

		[Category(TestCategory.UNIT)]
        [Test(Description = "When ExtractorTargetText processes a document, should not throw")]
		public void ProcessField()
		{
			var document = Dependencies.Pull<TextExtractorDocumentDependency>().Document;
			var extractorSet = Dependencies.Pull<ExtractorSetDependency>().ExtractorSet;

			var target = GetSystemUnderTest();

			Assert.DoesNotThrow(() => target.Process(document, extractorSet));
		}

		public ExtractorTargetText GetSystemUnderTest()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var queries = Dependencies.Pull<ArtifactQueriesDependency>().Queries;
			var errorQueue = Dependencies.Pull<ErrorQueueDependency>().ErrorLogModel;
			var helper = Dependencies.Pull<FakeHelper>().Helper;
			var extractorTargetTextRdo = Dependencies.Pull<ArtifactQueriesReturns>().ExtractorTargetTextRdo;

			return new ExtractorTargetText(queries, helper.GetServicesManager(), ExecutionIdentity.CurrentUser, random.Number(), extractorTargetTextRdo, errorQueue);
		}
	}
}

