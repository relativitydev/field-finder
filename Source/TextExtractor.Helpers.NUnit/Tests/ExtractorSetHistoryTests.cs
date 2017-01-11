using NUnit.Framework;
using Relativity.API;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.UNIT)]
	public class ExtractorSetHistoryTests : FakesFixture
	{
		[Test(Description = "Extractor Set History Constructor does not throw exception")]
		public void Constructor()
		{
			var random = Dependencies.Pull<RandomGenerator>();
			var artifactQuery = Dependencies.Pull<ArtifactQueriesDependency>().Queries;
			var helper = Dependencies.Pull<FakeHelper>().Helper;

			Assert.DoesNotThrow(() => new ExtractorSetHistory(helper.GetServicesManager(), ExecutionIdentity.CurrentUser, artifactQuery, random.Number(), random.Number(), random.Number(), random.Number(), random.Word(), random.Word(), random.Word(), random.Word()));
		}
	}
}