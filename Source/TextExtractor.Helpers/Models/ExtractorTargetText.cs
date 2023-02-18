using System;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Rsapi;
using TextExtractor.Helpers.ModelFactory;
using System.ComponentModel;

namespace TextExtractor.Helpers.Models
{
	public class ExtractorTargetText : IExtractorTargetText
	{
		public String TargetName { get; private set; }
		public Int32 ArtifactId { get; private set; }
		public String StartMarker { get; private set; }
        public String StopMarker { get; private set; }
		public ITargetRule TargetRule { get; private set; }
		public Artifact DestinationField { get; private set; }
		public Boolean UpdatedValue { get; private set; }

		public Artifact RegeExStartMarkerArtifact { get; private set; }
		public Artifact RegeExStopMarkerArtifact { get; private set; }
		public ExtractorRegularExpression RegExStartMarker {get; private set;}
		public String PlainTextStartMarker { get; private set; }
		public ExtractorRegularExpression RegExStopMarker { get; private set; }
		public String PlainTextStopMarker { get; private set; }

		private readonly ExecutionIdentity ExecutionIdentity;
		private readonly Int32 WorkspaceArtifactId;
		private readonly IServicesMgr ServicesMgr;
		private readonly IArtifactQueries ArtifactQueries;
		private readonly ITextExtractionUtility TextExtractionUtility;
		private readonly ErrorLogModel ErrorLogModel;


		/// <summary>
		/// For Testing Only
		/// </summary>
		public ExtractorTargetText() { }

		public ExtractorTargetText(IArtifactQueries artifactQueries, IServicesMgr servicesMgr, ExecutionIdentity executionIdentity, Int32 workspaceArtifactId, RDO extractorTargetTextRdo, ErrorLogModel errorLogModel)
		{
			//ErrorQueue = errorQueue;
			ServicesMgr = servicesMgr;
			ExecutionIdentity = executionIdentity;
			WorkspaceArtifactId = workspaceArtifactId;
			ArtifactQueries = artifactQueries;
			TextExtractionUtility = new TextExtractionUtility(new TextExtractionValidator());
			TargetRule = new TargetRule(extractorTargetTextRdo);
			ErrorLogModel = errorLogModel;

			SetMyProperties(extractorTargetTextRdo);
		}

		// Virtual for testing purpose
		public virtual Boolean Process(ExtractorSetDocument extractorSetDocument, ExtractorSet extractorSet)
		{
			if (extractorSetDocument == null)
			{
				throw new ArgumentNullException("extractorSetDocument");
			}
			if (extractorSetDocument.TextSource == null)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.EXTRACTOR_SET_SOURCE_LONG_TEXT_FIELD_IS_EMPTY);
			}

			var documentUpdatedWithExtractedText = false;

			ExtractorSetHistory extractorSetHistory = null;

			var errorContext = String.Format("An error occured when extracting text for field. [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}, TextExtractorFieldArtifactId: {2}]", WorkspaceArtifactId, extractorSetDocument.ArtifactId, ArtifactId);

			try
			{
				string historyStartMarkerName = null;
				string historyStopMarkerName = null;
				string historyMarkerType = null;

				switch (TargetRule.MarkerEnum)
				{
					case Constant.MarkerEnum.RegEx:
						historyStartMarkerName = RegExStartMarker.Name;
						historyStopMarkerName = (RegExStopMarker == null) ? null : RegExStopMarker.Name;
						historyMarkerType = "Regular Expression";
						break;

					case Constant.MarkerEnum.PlainText:
						historyStartMarkerName = StartMarker;
						historyStopMarkerName = StopMarker;
						historyMarkerType = "Plain Text";
						break;
				}

				extractorSetHistory = new ExtractorSetHistory(ServicesMgr, ExecutionIdentity, ArtifactQueries, extractorSet.ArtifactId, extractorSetDocument.ArtifactId, DestinationField.ArtifactID, WorkspaceArtifactId, TargetName, historyStartMarkerName, historyStopMarkerName, historyMarkerType);
                
                if (!String.IsNullOrEmpty(this.StopMarker))
                {
                    TextExtractionUtility.StopMarker = this.StopMarker;
                }

				var extractedText = TextExtractionUtility.ExtractText(extractorSetDocument.TextSource, StartMarker, StopMarker, TargetRule);

				if (TextExtractionUtility.IsMarkerFound == false)
				{
					//create extractor set history record
					extractorSetHistory.CreateRecord(Constant.ExtractionSetHistoryStatus.COMPLETE_MARKER_NOT_FOUND);
				}
				else
				{
					if (String.IsNullOrEmpty(extractedText))
					{
						//create extractor set history record
						extractorSetHistory.CreateRecord(Constant.ExtractionSetHistoryStatus.COMPLETE_TEXT_NOT_FOUND);
					}
					else
					{
						//update Document field with extracted text
						ArtifactQueries.UpdateDocumentTextFieldValue(ServicesMgr, ExecutionIdentity.CurrentUser, WorkspaceArtifactId, extractorSetDocument.ArtifactId, DestinationField.ArtifactID, extractedText);

						//check if text is truncated
						if (TextExtractionUtility.IsTextTruncated)
						{
							//Update TextExtractorDetails field on the Document object if extracted text is truncated
							var fieldValue = String.Format(Constant.TextExtractorDetailsMessages.TRUNCATED, ArtifactQueries.GetFieldNameForArtifactId(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, DestinationField.ArtifactID));

							ArtifactQueries.AppendToDocumentLongTextFieldValue(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, extractorSetDocument.ArtifactId, Constant.Guids.Fields.Document.TextExtractorDetails, fieldValue);
						}

						//create extractor set history record
						extractorSetHistory.CreateRecord(Constant.ExtractionSetHistoryStatus.COMPLETE_TEXT_EXTRACTED);
						documentUpdatedWithExtractedText = true;
					}
				}
			}
			catch (Exception ex)
			{
				//create extractor set history record
				if (extractorSetHistory != null)
				{
					extractorSetHistory.CreateRecord(Constant.ExtractionSetHistoryStatus.ERROR, ExceptionMessageFormatter.GetInnerMostExceptionMessage(ex));
				}
				else
				{
					throw new Exception("An error occured when creating Extractor Set History record.");
				}

				//log error message to ErrorLog table
				ErrorLogModel.InsertRecord(errorContext, ex, ArtifactId, WorkspaceArtifactId);
			}

