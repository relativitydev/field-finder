using System;
using System.Runtime.InteropServices;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using TextExtractor.Helpers;
using System.Collections.Generic;
using System.Linq;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.EventHandlers.ExtractorTargetText
{
	[Description("This Event Handler will verify field settings prior to saving the current Extractor Target Text record.")]
	[Guid("1BCB2E8C-5B18-4258-9157-9818938DF2BA")]
	public class TextExtractorTargetTextPreSave : PreSaveEventHandler
	{
		public override Response Execute()
		{
			var response = new Response() { Message = string.Empty, Success = true };
			var layoutArtifactIdByGuid = GetArtifactIdByGuid(Constant.Guids.Layout.TargetText);
			var layoutArtifactId = ActiveLayout.ArtifactID;
			var occurenceFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.Occurrence)].Value.Value;
			var occurence = occurenceFieldValue == null ? (int?) null : Convert.ToInt32(occurenceFieldValue);
			var charactersFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.NumberofCharacters)].Value.Value;
			var characters = charactersFieldValue == null ? (int?)null : Convert.ToInt32(charactersFieldValue);

			var maxExtractionsFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.MaximumExtractions)].Value.Value;
			var minExtractionsFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.MinimumExtractions)].Value.Value;
			var markerTypeFieldValue = (kCura.EventHandler.ChoiceCollection)ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.MarkerType)].Value.Value;
			var caseSensitiveFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.CaseSensitive)].Value.Value;
			var directionFieldValue = (kCura.EventHandler.ChoiceCollection)ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.Direction)].Value.Value;
			var applyStopMarkerFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.ApplyStopMarker)].Value.Value;
			var regExStartMarkerFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStartMarker)].Value.Value;
			var regExStopMarkerFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStopMarker)].Value.Value;
			var plainTextStartMarkerFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.PlainTextStartMarker)].Value.Value;
			var plainTextStopMarkerFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.PlainTextStopMarker)].Value.Value;


			int? maxExtractions = (maxExtractionsFieldValue == null) ? (int?)null : Convert.ToInt32(maxExtractionsFieldValue);
			int? minExtractions = (minExtractionsFieldValue == null) ? (int?)null : Convert.ToInt32(minExtractionsFieldValue);
				
			String markerType = null;
			foreach (kCura.EventHandler.Choice markerChoice in markerTypeFieldValue)
			{
				if (markerChoice.IsSelected)
				{
					markerType = markerChoice.Name;
					break;
				}
			}

			bool? caseSensitive = (caseSensitiveFieldValue == null) ? (bool?)null : Convert.ToBoolean(caseSensitiveFieldValue);
			String direction = null;
			foreach (kCura.EventHandler.Choice directionChoice in directionFieldValue)
			{
				if (directionChoice.IsSelected)
				{
					direction = directionChoice.Name;
					break;
				}
			}
			var applyStopMarker = Convert.ToBoolean(applyStopMarkerFieldValue);

			var regExStart = (regExStartMarkerFieldValue == null) ? (int?)null : Convert.ToInt32(regExStartMarkerFieldValue);
			var regExStop = (regExStopMarkerFieldValue == null) ? (int?)null : Convert.ToInt32(regExStopMarkerFieldValue);
			String plainTextStart = (plainTextStartMarkerFieldValue == null) ? null : Convert.ToString(plainTextStartMarkerFieldValue);
			String plainTextStop = (plainTextStopMarkerFieldValue == null) ? null : Convert.ToString(plainTextStopMarkerFieldValue);

			var validator = new Validator();
			
			//check if this is the Text Extractor Target Text layout
			if (!validator.VerifyIfNotLayout(layoutArtifactIdByGuid, layoutArtifactId))
			{
				var sqlQueryHelper = new SqlQueryHelper();
				var eddsDbContext = Helper.GetDBContext(-1);
				var workspaceArtifactId = Helper.GetActiveCaseID();
				var dbContext = Helper.GetDBContext(workspaceArtifactId);
				var activeArtifactId = ActiveArtifact.ArtifactID;
				var textExtractorTargetTextJob = new TextExtractorTargetTextJob(eddsDbContext, dbContext, sqlQueryHelper, activeArtifactId, characters, occurence, maxExtractions,
					minExtractions, markerType, caseSensitive, direction, applyStopMarker, regExStart, regExStop, plainTextStart, plainTextStop);

				ClearUnnecessaryFields(textExtractorTargetTextJob);
				
				response = textExtractorTargetTextJob.ExecutePreSave();
			}

			return response;
		}

		private void ClearUnnecessaryFields(TextExtractorTargetTextJob textExtractorTargetTextJob)
		{
			switch (textExtractorTargetTextJob.SelectedMarkerType)
			{
				case Constant.Choices.MarkerType.REGULAR_EXPRESSION:
					ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.PlainTextStartMarker)].Value.Value = null;
					ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.PlainTextStopMarker)].Value.Value = null;
					ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.CaseSensitive)].Value.Value = null;
					if (textExtractorTargetTextJob.ApplyStopMarker == false)
					{
						ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStopMarker)].Value.Value = null;
					}
					else
					{
						ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.Direction)].Value.Value = new kCura.EventHandler.ChoiceCollection();
					}
					break;
				case Constant.Choices.MarkerType.PLAIN_TEXT:
					ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStartMarker)].Value.Value = null;
					ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStopMarker)].Value.Value = null;
					if (textExtractorTargetTextJob.ApplyStopMarker == false)
					{
						ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.PlainTextStopMarker)].Value.Value = null;
					}
					else
					{
						ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.Direction)].Value.Value = new kCura.EventHandler.ChoiceCollection();
					}
					break;
			}
		}

		public override FieldCollection RequiredFields
		{
			get
			{
				FieldCollection retVal = new FieldCollection();

				return retVal;
			}
		}
	}
}
