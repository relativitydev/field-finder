using System;
using System.Data;
using Moq;
using NUnit.Framework;
using Relativity.API;
using TextExtractor.EventHandlers.ExtractorSet;
using TextExtractor.EventHandlers.Interfaces;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Rsapi;
using TextExtractor.TestHelpers;

namespace TextExtractor.EventHandlers.NUnit.Tests
{
	[TestFixture]
	[Category(TestCategory.INTEGRATION)]
	[Category(TestCategory.UNIT)]
	public class ManageJobConsoleTests
	{
		public IConsoleJob Sut { get; set; }

		#region constants

		private const int WORKSPACE_ARTIFACT_ID = 1000001;
		private const int ACTIVE_JOB_ARTIFACT_ID = 1000010;
		private const int SAVED_SEARCH_ARTIFACT_ID = 1000020;
		private const int PROFILE_ARTIFACT_ID = 1000030;
		private const int TEXT_FIELD_ARTIFACT_ID = 1000040;
		private const string BUTTON_NAME_SUBMIT = "submit";
		private const string BUTTON_NAME_CANCEL = "cancel";
		private const string ERRONEOUS_BUTTON_NAME_CANCEL = "XYZ";
		private const ExecutionIdentity EXECUTION_CURRENT_USER_IDENTITY = ExecutionIdentity.CurrentUser;

		private Mock<IServicesMgr> _mockServiceManager;
		private Mock<IDBContext> _mockEddsDbContext;
		private Mock<IArtifactQueries> _mockArtifactQueries;
		private Mock<ISqlQueryHelper> _mockSqlQueryHelper;

		#endregion

		[SetUp]
		public void Setup()
		{
			_mockServiceManager = new Mock<IServicesMgr>();
			_mockEddsDbContext = new Mock<IDBContext>();
			_mockArtifactQueries = new Mock<IArtifactQueries>();
			_mockSqlQueryHelper = new Mock<ISqlQueryHelper>();
		}

		[TearDown]
		public void TearDown()
		{
			_mockServiceManager = null;
			_mockEddsDbContext = null;
			_mockArtifactQueries = null;
			_mockSqlQueryHelper = null;
			Sut = null;
		}

