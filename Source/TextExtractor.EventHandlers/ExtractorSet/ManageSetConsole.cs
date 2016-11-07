using System;
using System.Collections.Generic;
using kCura.EventHandler;
using Relativity.API;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Rsapi;
using Console = kCura.EventHandler.Console;

namespace TextExtractor.EventHandlers.ExtractorSet
{
	[kCura.EventHandler.CustomAttributes.Description("This Event Handler allows a user to Submit or Cancel an Extractor Set.")]
	[System.Runtime.InteropServices.Guid("CEAFB746-8B95-4440-9C91-898A6FF55CE3")]
	public class ManageSetConsole : ConsoleEventHandler
	{
		private Console _console;
		private ConsoleButton _submitButton;
		private ConsoleButton _cancelButton;
		private ConsoleSeparator _separator;
		private readonly SqlQueryHelper _sqlQueryHelper = new SqlQueryHelper();
		private readonly ArtifactQueries _artifactQueries = new ArtifactQueries();
		public ExecutionIdentity ExecutionCurrentUserIdentity { get; set; }

		public override Console GetConsole(ConsoleEventHandler.PageEvent pageEvent)
		{
			_console = new Console { Items = new List<IConsoleItem>(), Title = "Manage Set" };

			_submitButton = new ConsoleButton
			{
				Name = Constant.Names.Console.SUBMIT,
				DisplayText = "Submit",
				ToolTip = "Click here to submit this Extractor Set to the queue.",
				RaisesPostBack = true
			};

			_cancelButton = new ConsoleButton
			{
				Name = Constant.Names.Console.CANCEL,
				DisplayText = "Cancel",
				ToolTip = "Click here to cancel this Extractor Set.",
				RaisesPostBack = true
			};

			_separator = new ConsoleSeparator();

			if (pageEvent == PageEvent.PreRender)
			{
				var textExtractorStatus = ActiveArtifact.Fields[Constant.Guids.Fields.ExtractorSet.Status.ToString()].Value.Value;

				if (textExtractorStatus == null)
				{
					_submitButton.Enabled = true;
					_cancelButton.Enabled = false;
				}
				else if (textExtractorStatus.ToString() == Constant.ExtractorSetStatus.CANCELLED || textExtractorStatus.ToString() == Constant.ExtractorSetStatus.COMPLETE || textExtractorStatus.ToString() == Constant.ExtractorSetStatus.COMPLETE_WITH_ERRORS || textExtractorStatus.ToString() == Constant.ExtractorSetStatus.ERROR)
				{
					_submitButton.Enabled = false;
					_cancelButton.Enabled = false;
				}
				else
				{
					_submitButton.Enabled = false;
					_cancelButton.Enabled = true;
				}
			}

			_console.Items.Add(_submitButton);
			_console.Items.Add(_cancelButton);
			_console.Items.Add(_separator);
			_console.AddRefreshLinkToConsole().Enabled = true;

			return _console;
		}

		public override void OnButtonClick(ConsoleButton consoleButton)
		{
			var svcMgr = Helper.GetServicesManager();
			var workspaceArtifactId = Helper.GetActiveCaseID();
			var eddsDbContext = Helper.GetDBContext(-1);
			var activeArtifactId = ActiveArtifact.ArtifactID;
			var savedSearchArtifactId = (int?)ActiveArtifact.Fields[Constant.Guids.Fields.ExtractorSet.SavedSearch.ToString()].Value.Value;
			var extractorProfileArtifactId = (int?)ActiveArtifact.Fields[Constant.Guids.Fields.ExtractorSet.ExtractorProfile.ToString()].Value.Value;
			var sourceLongTextFieldArtifactId = (int?)ActiveArtifact.Fields[Constant.Guids.Fields.ExtractorSet.SourceLongTextField.ToString()].Value.Value;

			ExecutionCurrentUserIdentity = ExecutionIdentity.CurrentUser;
			var buttonName = consoleButton.Name;
			var consoleJob = new ConsoleJob(svcMgr, _artifactQueries, _sqlQueryHelper, ExecutionCurrentUserIdentity, eddsDbContext, workspaceArtifactId, activeArtifactId, savedSearchArtifactId, extractorProfileArtifactId, sourceLongTextFieldArtifactId, buttonName);

			try
			{
				consoleJob.Execute();
			}
			catch (CustomExceptions.TextExtractorSetMissingFieldsException ex)
			{
				throw new Exception(ex.Message);
			}
			catch (CustomExceptions.TextExtractorSetConsoleCancelException ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public override FieldCollection RequiredFields
		{
			get
			{
				var retVal = new FieldCollection();
				retVal.Add(new Field(Constant.Guids.Fields.ExtractorSet.SavedSearch));
				retVal.Add(new Field(Constant.Guids.Fields.ExtractorSet.ExtractorProfile));
				retVal.Add(new Field(Constant.Guids.Fields.ExtractorSet.SourceLongTextField));
				return retVal;
			}
		}
	}
}
