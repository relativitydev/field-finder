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
