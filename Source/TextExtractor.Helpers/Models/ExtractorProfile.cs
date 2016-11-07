using System;
using System.Collections.Generic;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.ModelFactory;

namespace TextExtractor.Helpers.Models
{
	public class ExtractorProfile
	{
		public String ProfileName { get; private set; }
		public Int32 ArtifactId { get; private set; }
		public List<IExtractorTargetText> ExtractorTargetTexts { get; private set; }
		public Int32 NumberOfTargetTextsWithValues { get; private set; }
		public Int32 FieldCount
		{
			get
			{
				if (ExtractorTargetTexts == null)
				{
					return 0;
				}

				return ExtractorTargetTexts.Count;
			}
		}

		private readonly IArtifactFactory ArtifactFactory;
		private readonly Int32 WorkspaceArtifactId;
		private ErrorLogModel ErrorLogModel { get; set; }

		public ExtractorProfile(IArtifactFactory artifactFactory, Int32 workspaceArtifactId, RDO extractorProfileRdo, ErrorLogModel errorLogModel)
		{
			WorkspaceArtifactId = workspaceArtifactId;
			ArtifactFactory = artifactFactory;
			ExtractorTargetTexts = new List<IExtractorTargetText>();
			ErrorLogModel = errorLogModel;

			SetMyProperties(extractorProfileRdo);
		}

		public void ProcessAllTargetTexts(ExtractorSetDocument textExtractorDocument, ExtractorSet extractorSet)
		{
			if (ExtractorTargetTexts.Count > 0)
			{
				foreach (var currentExtractorTargetText in ExtractorTargetTexts)
				{
					var extractorTargetText = (ExtractorTargetText)currentExtractorTargetText;

					//check for ExtractorSet cancellation
					if (extractorSet.IsCancellationRequested()) { return; }

					try
					{
						var valueUpdated = extractorTargetText.Process(textExtractorDocument, extractorSet);

						IncrementValuesUpdated(valueUpdated);
					}
					catch (Exception ex)
					{
						var errorContext = string.Format("An error occured when processing field for ExtractorTargetText [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}, ExtractorTargetText_ArtifactId: {2}, ExtractorTargetText_TargetName: {3}]", WorkspaceArtifactId, textExtractorDocument.ArtifactId, extractorTargetText.ArtifactId, extractorTargetText.TargetName);

						//log error message to ErrorLog table
						ErrorLogModel.InsertRecord(errorContext, ex, ArtifactId, WorkspaceArtifactId);
					}
				}
			}
		}

		public void ResetNumberOfTargetTextsWithValues()
		{
			NumberOfTargetTextsWithValues = 0;
		}

		private void IncrementValuesUpdated(Boolean targetUpdated)
		{
			if (targetUpdated)
			{
				NumberOfTargetTextsWithValues++;
			}
		}

		private void SetMyProperties(RDO extractorProfileRdo)
		{
			try
			{
				ProfileName = extractorProfileRdo.Fields.Get(Constant.Guids.Fields.ExtractorProfile.ProfileName).ValueAsFixedLengthText;
				if (ProfileName == null)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.EXTRACTOR_PROFILE_PROFILE_NAME_IS_EMPTY);
				}

				ArtifactId = extractorProfileRdo.ArtifactID;
				if (ArtifactId < 1)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.EXTRACTOR_PROFILE_ARTIFACT_ID_CANNOT_BE_LESS_THAN_OR_EQUAL_TO_ZERO);
				}

				var targetTextArtifacts = extractorProfileRdo.Fields.Get(Constant.Guids.Fields.ExtractorProfile.TargetText).GetValueAsMultipleObject<Artifact>();
				if (targetTextArtifacts == null || targetTextArtifacts.Count == 0)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.EXTRACTOR_PROFILE_TARGET_TEXT_IS_EMPTY);
				}

				foreach (var targetTextArtifact in targetTextArtifacts)
				{
					var targetText = ArtifactFactory.GetInstanceOfExtractorTargetText(ExecutionIdentity.CurrentUser, WorkspaceArtifactId, targetTextArtifact.ArtifactID);

					ExtractorTargetTexts.Add(targetText);
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.DEFAULT_CONVERT_TO_EXTRACTOR_PROFILE_ERROR_MESSAGE + ". " + ExceptionMessageFormatter.GetInnerMostExceptionMessage(ex), ex);
			}
		}
	}
}