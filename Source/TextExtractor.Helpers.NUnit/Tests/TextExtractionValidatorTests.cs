using System;
using NUnit.Framework;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.UNIT)]
	public class TextExtractionValidatorTests
	{
		private ITextExtractionValidator Sut { get; set; }
        private ITargetRule TargetRule { get; set; }
		private const string TEXT_SOURCE = "abc";
		private const string MATCHING_TEXT = "b";        

		[SetUp]
		public void Setup()
		{
			Sut = new TextExtractionValidator();
            TargetRule = new FakeTargetRule();
            TargetRule.CharacterLength = 1;
            TargetRule.CaseSensitive = true;
            TargetRule.Occurrence = 1;
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
            TargetRule = null;
		}

		[Test(Description="Validator with null text source throws exception")]
		public void InvalidTextSourceTest()
		{
			Assert.Throws<ArgumentNullException>(() => Sut.Validate(null, MATCHING_TEXT, TargetRule));
		}

        [Test(Description = "Validator with null marker throws exception")]
		public void InvalidMatchingTextTest()
		{
            Assert.Throws<ArgumentNullException>(() => Sut.Validate(TEXT_SOURCE, null, TargetRule));
		}

        [Test(Description = "Validator with negative carachter lenght throws exception")]
		public void InvalidCharacterLengthTest()
		{
            TargetRule.CharacterLength = -1;
            Assert.Throws<CustomExceptions.TextExtractorException>(() => Sut.Validate(TEXT_SOURCE, MATCHING_TEXT, TargetRule), Constant.ErrorMessages.CHARACTER_LENGTH_IS_NEGATIVE);
		}

        [Test(Description = "Validator with negative occurence throws exception")]
		public void InvalidOccurrenceTest()
		{
            TargetRule.Occurrence = -1;
            Assert.Throws<CustomExceptions.TextExtractorException>(() => Sut.Validate(TEXT_SOURCE, MATCHING_TEXT, TargetRule), Constant.ErrorMessages.OCCURRENCE_LENGTH_IS_NEGATIVE);
		}

        [Test(Description = "Validator with marker length longer than text source length throws exception")]
		public void MatchingTextLengthGreaterThanTextSourceLengthTest()
		{
			Assert.Throws<CustomExceptions.TextExtractorException>(() => Sut.Validate("abc", "abcd", TargetRule), Constant.ErrorMessages.MATCHING_TEXT_LENGTH_GREATER_THAN_TEXT_SOURCE_LENGTH);
		}
	}
}
