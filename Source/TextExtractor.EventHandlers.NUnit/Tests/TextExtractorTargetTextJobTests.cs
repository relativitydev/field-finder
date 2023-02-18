using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Relativity.API;
using TextExtractor.EventHandlers.ExtractorTargetText;
using TextExtractor.EventHandlers.Interfaces;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.TestHelpers;

namespace TextExtractor.EventHandlers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.UNIT)]
	public class TextExtractorTargetTextJobTests
	{
		public ITextExtractorTargetTextJob TextExtractorFieldJob { get; set; }

		#region constants

		private Mock<IDBContext> _mockEddsDbContext;
		private Mock<IDBContext> _mockDbContext;
		private Mock<ISqlQueryHelper> _mockSqlQueryHelper;
		private const int ACTIVE_JOB_ARTIFACT_ID = 1000040;

		#endregion

		[SetUp]
		public void Setup()
		{
			_mockEddsDbContext = new Mock<IDBContext>();
			_mockDbContext = new Mock<IDBContext>();
			_mockSqlQueryHelper = new Mock<ISqlQueryHelper>();
		}

		[TearDown]
		public void TearDown()
		{
			_mockEddsDbContext = null;
			_mockDbContext = null;
			_mockSqlQueryHelper = null;
			TextExtractorFieldJob = null;
		}

		[TestCase(20, 2, 10, 1)]
		[TestCase(1000, 2, 1000, 2)]
		[TestCase(1000, 10, 1000, 10)]
		[TestCase(1000, 1, 1, 1)]
		[TestCase(1, 10, 10, 10)]
		[TestCase(2, 10, 2, 2)]
		[TestCase(1, 1, 1, 1)]
		[TestCase(1000, null, null, null)]
		[TestCase(1, null, null, null)]
		[Test]
		[Description("This will test the Golden Flow when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and the proper values for the Occurence, Number of Characters, MaxExtractions, MinExtractions are entered.")]
        public void TextExtractorField_PreSave_Golden_Flow(int? numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, true);
			Assert.AreEqual(response.Message, string.Empty);
		}

		[TestCase(-1, 2)]
		[TestCase(0, 2)]
		[Test]
		[Description("This will test when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and negative or 0 values are sumitted for the Number of Characters field.")]
		public void TextExtractorField_PreSave_Negative_Number_of_Characters(int numberOfCharacters, int? occurrences)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, 1, 1, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_CHARACTERS_NEGATIVE));
		}


		[TestCase(10001, 2)]
		[TestCase(100000, 2)]
		[Test]
		[Description("This will test when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and the max range is sumitted for the Number of Characters field.")]
		public void TextExtractorField_PreSave_Maximum_Number_of_Characters(int numberOfCharacters, int? occurrences)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, 1, 1, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_CHARACTERS_MAXIMUM_EXCEEDED.Substring(0, 65)));
		}

		[TestCase(10, -1)]
		[TestCase(10, 0)]
		[Test]
		[Description("This will test when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and negative or 0 values are sumitted for the Occurrences field.")]
		public void TextExtractorField_PreSave_Negative_Occurrences(int numberOfCharacters, int? occurrences)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, 1, 1, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_OCCURENCE_NEGATIVE));
		}

		[TestCase(10, 110)]
		[TestCase(10, 1000)]
		[Test]
		[Description("This will test when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and the max range is sumitted for the Occurrences field.")]
		public void TextExtractorField_PreSave_Maximum_Occurrences(int numberOfCharacters, int? occurrences)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, 1, 1, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_OCCURENCE_MAXIMUM_EXCEEDED.Substring(0, 65)));
		}

        [TestCase(10, 1, -1, 1)]
        [TestCase(10, 1, 0, 1)]
        [Test]
        [Description("This will test when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and negative or 0 values are sumitted for the MaxExtractions field.")]
        public void TextExtractorField_PreSave_Negative_MaxExtractions(int? numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
        {
            //Arrange
            _mockSqlQueryHelper
                .Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

            _mockSqlQueryHelper
                .SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(0)
                .Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

            //Act
            var response = TextExtractorFieldJob.ExecutePreSave();

            //Assert
            _mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            Assert.AreEqual(response.Success, false);
            Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_MAXIMUM_EXTRACTIONS_NEGATIVE.Substring(0, 65)));
        }

        [TestCase(10, 1, 1001, 1)]
        [TestCase(10, 1, 10000, 1)]
        [Test]
        [Description("This will test when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and the max range is submitted for the MaxExtractions field.")]
        public void TextExtractorField_PreSave_Maximum_MaxExtractions(int? numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
        {
            //Arrange
            _mockSqlQueryHelper
                .Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

            _mockSqlQueryHelper
                .SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(0)
                .Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

            //Act
            var response = TextExtractorFieldJob.ExecutePreSave();

            //Assert
            _mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            Assert.AreEqual(response.Success, false);
            Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_MAXIMUM_EXTRACTIONS_MAXIMUM_EXCEEDED.Substring(0, 65)));
        }

        [TestCase(10, 1, 1, 2)]
        [TestCase(10, 1, 2, 10)]
        [Test]
        [Description("This will test when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and the max range is not less than MinExtractions field for the MaxExtractions field.")]
        public void TextExtractorField_PreSave_Minimum_MaxExtractions(int? numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
        {
            //Arrange
            _mockSqlQueryHelper
                .Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

            _mockSqlQueryHelper
                .SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(0)
                .Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

            //Act
            var response = TextExtractorFieldJob.ExecutePreSave();

            //Assert
            _mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            Assert.AreEqual(response.Success, false);
            Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_MINIMUM_EXTRACTIONS_EXCEED_MAXIMUM_EXTRACTIONS.Substring(0, 65)));
        }

        [TestCase(10, 1, 1, -1)]
        [TestCase(10, 1, 1, 0)]
        [Test]
        [Description("This will test when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and negative or 0 values are sumitted for the MinExtractions field.")]
        public void TextExtractorField_PreSave_Negative_MinExtractions(int? numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
        {
            //Arrange
            _mockSqlQueryHelper
                .Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

            _mockSqlQueryHelper
                .SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(0)
                .Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

            //Act
            var response = TextExtractorFieldJob.ExecutePreSave();

            //Assert
            _mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            Assert.AreEqual(response.Success, false);
            Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_MINIMUM_EXTRACTIONS_NEGATIVE.Substring(0, 65)));
        }

        [TestCase(10, 1, 102, 101)]
        [TestCase(10, 1, 999, 999)]
        [Test]
        [Description("This will test when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and the max range is submitted for the MinExtractions field.")]
        public void TextExtractorField_PreSave_Maximum_MinExtractions(int? numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
        {
            //Arrange
            _mockSqlQueryHelper
                .Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

            _mockSqlQueryHelper
                .SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(0)
                .Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

            //Act
            var response = TextExtractorFieldJob.ExecutePreSave();

            //Assert
            _mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            Assert.AreEqual(response.Success, false);
            Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_MINIMUM_EXTRACTIONS_MAXIMUM_EXCEEDED.Substring(0, 65)));
        }

		[TestCase(20, 2, 1, 1)]
		[TestCase(1000, 2, 100, 2)]
		[TestCase(1000, 10, 999, 99)]
		[TestCase(1000, 1, 1, 1)]
		[TestCase(1, 10, 1, 1)]
		[TestCase(2, 10, 10, 2)]
		[TestCase(1, 1, 1, 1)]
		[TestCase(1000, null, null, null)]
		[TestCase(1, null, null, null)]
		[Test]
		[Description("This will test when one Text Extractor Profile record is present in the Manager queue tables (via associated profiles) and the proper values for the Occurence, Number of Characters, Maximum Extractions and Minimum Extractions are entered.")]
        public void TextExtractorField_PreSave_Record_Found_In_Manager_Queue(int? numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(1)
				.Returns(0);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_RECORD_DETECTED));
		}

        [TestCase(20, 2, 1, 1)]
        [TestCase(1000, 2, 100, 2)]
        [TestCase(1000, 10, 999, 99)]
        [TestCase(1000, 1, 1, 1)]
        [TestCase(1, 10, 1, 1)]
        [TestCase(2, 10, 10, 2)]
        [TestCase(1, 1, 1, 1)]
        [TestCase(1000, null, null, null)]
        [TestCase(1, null, null, null)]
		[Test]
        [Description("This will test when one Text Extractor Profile record is present in the Worker queue tables (via associated profiles) and the proper values for the Occurence, Number of Characters, Maximum Extractions and Minimum Extractions are entered.")]
        public void TextExtractorField_PreSave_Record_Found_In_Worker_Queue(int? numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(10)
				.Returns(15);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_RECORD_DETECTED));
		}

        [TestCase(20, 2, 1, 1)]
        [TestCase(1000, 2, 100, 2)]
        [TestCase(1000, 10, 999, 99)]
        [TestCase(1000, 1, 1, 1)]
        [TestCase(1, 10, 1, 1)]
        [TestCase(2, 10, 10, 2)]
        [TestCase(1, 1, 1, 1)]
        [TestCase(1000, null, null, null)]
        [TestCase(1, null, null, null)]
		[Test]
		[Description("This will test when many Text Extractor Profile records is present in the Manager and Worker queue tables (via associated profiles) and the proper values for the Occurence and Number of Characters are entered.")]
        public void TextExtractorField_PreSave_Records_Found_In_Manager_And_Worker_Queue(int? numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(1);

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_RECORD_DETECTED));
		}

		[TestCase(1, 1, 1, 1)]
		[Test]
		[Description("This will test when an Exception is thrown.")]
        public void TextExtractorField_PreSave_Exception_SQLQueryHelper(int numberOfCharacters, int? occurrences, int? maxExtractions, int? minExtractions)
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Throws(new Exception());

            TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences, maxExtractions, minExtractions, Constant.MarkerEnum.PlainText.ToString(), false, Constant.DirectionEnum.Right.ToString(), false, null, null, "Test", String.Empty);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.DEFAULT_ERROR_PREPEND));
		}
	}
}
