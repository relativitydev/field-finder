//using kCura.Talos.Utility;

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
    //[ReportingSuite("KCD")]
	[TestFixture]
	public class ExtractorTargetTextTests : FakesFixture
	{
		[Description("")]
        //[ReportingTest("a86b0bad-5d61-4616-aa84-a88556ef360d")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor()
		{
			var target = GetSystemUnderTest();

			Assert.IsFalse(target.ArtifactId == 0);
			Assert.IsNotNullOrEmpty(target.TargetName);
			Assert.IsNotNullOrEmpty(target.PlainTextStartMarker);
			Assert.IsNotNull(target.DestinationField);
		}

		[Description("")]
        //[ReportingTest("58d5ddf1-581a-4f7e-9bc7-2b44d2607c76")]
		[Category(TestCategory.UNIT)]
		[Test]
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