			return documentUpdatedWithExtractedText;
		}

		public void SetMyProperties(RDO extractorTargetTextRdo)
		{
			try
			{
				ArtifactId = extractorTargetTextRdo.ArtifactID;
				if (ArtifactId < 1)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.TARGET_TEXT_ARTIFACT_ID_CANNOT_BE_LESS_THAN_OR_EQUAL_TO_ZERO);
				}

				TargetName = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.TargetName).ValueAsFixedLengthText;
				if (TargetName == null)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.TARGET_TEXT_TARGET_NAME_IS_EMPTY);
				}

				switch (TargetRule.MarkerEnum)
				{
					case Constant.MarkerEnum.RegEx:
						RegeExStartMarkerArtifact = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStartMarker).ValueAsSingleObject;
						if (RegeExStartMarkerArtifact == null)
						{
							throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.TARGET_TEXT_REGULAR_EXPRESSION_IS_EMPTY);
						}

						ArtifactFactory artifactFactory = new ArtifactFactory(ArtifactQueries, ServicesMgr, ErrorLogModel);
						RegExStartMarker = artifactFactory.GetInstanceOfExtractorRegularExpression(ExecutionIdentity.CurrentUser, WorkspaceArtifactId, RegeExStartMarkerArtifact.ArtifactID);

						StartMarker = RegExStartMarker.RegularExpression;

						RegeExStopMarkerArtifact = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStopMarker).ValueAsSingleObject;
						if (RegeExStopMarkerArtifact == null || RegeExStopMarkerArtifact.ArtifactID < 1)
						{
							RegExStopMarker = null;
						}
						else
						{
							RegExStopMarker = artifactFactory.GetInstanceOfExtractorRegularExpression(ExecutionIdentity.CurrentUser, WorkspaceArtifactId, RegeExStopMarkerArtifact.ArtifactID);

							StopMarker = RegExStopMarker.RegularExpression;
						}
						break;

					case Constant.MarkerEnum.PlainText:
						PlainTextStartMarker = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.PlainTextStartMarker).ValueAsFixedLengthText;
						if (PlainTextStartMarker == null)
						{
							throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.TARGET_TEXT_PLAIN_TEXT_MARKER_IS_EMPTY);
						}

						StartMarker = PlainTextStartMarker;

						PlainTextStopMarker = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.PlainTextStopMarker).ValueAsFixedLengthText;
						StopMarker = PlainTextStopMarker;
						break;
				}

				if (StartMarker == null)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.TARGET_TEXT_START_MARKER_IS_EMPTY);
				}

				if (StopMarker != null)
				{
					TargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
				}

				DestinationField = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.DestinationField).ValueAsSingleObject;
				if (DestinationField == null)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.TARGET_TEXT_DESTINATION_FIELD_IS_EMPTY);
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.DEFAULT_CONVERT_TO_EXTRACTOR_TARGET_TEXT_ERROR_MESSAGE + ". " + ExceptionMessageFormatter.GetInnerMostExceptionMessage(ex), ex);
			}
		}
	}
}