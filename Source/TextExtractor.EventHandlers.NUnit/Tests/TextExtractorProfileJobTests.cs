using System;
using Moq;
using NUnit.Framework;
using Relativity.API;
using TextExtractor.EventHandlers.ExtractorProfile;
using TextExtractor.EventHandlers.Interfaces;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.TestHelpers;

namespace TextExtractor.EventHandlers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.INTEGRATION)]
	[Category(TestCategory.UNIT)]
	public class TextExtractorProfileJobTests
	{
		public ITextExtractorProfileJob TextExtractorProfileJob { get; set; }

		#region constants

		private const int ACTIVE_JOB_ARTIFACT_ID = 1000040;
		private Mock<IDBContext> _mockDbContext;
		private Mock<ISqlQueryHelper> _mockSqlQueryHelper;

		#endregion

		[SetUp]
		public void Setup()
		{
			_mockDbContext = new Mock<IDBContext>();
			_mockSqlQueryHelper = new Mock<ISqlQueryHelper>();
		}

		[TearDown]
		public void TearDown()
		{
			_mockDbContext = null;
			_mockSqlQueryHelper = null;
			TextExtractorProfileJob = null;
		}

		[Test]
        //[ReportingTest("FE9D5EB4-DA9D-4CBD-B15B-77F538721C9E")]
		[Description("This will test the Golden Flow when no Text Extractor Profiles are present in the Manager and Worker queue tables.")]
		public void TextExtractorProfile_PreSave_Golden_Flow()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(0);

			TextExtractorProfileJob = new TextExtractorProfileJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorProfileJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, true);
			Assert.AreEqual(response.Message, string.Empty);
		}

		[Test]
        //[ReportingTest("C8C336E6-888B-426B-8C2C-1B5A52BDF57D")]
		[Description("This will test when one Text Extractor Profile record is present in the Manager queue tables.")]
		public void TextExtractorProfile_PreSave_Record_Found_In_Manager_Queue()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(1)
				.Returns(0);

			TextExtractorProfileJob = new TextExtractorProfileJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorProfileJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.EXTRACTION_PROFILE_RECORD_DETECTED));
		}

		[Test]
        //[ReportingTest("555B7DBA-0713-435F-93AE-3E0C61B61F61")]
		[Description("This will test when one Text Extractor Profile record is present in the Worker queue tables.")]
		public void TextExtractorProfile_PreSave_Record_Found_In_Worker_Queue()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(1);

			TextExtractorProfileJob = new TextExtractorProfileJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorProfileJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.EXTRACTION_PROFILE_RECORD_DETECTED));
		}

		[Test]
        //[ReportingTest("DEAA2E4F-D3D5-4BD4-B072-8574C7A866AA")]
		[Description("This will test when more than one Text Extractor Profile record is present in the Manager and Worker queue tables.")]
		public void TextExtractorProfile_PreSave_Record_Found_In_Manager_And_Worker_Queue()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(10)
				.Returns(15);

			TextExtractorProfileJob = new TextExtractorProfileJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorProfileJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.EXTRACTION_PROFILE_RECORD_DETECTED));
		}

		[Test]
        //[ReportingTest("81F235BD-6E3E-410C-8A87-159F9EE90131")]
		[Description("This will test when an Exception is thrown.")]
		public void TextExtractorProfile_PreSave_Exception_SQLQueryHelper()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorProfileCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Throws(new Exception());

			TextExtractorProfileJob = new TextExtractorProfileJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorProfileJob.ExecutePreSave();

			//Assert
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.DEFAULT_ERROR_PREPEND));
		}
	}
}
