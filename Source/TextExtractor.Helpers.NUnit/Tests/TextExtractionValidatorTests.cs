using System;
using NUnit.Framework;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;
using TextExtractor.TestHelpers;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.UNIT)]
	public class TextExtractionValidatorTests
	{
		private ITextExtractionValidator Sut { get; set; }
		private const string TEXT_SOURCE = "abc";
		private const string MATCHING_TEXT = "b";
		private const int CHARACTER_LENGTH = 1;
		private const bool IS_CASE_SENSITIVE = true;
		private const int OCCURRENCE = 1;
        private TargetRule TargetRule;

		[SetUp]
		public void Setup()
		{
			Sut = new TextExtractionValidator();
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void InvalidTextSourceTest()
		{
			Assert.Throws<ArgumentNullException>(() => Sut.Validate(null, MATCHING_TEXT, CHARACTER_LENGTH, IS_CASE_SENSITIVE, OCCURRENCE));
		}

		[Test]
		public void InvalidMatchingTextTest()
		{
			Assert.Throws<ArgumentNullException>(() => Sut.Validate(TEXT_SOURCE, null, CHARACTER_LENGTH, IS_CASE_SENSITIVE, OCCURRENCE));
		}

		[Test]
		public void InvalidCharacterLengthTest()
		{
			Assert.Throws<CustomExceptions.TextExtractorException>(() => Sut.Validate(TEXT_SOURCE, MATCHING_TEXT, -1, IS_CASE_SENSITIVE, OCCURRENCE), Constant.ErrorMessages.CHARACTER_LENGTH_IS_NEGATIVE);
		}

		[Test]
		public void InvalidOccurrenceTest()
		{
			Assert.Throws<CustomExceptions.TextExtractorException>(() => Sut.Validate(TEXT_SOURCE, MATCHING_TEXT, CHARACTER_LENGTH, IS_CASE_SENSITIVE, -1), Constant.ErrorMessages.OCCURRENCE_LENGTH_IS_NEGATIVE);
		}

		[Test]
		public void MatchingTextLengthGreaterThanTextSourceLengthTest()
		{
			Assert.Throws<CustomExceptions.TextExtractorException>(() => Sut.Validate("abc", "abcd", CHARACTER_LENGTH, IS_CASE_SENSITIVE, OCCURRENCE), Constant.ErrorMessages.MATCHING_TEXT_LENGTH_GREATER_THAN_TEXT_SOURCE_LENGTH);
		}
	}
}