		[Test]
		[Description("This will test the Golden Flow when the Submit button is clicked (Golden Path).")]
		public void ManageJobConsole_Submit_Golden_Flow()
		{
			//Arrange
			_mockSqlQueryHelper
				.Setup(x => x.RetrieveSingleInManagerQueueByArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns<DataRow>(null);

			_mockArtifactQueries.Setup(
				x =>
					x.UpdateRdoStringFieldValue(It.IsAny<IServicesMgr>(), It.IsAny<ExecutionIdentity>(), It.IsAny<int>(),
						It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

			_mockSqlQueryHelper.Setup(
				x =>
					x.InsertRowIntoManagerQueue(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
						It.IsAny<int>(), It.IsAny<int>()));

			Sut = new ConsoleJob(_mockServiceManager.Object, _mockArtifactQueries.Object, _mockSqlQueryHelper.Object, EXECUTION_CURRENT_USER_IDENTITY, _mockEddsDbContext.Object, WORKSPACE_ARTIFACT_ID, ACTIVE_JOB_ARTIFACT_ID, SAVED_SEARCH_ARTIFACT_ID, PROFILE_ARTIFACT_ID, TEXT_FIELD_ARTIFACT_ID, BUTTON_NAME_SUBMIT);

			//Act
			Sut.Execute();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveSingleInManagerQueueByArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>()));
			_mockArtifactQueries.Verify(x => x.UpdateRdoStringFieldValue(It.IsAny<IServicesMgr>(), It.IsAny<ExecutionIdentity>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));
			_mockSqlQueryHelper.Verify(x => x.InsertRowIntoManagerQueue(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
		}

		[Test]
		[Description("This will test the Golden Flow when the Cancel button is clicked (Golden Path).")]
		public void ManageJobConsole_Cancel_Golden_Flow()
		{
			//Arrange
			_mockArtifactQueries
				.Setup(x => x.GetExtractorSetStatus(It.IsAny<IServicesMgr>(), It.IsAny<ExecutionIdentity>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(Constant.ExtractorSetStatus.IN_PROGRESS_MANAGER_PROCESSING);

			_mockArtifactQueries.Setup(
				x =>
					x.UpdateRdoStringFieldValue(It.IsAny<IServicesMgr>(), It.IsAny<ExecutionIdentity>(), It.IsAny<int>(),
						It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

			Sut = new ConsoleJob(_mockServiceManager.Object, _mockArtifactQueries.Object, _mockSqlQueryHelper.Object, EXECUTION_CURRENT_USER_IDENTITY, _mockEddsDbContext.Object, WORKSPACE_ARTIFACT_ID, ACTIVE_JOB_ARTIFACT_ID, SAVED_SEARCH_ARTIFACT_ID, PROFILE_ARTIFACT_ID, TEXT_FIELD_ARTIFACT_ID, BUTTON_NAME_CANCEL);

			//Act
			Sut.Execute();

			//Assert
			_mockArtifactQueries.Verify(x => x.UpdateRdoStringFieldValue(It.IsAny<IServicesMgr>(), It.IsAny<ExecutionIdentity>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));
		}

		[Test]
		[Description("This will test when the Cancel button is clicked (Golden Path) and the status of the Extractor Set is Complete.")]
		public void ManageJobConsole_Cancel_Set_Complete()
		{
			//Arrange
			_mockArtifactQueries
				.Setup(x => x.GetExtractorSetStatus(It.IsAny<IServicesMgr>(), It.IsAny<ExecutionIdentity>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(Constant.ExtractorSetStatus.COMPLETE).Verifiable();

			Sut = new ConsoleJob(_mockServiceManager.Object, _mockArtifactQueries.Object, _mockSqlQueryHelper.Object, EXECUTION_CURRENT_USER_IDENTITY, _mockEddsDbContext.Object, WORKSPACE_ARTIFACT_ID, ACTIVE_JOB_ARTIFACT_ID, SAVED_SEARCH_ARTIFACT_ID, PROFILE_ARTIFACT_ID, TEXT_FIELD_ARTIFACT_ID, BUTTON_NAME_CANCEL);

			//Assert
			var exception = Assert.Throws<CustomExceptions.TextExtractorSetConsoleCancelException>(() => Sut.Execute());
			Assert.That(exception.Message, Is.StringContaining(Constant.ErrorMessages.EXTRACTION_SET_RECORD_COMPLETE));
		}

		[Test]
		[Description("This will test when the Submit button is clicked and a record already exists on the Manager queue table.")]
		public void ManageJobConsole_Submit_With_Manager_Record()
		{
			//Arrange
			var dataTable = new DataTable();
			dataTable.Columns.Add("ID", typeof(int));
			dataTable.Columns.Add("Added On", typeof(DateTime));
			dataTable.Columns.Add("Workspace Artifact ID", typeof(int));
			dataTable.Columns.Add("Workspace Name", typeof(string));
			dataTable.Columns.Add("Status", typeof(string));
			dataTable.Columns.Add("Agent Artifact ID", typeof(int));
			dataTable.Columns.Add("Job ID", typeof(int));

			var dataRow = dataTable.NewRow();
			dataRow["ID"] = 1;
			dataRow["Added On"] = "10/1/2015 10:31:00 AM";
			dataRow["Workspace Artifact ID"] = 1000001;
			dataRow["Workspace Name"] = "Test Workspace";
			dataRow["Status"] = "New";
			dataRow["Agent Artifact ID"] = 1000010;
			dataRow["Job ID"] = 111;

			_mockSqlQueryHelper
				.Setup(x => x.RetrieveSingleInManagerQueueByArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(dataRow);

			Sut = new ConsoleJob(_mockServiceManager.Object, _mockArtifactQueries.Object, _mockSqlQueryHelper.Object, EXECUTION_CURRENT_USER_IDENTITY, _mockEddsDbContext.Object, WORKSPACE_ARTIFACT_ID, ACTIVE_JOB_ARTIFACT_ID, SAVED_SEARCH_ARTIFACT_ID, PROFILE_ARTIFACT_ID, TEXT_FIELD_ARTIFACT_ID, BUTTON_NAME_SUBMIT);

			//Act
			Sut.Execute();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveSingleInManagerQueueByArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>()));
			_mockArtifactQueries.Verify(x => x.UpdateRdoStringFieldValue(It.IsAny<IServicesMgr>(), It.IsAny<ExecutionIdentity>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
			_mockSqlQueryHelper.Verify(x => x.InsertRowIntoManagerQueue(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		[Description("This will test when a button with a name other than Submit or Cancel is clicked.")]
		public void ManageJobConsole_Erroneous_Button_Golden_Flow()
		{
			//Arrange
			Sut = new ConsoleJob(_mockServiceManager.Object, _mockArtifactQueries.Object, _mockSqlQueryHelper.Object, EXECUTION_CURRENT_USER_IDENTITY, _mockEddsDbContext.Object, WORKSPACE_ARTIFACT_ID, ACTIVE_JOB_ARTIFACT_ID, SAVED_SEARCH_ARTIFACT_ID, PROFILE_ARTIFACT_ID, TEXT_FIELD_ARTIFACT_ID, ERRONEOUS_BUTTON_NAME_CANCEL);

			//Act
			Sut.Execute();

			//Assert
			_mockSqlQueryHelper.Verify(x => x.RetrieveSingleInManagerQueueByArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			_mockArtifactQueries.Verify(x => x.UpdateRdoStringFieldValue(It.IsAny<IServicesMgr>(), It.IsAny<ExecutionIdentity>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
			_mockSqlQueryHelper.Verify(x => x.InsertRowIntoManagerQueue(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		[Description("This will test when an Exception is thrown.")]
		public void ManageJobConsole_Exception_SQLQueryHelper()
		{
			//Arrange
			Sut = new ConsoleJob(null, null, null, EXECUTION_CURRENT_USER_IDENTITY, _mockEddsDbContext.Object, WORKSPACE_ARTIFACT_ID, ACTIVE_JOB_ARTIFACT_ID, SAVED_SEARCH_ARTIFACT_ID, PROFILE_ARTIFACT_ID, TEXT_FIELD_ARTIFACT_ID, BUTTON_NAME_SUBMIT);

			//Assert
			Assert.Throws<Exception>(() => Sut.Execute());
		}

		[Test]
        [Description("This will test when an Exception is thrown.")]
		public void ManageJobConsole_EXCEPTION()
		{
			//Arrange
			_mockSqlQueryHelper
					.Setup(x => x.RetrieveSingleInManagerQueueByArtifactId(It.IsAny<IDBContext>(), It.IsAny<int>(), It.IsAny<int>()))
					.Throws<Exception>();

			Sut = new ConsoleJob(_mockServiceManager.Object, _mockArtifactQueries.Object, _mockSqlQueryHelper.Object, EXECUTION_CURRENT_USER_IDENTITY, _mockEddsDbContext.Object, WORKSPACE_ARTIFACT_ID, ACTIVE_JOB_ARTIFACT_ID, SAVED_SEARCH_ARTIFACT_ID, PROFILE_ARTIFACT_ID, TEXT_FIELD_ARTIFACT_ID, BUTTON_NAME_SUBMIT);

			//Assert          
			Assert.Throws<Exception>(() => Sut.Execute());
		}

	}
}
