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

		[TestCase(20, 2)]
		[TestCase(1000, 2)]
		[TestCase(1000, 10)]
		[TestCase(1000, 1)]
		[TestCase(1, 10)]
		[TestCase(2, 10)]
		[TestCase(1, 1)]
		[TestCase(1000, null)]
		[TestCase(1, null)]
		[Test]
        //[ReportingTest("8D7A56A5-FD93-4DEC-B4F6-0B491C32675D")]
		[Description("This will test the Golden Flow when no Text Extractor Fields are present in the Manager and Worker queue tables (via associated profiles) and the proper values for the Occurence and Number of Characters are entered.")]
		public void TextExtractorField_PreSave_Golden_Flow(int? numberOfCharacters, int? occurrences)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(0);

			TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences);

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
        //[ReportingTest("B04D98A1-B31C-488C-A0BE-F72548356DC2")]
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

			TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences);

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
        //[ReportingTest("D742A619-61B9-4886-8461-8F02BC348B7B")]
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

			TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences);

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
        //[ReportingTest("9ADF3794-BCB6-4232-9069-A192844FC0C0")]
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

			TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences);

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
        //[ReportingTest("E7031624-8C02-4E52-BAF3-8296E75F7FB6")]
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

			TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_OCCURENCE_MAXIMUM_EXCEEDED.Substring(0, 65)));
		}

		[TestCase(20, 2)]
		[TestCase(1000, 2)]
		[TestCase(1000, 10)]
		[TestCase(1000, 1)]
		[TestCase(1, 10)]
		[TestCase(2, 10)]
		[TestCase(1, 1)]
		[TestCase(1000, null)]
		[TestCase(1, null)]
		[Test]
        //[ReportingTest("5C267CF0-7214-47E8-B91B-F9A5B9C28F33")]
		[Description("This will test when one Text Extractor Profile record is present in the Manager queue tables (via associated profiles) and the proper values for the Occurence and Number of Characters are entered.")]
		public void TextExtractorField_PreSave_Record_Found_In_Manager_Queue(int? numberOfCharacters, int? occurrences)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(1)
				.Returns(0);

			TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_RECORD_DETECTED));
		}

		[TestCase(20, 2)]
		[TestCase(1000, 2)]
		[TestCase(1000, 10)]
		[TestCase(1000, 1)]
		[TestCase(1, 10)]
		[TestCase(2, 10)]
		[TestCase(1, 1)]
		[TestCase(1000, null)]
		[TestCase(1, null)]
		[Test]
        //[ReportingTest("D5A9C1C7-2DC7-4D56-AA33-70AD26A229C5")]
		[Description("This will test when one Text Extractor Profile record is present in the Worker queue tables (via associated profiles) and the proper values for the Occurence and Number of Characters are entered.")]
		public void TextExtractorField_PreSave_Record_Found_In_Worker_Queue(int? numberOfCharacters, int? occurrences)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(10)
				.Returns(15);

			TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_RECORD_DETECTED));
		}

		[TestCase(20, 2)]
		[TestCase(1000, 2)]
		[TestCase(1000, 10)]
		[TestCase(1000, 1)]
		[TestCase(1, 10)]
		[TestCase(2, 10)]
		[TestCase(1, 1)]
		[TestCase(1000, null)]
		[TestCase(1, null)]
		[Test]
        //[ReportingTest("2A6BC7B0-515F-463D-A6E6-105E6B639053")]
		[Description("This will test when many Text Extractor Profile records is present in the Manager and Worker queue tables (via associated profiles) and the proper values for the Occurence and Number of Characters are entered.")]
		public void TextExtractorField_PreSave_Records_Found_In_Manager_And_Worker_Queue(int? numberOfCharacters, int? occurrences)
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<int>(new[] { 1000007, 1000008, 1000009 }));

			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(1);

			TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.TARGET_TEXT_RECORD_DETECTED));
		}

		[TestCase(1, 1)]
		[Test]
        //[ReportingTest("28A1ADBA-7CA5-4160-AF53-B82CC6B4EE14")]
		[Description("This will test when an Exception is thrown.")]
		public void TextExtractorField_PreSave_Exception_SQLQueryHelper(int numberOfCharacters, int? occurrences)
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfilesForField(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Throws(new Exception());

			TextExtractorFieldJob = new TextExtractorTargetTextJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID, numberOfCharacters, occurrences);

			//Act
			var response = TextExtractorFieldJob.ExecutePreSave();

			//Assert
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.DEFAULT_ERROR_PREPEND));
		}
	}
}
