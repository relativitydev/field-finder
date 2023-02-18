using System;
using System.IO;
using Moq;
using NUnit.Framework;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.UNIT)]
	public class TextExtractionUtilityTests
	{
        private ITargetRule TargetRule { get; set; }

        [SetUp]
        public void Setup()
        {
            TargetRule = new FakeTargetRule();
            TargetRule.MarkerEnum = Constant.MarkerEnum.PlainText;
            TargetRule.CharacterLength = 1;
            TargetRule.CaseSensitive = true;
            TargetRule.Occurrence = 1;
            TargetRule.MinimumExtractions = 1;
            TargetRule.CustomDelimiter = "; ";
            TargetRule.MaximumExtractions = 1;
            TargetRule.IncludeMarker = true;
        }

        [TearDown]
        public void TearDown()
        {

            TargetRule = null;
        }

		#region Match        

		[Test(Description="Extracting Right, with and without including marker, returns correct results")]
        [TestCase("Lorem ipsum dolor sit amet", "dolor", 4, false, "sit")]
        [TestCase("Lorem ipsum dolor sit amet", "ipsum", 6, false, "dolor")]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 4, true, "dolor sit")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 6, true, "ipsum dolor")]
		public void ExtractRight(string paragraph, string word, int length, bool includeMarker, string expected)
		{
			var utility = GetSystemUnderTest();            

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.IncludeMarker = includeMarker;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extracting Left, with and without including marker, returns correct results")]
        [TestCase("Lorem ipsum dolor sit amet", "dolor", 7, false, "ipsum")]
        [TestCase("Lorem ipsum dolor sit amet", "sit", 6, false, "dolor")]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 7, true, "ipsum dolor")]
		[TestCase("Lorem ipsum dolor sit amet", "sit", 6, true, "dolor sit")]
        public void ExtractLeft(string paragraph, string word, int length, bool includeMarker, string expected)
		{
			var utility = GetSystemUnderTest();
             
            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.IncludeMarker = includeMarker;
			
            var actual = utility.ExtractText(
				paragraph,
				word,
                String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extracting Left and Right, with and without including marker, returns correct results")]    
        [TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, true, "Lorem ipsum dolor")]
        [TestCase("Lorem ipsum dolor sit amet", "sit", 6, true, "dolor sit amet")]
        [TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, false, "Lorem dolor")]
        [TestCase("Lorem ipsum dolor sit amet", "sit", 6, false, "dolor amet")]
        public void ExtractLeftAndRight(string paragraph, string word, int length, bool includeMarker, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.LeftAndRight;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.IncludeMarker = includeMarker;

            TargetRule.CharacterLength = length;
           
            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting to Stop Marker, with and without including marker, returns correct results")]    
        [TestCase("Lorem ipsum dolor sit amet", "ipsum", "sit", 6, false, "dolor")]
        [TestCase("Lorem ipsum dolor sit amet", "dolor", "amet", 5, false, "sit")]
        [TestCase("Lorem ipsum dolor sit amet", "ipsum", "sit", 6, true, "ipsum dolor")]
        [TestCase("Lorem ipsum dolor sit amet", "dolor", "amet", 5, true, "dolor sit amet")]
        public void ExtractStopMarker(string paragraph, string word, string stop, int length, bool includeMarker, string expected)
        {
            var utility = GetSystemUnderTest();
            
            TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.IncludeMarker = includeMarker;

            var actual = utility.ExtractText(
                paragraph,
                word,
                stop,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting with RegEx Marker for whole number, returns correct results")]   
        [TestCase("Lorem 1 ipsum dolor sit amet", @"[-+]?[1-9]\d*\.?[0]*", 6, "1 ipsum")]
        [TestCase("Lorem ipsum 2 dolor sit amet", @"[-+]?[1-9]\d*\.?[0]*", 10, "2 dolor sit")]
        public void ExtractRegex(string paragraph, string word, int length, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.MarkerEnum = Constant.MarkerEnum.RegEx;
            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting with RegEx Start Marker for whole number and Regex Stop Marker for email, returns correct results")]
        [TestCase("Lorem 1 start to stop ipsum@dolor.sit amet", @"[-+]?[1-9]\d*\.?[0]*", @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", 15, "1 start to stop ipsum@dolor.sit")]
        [TestCase("Lorem ipsum 2 from to@here.com dolor sit amet", @"[-+]?[1-9]\d*\.?[0]*", @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", 10, "2 from to@here.com")]
        public void ExtractRegexStop(string paragraph, string word, string stop, int length, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.MarkerEnum = Constant.MarkerEnum.RegEx;
            TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;

            var actual = utility.ExtractText(
                paragraph,
                word,
                stop,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting Right, from Unicode text, returns correct results")]
        [TestCase("化雑緑司覧期鹿格手革映従。", "格", 3, "格手革映")]
		public void ExtractRight_Unicode(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;   

			var actual = utility.ExtractText(
				paragraph,
				word,
                String.Empty,
                TargetRule
				);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extracting Left, from Unicode text, returns correct results")]
        [TestCase("化雑緑司覧期鹿格手革映従。", "手", 5, "司覧期鹿格手")]
		public void ExtractLeft_Unicode(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;    

			var actual = utility.ExtractText(
				paragraph,
				word,
                String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extracting Left and Right, from Unicode text, returns correct results")]
        [TestCase("化雑緑司覧期鹿格手革映従。", "手", 2, "鹿格手革映")]
        public void ExtractLeftAndRight_Unicode(string paragraph, string word, int length, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.LeftAndRight;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting to Stop Marker, from Unicode text, returns correct results")]
        [TestCase("化雑緑司覧期鹿格手革映従。", "手", "映", 5, "手革映")]
        public void ExtractStopMarker_Unicode(string paragraph, string word, string stop, int length, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;

            var actual = utility.ExtractText(
                paragraph,
                word,
                stop,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting Right, from given occurence, returns correct results")]
		[TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 2, "ipsum dolor2")]
		[TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 3, "ipsum dolor3")]
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
		Bank Account Name: abc", "Bank Account Name:", 20, 2, "Bank Account Name: abc")]
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

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.Occurrence = occurence;
            TargetRule.MaximumExtractions = 1;

			var actual = utility.ExtractText(
				paragraph,
				word,
				String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extracting Left, from given occurence, returns correct results")]
		[TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 2, "Lorem2 ipsum")]
		[TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 3, "Lorem3 ipsum")]
		public void ExtractLeft_Occurence(string paragraph, string word, int length, int occurence, string expected)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.Occurrence = occurence;
            TargetRule.MaximumExtractions = 1;

			var actual = utility.ExtractText(
				paragraph,
				word,
                String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extracting Left and Right, from given occurence, returns correct results")]
        [TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 2, "Lorem2 ipsum dolor2")]
        [TestCase("Lorem1 ipsum dolor1 sit1 amet1 Lorem2 ipsum dolor2 sit2 amet2 Lorem3 ipsum dolor3 sit3 amet3", "ipsum", 7, 3, "Lorem3 ipsum dolor3")]
        public void ExtractLeftAndRight_Occurence(string paragraph, string word, int length, int occurence, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.LeftAndRight;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.Occurrence = occurence;
            TargetRule.MaximumExtractions = 1;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting to Stop marker, from given occurence, returns correct results")]
        [TestCase("Lorem1 ipsum dolor1 sit amet1 Lorem2 ipsum dolor2 sit amet2 Lorem3 ipsum dolor3 sit amet3", "ipsum", "sit", 10, 2, "ipsum dolor2 sit")]
        [TestCase("Lorem1 ipsum dolor1 sit amet1 Lorem2 ipsum dolor2 sit amet2 Lorem3 ipsum dolor3 sit amet3", "ipsum", "sit", 10, 3, "ipsum dolor3 sit")]
        public void ExtractStopMarker_Occurence(string paragraph, string word, string stop, int length, int occurence, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.Occurrence = occurence;
            TargetRule.MaximumExtractions = 1;

            var actual = utility.ExtractText(
                paragraph,
                word,
                stop,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting with Regex, from given occurence, returns correct results")]
        [TestCase("Lorem 1 ipsum dolor sit amet Lorem ipsum 2 dolor sit amet Lorem ipsum dolor 3 sit amet", @"[-+]?[1-9]\d*\.?[0]*", 6, 2, "2 dolor")] //Regex for number 
        [TestCase("Lorem 1 ipsum dolor sit amet Lorem ipsum 2 dolor sit amet Lorem ipsum dolor 3 sit amet", @"[-+]?[1-9]\d*\.?[0]*", 4, 3, "3 sit")]
        public void ExtractRegex_Occurence(string paragraph, string word, int length, int occurence, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.MarkerEnum = Constant.MarkerEnum.RegEx;
            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.Occurrence = occurence;
            TargetRule.MaximumExtractions = 1;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting Minimum extractions, from given occurence, returns correct results")]
        [TestCase("Lorem1 ipsum dolor1 sit amet1 Lorem2 ipsum dolor2 sit amet2 Lorem3 ipsum dolor3 sit amet3", "ipsum", 7, 1, 1, "ipsum dolor1 | ipsum dolor2 | ipsum dolor3")]
        [TestCase("Lorem1 ipsum dolor1 sit amet1 Lorem2 ipsum dolor2 sit amet2 Lorem3 ipsum dolor3 sit amet3", "ipsum", 7, 2, 2, "ipsum dolor2 | ipsum dolor3")]
        public void Extract_MinimumExtractions(string paragraph, string word, int length, int occurence,int minExtractions, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.Occurrence = occurence;
            TargetRule.MinimumExtractions = minExtractions;
            TargetRule.MaximumExtractions = 100;
            TargetRule.CustomDelimiter = " | ";

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting Maximum extractions, returns correct results")]
        [TestCase("Lorem1 ipsum dolor1 sit amet1 Lorem2 ipsum dolor2 sit amet2 Lorem3 ipsum dolor3 sit amet3", "ipsum", 7, 1, "ipsum dolor1")]
        [TestCase("Lorem1 ipsum dolor1 sit amet1 Lorem2 ipsum dolor2 sit amet2 Lorem3 ipsum dolor3 sit amet3", "ipsum", 7, 2, "ipsum dolor1 | ipsum dolor2")]
        public void Extract_MaximumExtractions(string paragraph, string word, int length, int maxExtractions, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.Occurrence = 1;
            TargetRule.MinimumExtractions = 1;
            TargetRule.MaximumExtractions = maxExtractions;
            TargetRule.CustomDelimiter = " | ";

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting Right, case insensitive, returns correct results")]
		[TestCase("Lorem ipsum Dolor sit amet", "dolor", 4, false, "Dolor sit")]
		[TestCase("Lorem Ipsum dolor sit amet", "ipsum", 6, false, "Ipsum dolor")]
		public void ExtractRight_Case_Insensitive(string paragraph, string word, int length, bool isCaseSensitive, string expected)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = isCaseSensitive;

			var actual = utility.ExtractText(
				paragraph,
				word,
                String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extracting Left, case insensitive, returns correct results")]
		[TestCase("Lorem ipsum Dolor sit amet", "dolor", 7, false, "ipsum Dolor")]
		[TestCase("Lorem Ipsum dolor Sit amet", "sit", 6, false, "dolor Sit")]
		public void ExtractLeft_Case_Insensitive(string paragraph, string word, int length, bool isCaseSensitive, string expected)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = isCaseSensitive;

			var actual = utility.ExtractText(
				paragraph,
				word,
                String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extracting Left and Right, case insensitive, returns correct results")]
        [TestCase("Lorem Ipsum Dolor sit amet", "ipsum", 7, false, "Lorem Ipsum Dolor")]
        [TestCase("Lorem Ipsum dolor Sit amet", "sit", 6, false, "dolor Sit amet")]
        public void ExtractLeftAndRight_Case_Insensitive(string paragraph, string word, int length, bool isCaseSensitive, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.LeftAndRight;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = isCaseSensitive;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting to Stop Marker, case insensitive, returns correct results")]
        [TestCase("Lorem Ipsum Dolor Sit amet", "ipsum", "sit", 10, false, "Ipsum Dolor Sit")]
        [TestCase("Lorem Ipsum dolor Sit amet", "IPSUM", "SIT", 10, false, "Ipsum dolor Sit")]
        public void ExtractStopMarker_Case_Insensitive(string paragraph, string word, string stop, int length, bool isCaseSensitive, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = isCaseSensitive;

            var actual = utility.ExtractText(
                paragraph,
                word,
                stop,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extracting and Triming white spaces from result, returns correct results")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, Constant.TrimStyleEnum.LeftAndRight, "ipsum dolor")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, Constant.TrimStyleEnum.Left, "ipsum dolor ")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, Constant.TrimStyleEnum.Right, "ipsum dolor")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 7, Constant.TrimStyleEnum.None, "ipsum dolor ")]
		public void TrimTest(string paragraph, string word, int length, Constant.TrimStyleEnum trimStyle, string expected)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = trimStyle;
            TargetRule.CharacterLength = length;

			var actual = utility.ExtractText(
				paragraph,
				word,
                String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "When unexpected exception occurs, throws TextExtractorException")]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 4, "dolor sit")]
		public void UnexpectedExceptionTest(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

			var mockTextExtractionValidator = new Mock<ITextExtractionValidator>();
			mockTextExtractionValidator.Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ITargetRule>())).Throws<Exception>();
			utility.TextExtractionValidator = mockTextExtractionValidator.Object;

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;

			Assert.Throws<CustomExceptions.TextExtractorException>(() => utility.ExtractText(
				paragraph,
				word,
				String.Empty,
                TargetRule), Constant.ErrorMessages.DEFAULT_TEXT_EXTRACTION_ERROR_MESSAGE);
		}

        [Test(Description = "Extract Right, with occurrence max value, throws exception")]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 4)]
		public void ExtractTextToRightThrowsExceptionTest(string paragraph, string word, int length)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;
            TargetRule.Occurrence = Int32.MaxValue;

			var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
			{
				utility.ExtractText(
					paragraph,
					word,
                    String.Empty,
                    TargetRule);
			});

			Assert.That(exception.Message, Is.StringContaining("Occurrence is not between"));
		}

        [Test(Description = "Extract Left, with occurrence max value, throws exception")]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 4)]
		public void ExtractTextToLeftThrowsExceptionTest(string paragraph, string word, int length)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;
            TargetRule.Occurrence = Int32.MaxValue;

			var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
			{
				utility.ExtractText(
					paragraph,
					word,
                    String.Empty,
                    TargetRule);
			});

			Assert.That(exception.Message, Is.StringContaining("Occurrence is not between"));
		}

        [Test(Description = "Extract Left and Right, with occurrence max value, throws exception")]
        [TestCase("Lorem ipsum dolor sit amet", "dolor", 4)]
        public void ExtractTextToLeftAndRightThrowsExceptionTest(string paragraph, string word, int length)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.LeftAndRight;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;
            TargetRule.Occurrence = Int32.MaxValue;

            var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
            {
                utility.ExtractText(
                    paragraph,
                    word,
                    String.Empty,
                    TargetRule);
            });

            Assert.That(exception.Message, Is.StringContaining("Occurrence is not between"));
        }

        [Test(Description = "Extract to Stop Marker, with occurrence max value, throws exception")]
        [TestCase("Lorem ipsum dolor sit amet", "dolor", "amet", 8)]
        public void ExtractTextStopMarkerThrowsExceptionTest(string paragraph, string word, string stop, int length)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;
            TargetRule.Occurrence = Int16.MaxValue;

            var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
            {
                utility.ExtractText(
                    paragraph,
                    word,
                    String.Empty,
                    TargetRule);
            });

            Assert.That(exception.Message, Is.StringContaining("Occurrence is not between"));
        }

        [Test(Description = "Extract from RegEx, with occurrence max value, throws exception")]
        [TestCase("Lorem ipsum dolor sit amet", @"[-+]?[1-9]\d*\.?[0]*", 8)]
        public void ExtractTextRegexThrowsExceptionTest(string paragraph, string word, int length)
        {
            var utility = GetSystemUnderTest();

            TargetRule.MarkerEnum = Constant.MarkerEnum.RegEx;
            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;
            TargetRule.Occurrence = Int16.MaxValue;

            var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
            {
                utility.ExtractText(
                    paragraph,
                    word,
                    String.Empty,
                    TargetRule);
            });

            Assert.That(exception.Message, Is.StringContaining("Occurrence is not between"));
        }

        [Test(Description = "Extract Right, with charachter length max value, throws exception")]
		[TestCase("Lorem ipsum dolor sit amet", "dolor")]
		public void ExtractTextToRightThrowsException1Test(string paragraph, string word)
		{
			var utility = GetSystemUnderTest();
            
            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = Int32.MaxValue;
            TargetRule.CaseSensitive = true;
			
            var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
			{
				utility.ExtractText(
					paragraph,
					word,
                    String.Empty,
                    TargetRule);
			});

			Assert.That(exception.Message, Is.StringContaining("Number of Characters is not between"));
		}

        [Test(Description = "Extract Left, with charachter length max value, throws exception")]
		[TestCase("Lorem ipsum dolor sit amet", "dolor")]
		public void ExtractTextToLeftThrowsException1Test(string paragraph, string word)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = Int32.MaxValue;
            TargetRule.CaseSensitive = true;

			var exception = Assert.Throws<CustomExceptions.TextExtractorException>(() =>
			{
				utility.ExtractText(
					paragraph,
					word,
                    String.Empty,
                    TargetRule);
			});

			Assert.That(exception.Message, Is.StringContaining("Number of Characters is not between"));
		}

        [Test(Description = "Extract Right, with longer charachter length than text source, truncates correctly to end of source text")]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 100, "dolor sit amet")]
		[TestCase("Lorem ipsum dolor sit amet", "ipsum", 100, "ipsum dolor sit amet")]
		public void ExtractRight_Truncate(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;

			var actual = utility.ExtractText(
				paragraph,
				word,
                String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extract Left, with longer charachter length than text source start, truncates correctly to the start of source text")]
		[TestCase("Lorem ipsum dolor sit amet", "dolor", 100, "Lorem ipsum dolor")]
		[TestCase("Lorem ipsum dolor sit amet", "sit", 100, "Lorem ipsum dolor sit")]
		public void ExtractLeft_Truncate(string paragraph, string word, int length, string expected)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;

			var actual = utility.ExtractText(
				paragraph,
				word,
                String.Empty,
                TargetRule);

			Assert.AreEqual(expected, actual);
		}

        [Test(Description = "Extract Left and Right, with longer charachter length than text source, truncates correctly to both ends of source text")]
        [TestCase("Lorem ipsum dolor sit amet", "dolor", 100, "Lorem ipsum dolor sit amet")]
        public void ExtractLeftAndRight_Truncate(string paragraph, string word, int length, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.LeftAndRight;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extract to Stop Marker, with longer charachter length than length between them, truncates result correctly")]
        [TestCase("Lorem ipsum dolor sit amet", "Lorem", "dolor", 100, "Lorem ipsum dolor")]
        [TestCase("Lorem ipsum dolor sit amet", "ipsum", "sit", 100, "ipsum dolor sit")]
        public void ExtractStop_Truncate(string paragraph, string word, string stop, int length, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                word,
                stop,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extract Left from Regex for whole number, with longer charachter length than text source start, truncates correctly to the start of source text")]
        [TestCase("Lorem ipsum 12 dolor sit amet", @"[-+]?[1-9]\d*\.?[0]*", 100, "Lorem ipsum 12")]
        [TestCase("Lorem ipsum dolor 3210 sit amet", @"[-+]?[1-9]\d*\.?[0]*", 100, "Lorem ipsum dolor 3210")]
        public void ExtractRegexLeft_Truncate(string paragraph, string word, int length, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.MarkerEnum = Constant.MarkerEnum.RegEx;
            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Extract Right from Regex for whole number, with longer charachter length than text source, truncates correctly to the end of source text")]
        [TestCase("Lorem ipsum 12 dolor sit amet", @"[-+]?[1-9]\d*\.?[0]*", 100, "12 dolor sit amet")]
        [TestCase("Lorem ipsum dolor 3210 sit amet", @"[-+]?[1-9]\d*\.?[0]*", 100, "3210 sit amet")]
        public void ExtractRegexRight_Truncate(string paragraph, string word, int length, string expected)
        {
            var utility = GetSystemUnderTest();

            TargetRule.MarkerEnum = Constant.MarkerEnum.RegEx;
            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = length;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.AreEqual(expected, actual);
        }

        #endregion Match

        #region NoMatch

        [Test(Description = "Extract Right, with no match, does not extract")]
		[TestCase("Lorem ipsum dolor sit amet", "nothere")]
		public void DoesNotMatchRight(string paragraph, string missingWord)
		{
			var utility = GetSystemUnderTest();


            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = 5;
            TargetRule.CaseSensitive = true;

			var actual = utility.ExtractText(
				paragraph,
				missingWord,
                String.Empty,
                TargetRule);

			Assert.IsNull(actual);
		}

        [Test(Description = "Extract Left, with no match, does not extract")]
		[TestCase("Lorem ipsum dolor sit amet", "nothere")]
		public void DoesNotMatchLeft(string paragraph, string missingWord)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = 5;
            TargetRule.CaseSensitive = true;

			var actual = utility.ExtractText(
				paragraph,
				missingWord,
                String.Empty,
                TargetRule);

			Assert.IsNull(actual);
		}

        [Test(Description = "Extract Left and Right, with no match, does not extract")]
        [TestCase("Lorem ipsum dolor sit amet", "nothere")]
        public void DoesNotMatchLeftAndRight(string paragraph, string missingWord)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.LeftAndRight;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = 5;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                missingWord,
                String.Empty,
                TargetRule);

            Assert.IsNull(actual);
        }

        [Test(Description = "Extract to Stop Marker, with no match, does not extract")]
        [TestCase("Lorem ipsum dolor sit amet","ipsum", "nothere")]
        public void DoesNotMatchStop(string paragraph, string word, string missingStop)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = 5;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                word,
                missingStop,
                TargetRule);

            Assert.IsNull(actual);
        }

        [Test(Description = "Extract from RegEx for whole number, with no match, does not extract")]
        [TestCase("Lorem ipsum dolor sit amet", @"[-+]?[1-9]\d*\.?[0]*")]
        public void DoesNotMatchRegex(string paragraph, string word)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = 5;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

            Assert.IsNull(actual);
        }

        [Test(Description = "Extract Right from Unicode text, with no match, does not extract")]
		[TestCase("化雑緑司覧期鹿格手革映従。", "進")]
		public void DoesNotMatchRight_Unicode(string paragraph, string missingWord)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = 5;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                missingWord,
                String.Empty,
                TargetRule);

			Assert.IsNull(actual);
		}

        [Test(Description = "Extract Left from Unicode text, with no match, does not extract")]
		[TestCase("化雑緑司覧期鹿格手革映従。", "進")]
		public void DoesNotMatchLeft_Unicode(string paragraph, string missingWord)
		{
			var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Left;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = 5;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                missingWord,
                String.Empty,
                TargetRule);

			Assert.IsNull(actual);
		}

        [Test(Description = "Extract Left and Right from Unicode text, with no match, does not extract")]
        [TestCase("化雑緑司覧期鹿格手革映従。", "進")]
        public void DoesNotMatchLeftAndRight_Unicode(string paragraph, string missingWord)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.LeftAndRight;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = 5;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                missingWord,
                String.Empty,
                TargetRule);

            Assert.IsNull(actual);
        }

        [Test(Description = "Extract to Stop Marker from Unicode text, with no match, does not extract")]
        [TestCase("化雑緑司覧期鹿格手革映従。", "格", "進")]
        public void DoesNotMatchStop_Unicode(string paragraph, string word, string missingStop)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            TargetRule.CharacterLength = 5;
            TargetRule.CaseSensitive = true;

            var actual = utility.ExtractText(
                paragraph,
                word,
                missingStop,
                TargetRule);

            Assert.IsNull(actual);
        }

        [Test(Description = "Extract with minimum extractions set to exeed occurences in source, does not extract")]
        [TestCase("Lorem1 ipsum dolor1 sit amet1 Lorem2 ipsum dolor2 sit amet2 Lorem3 ipsum dolor3 sit amet3", "ipsum", 1, 10)]
        [TestCase("Lorem1 ipsum dolor1 sit amet1 Lorem2 ipsum dolor2 sit amet2 Lorem3 ipsum dolor3 sit amet3", "ipsum", 2, 3)]
        public void Extract_MinimumExtractionsExeedsOccurences(string paragraph, string word, int occurence, int minExtractions)
        {
            var utility = GetSystemUnderTest();

            TargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            TargetRule.TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
            TargetRule.CharacterLength = 100;
            TargetRule.Occurrence = occurence;
            TargetRule.MinimumExtractions = minExtractions;
            TargetRule.MaximumExtractions = 100;

            var actual = utility.ExtractText(
                paragraph,
                word,
                String.Empty,
                TargetRule);

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