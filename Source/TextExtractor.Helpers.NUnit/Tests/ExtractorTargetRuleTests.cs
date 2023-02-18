using System;

using NUnit.Framework;

using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	public class ExtractorTargetRuleTests : FakesFixture
	{
		[Description("When the constructor receives a valid Rdo, should not throw")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor()
		{
			var rdo = Dependencies.Pull<ArtifactQueriesReturns>().ExtractorTargetTextRdo;

			Assert.DoesNotThrow(() => new TargetRule(rdo));
		}

		[Description("When the constructor receives a null Rdo, should throw argument null exception")]
		[Category(TestCategory.UNIT)]
		[Test]
		public void Constructor_NullThrows()
		{
			Assert.Throws<ArgumentNullException>(() => new TargetRule(null));
		}

		[Description("When the constructor receives an Rdo without fields, should throw custom exception")]
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

