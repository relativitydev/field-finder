using System;

//using kCura.Talos.Utility;

using NUnit.Framework;

using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;

namespace TextExtractor.Helpers.NUnit.Tests
{
    //[ReportingSuite("KCD")]
	[TestFixture]
	public class ExtractorTargetRuleTests : FakesFixture
	{
		[Description("When the constructor receives a valid Rdo, should not throw")]
        //[ReportingTest("d082fefa-7aa9-45a0-a790-7842ec669252")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor()
		{
			var rdo = Dependencies.Pull<ArtifactQueriesReturns>().ExtractorTargetTextRdo;

			Assert.DoesNotThrow(() => new TargetRule(rdo));
		}

		[Description("When the constructor receives a null Rdo, should throw argument null exception")]
        //[ReportingTest("be02461e-7d20-4aba-a8b1-07bceb1eb1a1")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor_NullThrows()
		{
			Assert.Throws<ArgumentNullException>(() => new TargetRule(null));
		}

		[Description("When the constructor receives an Rdo without fields, should throw custom exception")]
        //[ReportingTest("f73a11ad-f3a5-4c24-a317-0719423cc0cd")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor_RdoWithoutFields_Throws()
		{
			var rdo = Dependencies.Pull<ArtifactQueriesReturns>().ExtractorTargetTextRdo;
			rdo.Fields.Clear();

			Assert.Throws<CustomExceptions.TextExtractorException>(() => new TargetRule(rdo));
		}
	}
}

