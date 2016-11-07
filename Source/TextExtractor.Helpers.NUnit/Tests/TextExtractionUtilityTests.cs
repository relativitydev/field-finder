using System;
using System.IO;
using Moq;
using NUnit.Framework;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.TestHelpers;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.UNIT)]
	public class TextExtractionUtilityTests
	{
		#region Match

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 4, "sit")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 6, "dolor")]
		public void ExtractRight(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Right,
				Constant.TrimStyleEnum.LeftAndRight,
				length,
				true,
				1);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 7, "ipsum")]
		[TestCase("Lorem ipsum dolor sit amet", "sit", 6, "dolor")]
		public void ExtractLeft(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Left,
				Constant.TrimStyleEnum.LeftAndRight,
				length,
				true,
				1);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("化雑緑司覧期鹿格手革映従。", "格", 3, "手革映")]
		public void ExtractRight_Unicode(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Right,
				Constant.TrimStyleEnum.LeftAndRight,
				length,
				true,
				1);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("化雑緑司覧期鹿格手革映従。", "手", 5, "司覧期鹿格")]
		public void ExtractLeft_Unicode(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Left,
				Constant.TrimStyleEnum.LeftAndRight,
				length,
				true,
				1);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 2, "dolor2")]
		[TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 3, "dolor3")]
		[TestCase(@"   GRANTS WORLDWIDE 
				                                                   Bank Account Number: 1001-2110 
				     Page:              1                          BSB Number:          831-787 
				                                                   Remitted Amount:     56,115.84 
				                                                   Fax Number: 
				                                                   Email Address:       eft@grants.com.au 
				
				  PAYMENT NUMBER     INVOICE NUMBER    INVOICE DATE                    AMOUNT DISCOUNT      AMOUNT PAID 
				
				  D I S B280902      211701221123       25/03/2015                   $48,156.63              $48,156.63 
				  D I S B280902      211701222210       25/03/2015                    $7,959.21               $7,959.21 
				   Totals                                                            $56,115.84              $56,115.84 
				", "Bank Account Name:", 20, 2, null)]
		[TestCase(@"Bank Account Name: abc   GRANTS WORLDWIDE 
		                                                   Bank Account Number: 1001-2110 
		     Page:              1                          BSB Number:          831-787 
		                                                   Remitted Amount:     56,115.84 
		                                                   Fax Number: 
		                                                   Email Address:       eft@grants.com.au 
		
		  PAYMENT NUMBER     INVOICE NUMBER    INVOICE DATE                    AMOUNT DISCOUNT      AMOUNT PAID 
		
		  D I S B280902      211701221123       25/03/2015                   $48,156.63              $48,156.63 
		  D I S B280902      211701222210       25/03/2015                    $7,959.21               $7,959.21 
		   Totals                                                            $56,115.84              $56,115.84 
		Bank Account Name: abc", "Bank Account Name:", 20, 2, "abc")]
		[TestCase(@"
		020 - Deloitte Finance Pty Ltd 
		A.B.N. 72 008 531 360 
		225 George Street 
		Sydney                                                                    Deloitte. 
		
		
		DX 1030755E 
		Telephone (02) 9322 7000 
		Fax: (02) 9322 7001 
		www.deloitte.com.au 
		
		
		                Attention: EFT Remittance Advice 
		                GRANTS GROUP 
		                GRANTS WORLDWIDE (AUSTRALIA) 
		                PTY LIMITED 
		                LOCK BAG 2011 
		                SYDNEY NSW 1730                    The following amount was remitted to the account 
		                                                   below on 13/04/2015 
		                ACC - 559711292 BSB - 082057 
		     Vendor Code: 474239                           Bank Account Name:   GRANTS WORLDWIDE 
		                                                   Bank Account Number: 1001-2110 
		     Page:              1                          BSB Number:          831-787 
		                                                   Remitted Amount:     56,115.84 
		                                                   Fax Number: 
		                                                   Email Address:       eft@grants.com.au 
		
		  PAYMENT NUMBER     INVOICE NUMBER    INVOICE DATE                    AMOUNT DISCOUNT      AMOUNT PAID 
		
		  D I S B280902      211701221123       25/03/2015                   $48,156.63              $48,156.63 
		  D I S B280902      211701222210       25/03/2015                    $7,959.21               $7,959.21 
		   Totals                                                            $56,115.84              $56,115.84 
		", "Bank Account Name:", 20, 2, null)]
		public void ExtractRight_Occurence(string paragraph, string word, int length, int occurence, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Right,
				Constant.TrimStyleEnum.LeftAndRight,
				length,
				true,
				occurence);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 2, "Lorem2")]
		[TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 3, "Lorem3")]
		public void ExtractLeft_Occurence(string paragraph, string word, int length, int occurence, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Left,
				Constant.TrimStyleEnum.LeftAndRight,
				length,
				true,
				occurence);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("Lorem ipsum Dolor sit amet", "dolor", 4, false, "sit")]
		[TestCase("Lorem Ipsum dolor sit amet", "ipsum", 6, false, "dolor")]
		public void ExtractRight_Case_Insensitive(string paragraph, string word, int length, bool isCaseSensitive, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Right,
				Constant.TrimStyleEnum.LeftAndRight,
				length,
				isCaseSensitive,
				1);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("Lorem ipsum Dolor sit amet", "dolor", 7, false, "ipsum")]
		[TestCase("Lorem Ipsum dolor Sit amet", "sit", 6, false, "dolor")]
		public void ExtractLeft_Case_Insensitive(string paragraph, string word, int length, bool isCaseSensitive, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Left,
				Constant.TrimStyleEnum.LeftAndRight,
				length,
				isCaseSensitive,
				1);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, Constant.TrimStyleEnum.LeftAndRight, "dolor")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, Constant.TrimStyleEnum.Left, "dolor ")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, Constant.TrimStyleEnum.Right, " dolor")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, Constant.TrimStyleEnum.None, " dolor ")]
		public void TrimTest(string paragraph, string word, int length, Constant.TrimStyleEnum trimStyle, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Right,
				trimStyle,
				length,
				true,
				1);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 4, "sit")]
		public void UnexpectedExceptionTest(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

			var mockTextExtractionValidator = new Mock<ITextExtractionValidator>();
			mockTextExtractionValidator.Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>())).Throws<Exception>();
			utility.TextExtractionValidator = mockTextExtractionValidator.Object;

			Assert.Throws<CustomExceptions.TextExtractorException>(() => utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Right,
				Constant.TrimStyleEnum.LeftAndRight,
				length,
				true,
				1), Constant.ErrorMessages.DEFAULT_TEXT_EXTRACTION_ERROR_MESSAGE);
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 4)]
		public void ExtractTextToRightThrowsExceptionTest(string paragraph, string word, int length)
		{
			var utility = GetSystemUnderTest();

			var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
			{
				utility.ExtractText(
					paragraph,
					word,
					Constant.DirectionEnum.Right,
					Constant.TrimStyleEnum.LeftAndRight,
					length,
					true,
					Int32.MaxValue);
			});

			Assert.That(exception.Message, Is.StringContaining("Occurrence is not between"));
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 4)]
		public void ExtractTextToLeftThrowsExceptionTest(string paragraph, string word, int length)
		{
			var utility = GetSystemUnderTest();

			var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
			{
				utility.ExtractText(
					paragraph,
					word,
					Constant.DirectionEnum.Left,
					Constant.TrimStyleEnum.LeftAndRight,
					length,
					true,
					Int32.MaxValue);
			});

			Assert.That(exception.Message, Is.StringContaining("Occurrence is not between"));
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "dolor")]
		public void ExtractTextToRightThrowsException1Test(string paragraph, string word)
		{
			var utility = GetSystemUnderTest();

			var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
			{
				utility.ExtractText(
					paragraph,
					word,
					Constant.DirectionEnum.Right,
					Constant.TrimStyleEnum.LeftAndRight,
					Int32.MaxValue,
					true,
					1);
			});

			Assert.That(exception.Message, Is.StringContaining("Number of Characters is not between"));
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "dolor")]
		public void ExtractTextToLeftThrowsException1Test(string paragraph, string word)
		{
			var utility = GetSystemUnderTest();

			var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
			{
				utility.ExtractText(
					paragraph,
					word,
					Constant.DirectionEnum.Left,
					Constant.TrimStyleEnum.LeftAndRight,
					Int32.MaxValue,
					true,
					1);
			});

			Assert.That(exception.Message, Is.StringContaining("Number of Characters is not between"));
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 100, " sit amet")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 100, " dolor sit amet")]
		public void ExtractRight_Truncate(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Right,
				Constant.TrimStyleEnum.None,
				length,
				true,
				1);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 100, "Lorem ipsum ")]
		[TestCase("Lorem ipsum dolor sit amet", "sit", 100, "Lorem ipsum dolor ")]
		public void ExtractLeft_Truncate(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				word,
				Constant.DirectionEnum.Left,
				Constant.TrimStyleEnum.None,
				length,
				true,
				1);

			Assert.AreEqual(expected, actual);
		}

		#endregion Match

		#region NoMatch

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "nothere")]
		public void DoesNotMatchRight(string paragraph, string missingWord)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				missingWord,
				Constant.DirectionEnum.Right,
				Constant.TrimStyleEnum.LeftAndRight,
				5,
				true,
				1);

			Assert.IsNull(actual);
		}

		[Test]
		[TestCase("Lorem ipsum dolor sit amet", "nothere")]
		public void DoesNotMatchLeft(string paragraph, string missingWord)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				missingWord,
				Constant.DirectionEnum.Left,
				Constant.TrimStyleEnum.LeftAndRight,
				5,
				true,
				1);

			Assert.IsNull(actual);
		}

		[Test]
		[TestCase("化雑緑司覧期鹿格手革映従。", "進")]
		public void DoesNotMatchRight_Unicode(string paragraph, string missingWord)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				missingWord,
				Constant.DirectionEnum.Right,
				Constant.TrimStyleEnum.LeftAndRight,
				5,
				true,
				1);

			Assert.IsNull(actual);
		}

		[Test]
		[TestCase("化雑緑司覧期鹿格手革映従。", "進")]
		public void DoesNotMatchLeft_Unicode(string paragraph, string missingWord)
		{
			var utility = GetSystemUnderTest();

			var actual = utility.ExtractText(
				paragraph,
				missingWord,
				Constant.DirectionEnum.Left,
				Constant.TrimStyleEnum.LeftAndRight,
				5,
				true,
				1);

			Assert.IsNull(actual);
		}

		#endregion NoMatch

		private TextExtractionUtility GetSystemUnderTest()
		{
			var utility = new TextExtractionUtility(new TextExtractionValidator());

			return utility;
		}
	}
}