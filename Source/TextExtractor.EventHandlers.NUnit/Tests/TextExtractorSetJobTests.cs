﻿using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Relativity.API;
using TextExtractor.EventHandlers.ExtractorSet;
using TextExtractor.EventHandlers.Interfaces;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.TestHelpers;

namespace TextExtractor.EventHandlers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.UNIT)]
	public class TextExtractorSetJobTests
	{
		public ITextExtractorSetJob TextExtractorSetJob { get; set; }

		#region constants

		private const int ACTIVE_JOB_ARTIFACT_ID = 1000040;
		private Mock<IDBContext> _mockDbContext;
		private Mock<IDBContext> _mockEddsDbContext;
		private Mock<ISqlQueryHelper> _mockSqlQueryHelper;

		#endregion

		[SetUp]
		public void Setup()
		{
			_mockDbContext = new Mock<IDBContext>();
			_mockEddsDbContext = new Mock<IDBContext>();
			_mockSqlQueryHelper = new Mock<ISqlQueryHelper>();
		}

		[TearDown]
		public void TearDown()
		{
			_mockDbContext = null;
			_mockEddsDbContext = null;
			_mockSqlQueryHelper = null;
			TextExtractorSetJob = null;
		}

		[Test]
        //[ReportingTest("3CB2FF85-B3B9-4E44-8BC9-6B546533233D")]
		[Description("This will test the Golden Flow when no Text Extractor Sets are present in the Manager and Worker queue tables.")]
		public void TextExtractorSet_PreSave_Golden_Flow()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveJobCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(0);

			TextExtractorSetJob = new TextExtractorSetJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorSetJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveJobCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, true);
			Assert.AreEqual(response.Message, string.Empty);
		}

		[Test]
        //[ReportingTest("D77120E8-C449-49F0-89F1-739902888E29")]
		[Description("This will test when one Text Extractor Set record is present in the Manager queue tables.")]
		public void TextExtractorSet_PreSave_Record_Found_In_Manager_Queue()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveJobCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(1)
				.Returns(0);

			TextExtractorSetJob = new TextExtractorSetJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorSetJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveJobCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.EXTRACTION_SET_RECORD_DETECTED));
		}

		[Test]
        //[ReportingTest("2F6B38C8-F5D9-44B8-8777-C14494981BA8")]
		[Description("This will test when one Text Extractor Set record is present in the Worker queue tables.")]
		public void TextExtractorSet_PreSave_Record_Found_In_Worker_Queue()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveJobCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(0)
				.Returns(1);

			TextExtractorSetJob = new TextExtractorSetJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorSetJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveJobCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.EXTRACTION_SET_RECORD_DETECTED));
		}
	
		[Test]
        //[ReportingTest("4646C406-0B1C-410D-9508-B05C4F0E4ED5")]
		[Description("This will test when more than one Text Extractor Set record is present in the Manager and Worker queue tables.")]
		public void TextExtractorSet_PreSave_Record_Found_In_Manager_And_Worker_Queue()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveJobCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(10)
				.Returns(15);

			TextExtractorSetJob = new TextExtractorSetJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorSetJob.ExecutePreSave();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveJobCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.EXTRACTION_SET_RECORD_DETECTED));
		}

		[Test]
        //[ReportingTest("226A3FBD-EB8E-447F-AE1E-111B684F3850")]
		[Description("This will test when an Exception is thrown.")]
		public void TextExtractorSet_PreSave_Exception_SQLQueryHelper()
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveJobCountInQueue(It.IsAny<IDBContext>(), It.IsAny<string>(), It.IsAny<string>()))
				.Throws(new Exception());

			TextExtractorSetJob = new TextExtractorSetJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorSetJob.ExecutePreSave();

			//Assert
			Assert.AreEqual(response.Success, false);
			Assert.That(response.Message, Is.StringContaining(Constant.ErrorMessages.DEFAULT_ERROR_PREPEND));
		}

		[Test]
        //[ReportingTest("7EDE59AB-19AD-4482-82F9-8B68B974D845")]
		[Description("This will test the Golden Flow when the status of the current Text Extractor Set record is null and the user is trying to delete the record.")]
		public void TextExtractorSet_PreDelete_Golden_Flow()
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorSetStatusBySetArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>()))
				.Returns<string>(null);

			TextExtractorSetJob = new TextExtractorSetJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorSetJob.ExecutePreDelete();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorSetStatusBySetArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>()));
			Assert.AreEqual(response.Success, true);
			Assert.AreEqual(response.Message, string.Empty);
		}

		[Test]
        //[ReportingTest("7EDE59AB-19AD-4482-82F9-8B68B974D845")]
		[Description("This will test the Golden Flow when the status of the current Text Extractor Set record is null and the user is trying to delete the record.")]
		public void TextExtractorSet_PreDelete_One_Set_With_Non_Null_Status()
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorSetStatusBySetArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>()))
				.Returns("Submitted");

			TextExtractorSetJob = new TextExtractorSetJob(_mockDbContext.Object, _mockSqlQueryHelper.Object, ACTIVE_JOB_ARTIFACT_ID);

			//Act
			var response = TextExtractorSetJob.ExecutePreDelete();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorSetStatusBySetArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>()));
			Assert.AreEqual(response.Success, false);
			Assert.IsInstanceOf<SystemException>(response.Exception); 
			Assert.That(response.Exception.Message, Is.StringContaining(Constant.ErrorMessages.EXTRACTION_SET_CANNOT_DELETE_STATUS_NOT_NULL));
		}
		
		[Test]
        //[ReportingTest("8C253C7E-1AD6-4C71-BEA0-F9B48182001A")]
		[Description("This will test the Golden Flow when the status of the all the selected Text Extractor Set records are null and the user is trying to mass delete the records.")]
		public void TextExtractorSet_PreMassDelete_Golden_Flow()
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveExtractorSetStatusCountByTempTable(It.IsAny<IDBContext>(), It.IsAny<string>()))
				.Returns(0);

			TextExtractorSetJob = new TextExtractorSetJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, Constant.Tables.ExtractorSetHistory);

			//Act
			var response = TextExtractorSetJob.ExecutePreMassDelete();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorSetStatusCountByTempTable(It.IsAny<IDBContext>(), It.IsAny<string>()));
			Assert.AreEqual(response.Success, true);
			Assert.AreEqual(response.Message, string.Empty);
		}

		[TestCase(1)]
		[TestCase(100)]
		[Test]
        //[ReportingTest("76F67EAB-B10C-4B1A-A330-5155FDAAA4FE")]
		[Description("This will test when the multiple Set Statuses for some of the selected Text Extractor Set records are set and the user is trying to mass delete the records.")]
		public void TextExtractorSet_PreMassDelete_Set_Statuses_Exist(int setRecords)
		{
			//Arrange
			_mockSqlQueryHelper
				.SetupSequence(x => x.RetrieveExtractorSetStatusCountByTempTable(It.IsAny<IDBContext>(), It.IsAny<string>()))
				.Returns(setRecords)
				.Returns(setRecords);

			TextExtractorSetJob = new TextExtractorSetJob(_mockEddsDbContext.Object, _mockDbContext.Object, _mockSqlQueryHelper.Object, Constant.Tables.ExtractorSetHistory);

			//Act
			var response = TextExtractorSetJob.ExecutePreMassDelete();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveExtractorSetStatusCountByTempTable(It.IsAny<IDBContext>(), It.IsAny<string>()));
			Assert.AreEqual(response.Success, false);
			Assert.IsInstanceOf<SystemException>(response.Exception);
			Assert.That(response.Exception.ToString(), Is.StringContaining(Constant.ErrorMessages.EXTRACTION_SET_CANNOT_DELETE_MULTIPLE_RECORD));
		}

	}
}