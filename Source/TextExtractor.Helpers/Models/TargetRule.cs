using System;

using kCura.Relativity.Client.DTOs;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.Helpers.Models
{
	public class TargetRule : ITargetRule
	{
        public Constant.MarkerEnum MarkerEnum { get; set; }
		public Constant.DirectionEnum DirectionEnum { get; set; }
		public Int32 CharacterLength { get; set; }
		public Constant.TrimStyleEnum TrimStyleEnum { get; set; }
		public Boolean CaseSensitive { get; set; }
        public Boolean IncludeMarker { get; set; }
		public Int32 Occurrence { get; set; }

		public Int32 MaximumExtractions { get; set; }
		public Int32 MinimumExtractions { get; set; }
		public String CustomDelimiter { get; set; }


		public TargetRule(RDO extractorTargetTextRdo)
		{
			if (extractorTargetTextRdo == null)
			{
				throw new ArgumentNullException("extractorTargetTextRdo");
			}

			SetMyProperties(extractorTargetTextRdo);
		}

		private void SetMyProperties(RDO extractorTargetTextRdo)
		{
			try
			{
				SetDirection(extractorTargetTextRdo);

				SetTrim(extractorTargetTextRdo);

				SetMarkerType(extractorTargetTextRdo);

				bool? includeMarker = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.IncludeMarker).ValueAsYesNo;
				IncludeMarker = (includeMarker != null) ? includeMarker.Value : false;

				int? characterLength = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.NumberofCharacters).ValueAsWholeNumber;
				CharacterLength = (characterLength == null) ? Constant.Sizes.EXTRACTOR_TARGET_TEXT_CHARACTERS_MAXIMUM : Convert.ToInt32(characterLength);

				bool? caseSensitive = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.CaseSensitive).ValueAsYesNo;
				if (caseSensitive == null)
				{
					caseSensitive = false;
				}
				CaseSensitive = Convert.ToBoolean(caseSensitive);

				int? occurrence = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.Occurrence).ValueAsWholeNumber;
				Occurrence = occurrence == null ? Constant.Sizes.DEFAULT_OCCURENCE : Convert.ToInt32(occurrence);

				int? maxNumberOfExtractions = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.MaximumExtractions).ValueAsWholeNumber;
				MaximumExtractions = maxNumberOfExtractions ?? Constant.Sizes.DEFAULT_MAXIMUM_EXTRACTIONS;

				int? minNumberOfExtractions = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.MinimumExtractions).ValueAsWholeNumber;
				MinimumExtractions = minNumberOfExtractions ?? Constant.Sizes.DEFAULT_MINIMUM_EXTRACTIONS;

				string customDelimiter = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.ResultsCustomDelimiter).ValueAsFixedLengthText;
				CustomDelimiter = (String.IsNullOrEmpty(customDelimiter)) ? Constant.Sizes.DEFAULT_DELIMITER : customDelimiter + " ";
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.DEFAULT_CONVERT_TO_EXTRACTOR_TARGET_TEXT_ERROR_MESSAGE, ex);
			}
		}

		private void SetDirection(RDO extractorTargetTextRdo)
		{
			var direction = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.Direction).ValueAsSingleChoice;
			if (direction != null)
			{
				switch (direction.Name)
				{
					case Constant.Choices.Direction.LEFT:
						DirectionEnum = Constant.DirectionEnum.Left;
						break;

					case Constant.Choices.Direction.RIGHT:
						DirectionEnum = Constant.DirectionEnum.Right;
						break;

					case Constant.Choices.Direction.LEFT_AND_RIGHT:
						DirectionEnum = Constant.DirectionEnum.LeftAndRight;
						break;

					default:
						throw new CustomExceptions.TextExtractorException(string.Format("Direction({0}) is not supported", direction.Name));
				}
			}
		}

		private void SetMarkerType(RDO extractorTargetTextRdo)
		{
			var markerType = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.MarkerType).ValueAsSingleChoice;
			if (markerType == null)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.TARGET_TEXT_MARKER_TYPE_IS_EMPTY);
			}

			switch (markerType.Name)
			{
				case Constant.Choices.MarkerType.REGULAR_EXPRESSION:
					MarkerEnum = Constant.MarkerEnum.RegEx;
					break;

				case Constant.Choices.MarkerType.PLAIN_TEXT:
					MarkerEnum = Constant.MarkerEnum.PlainText;
					break;

				default:
					throw new CustomExceptions.TextExtractorException(string.Format("Marker type({0}) is not supported", markerType.Name));
			}
		}

		private void SetTrim(RDO extractorTargetTextRdo)
		{
			var trimStyle = extractorTargetTextRdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.TrimStyle).ValueAsSingleChoice;
			if (trimStyle == null)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.TARGET_TEXT_TRIM_STYLE_IS_EMPTY);
			}

			switch (trimStyle.Name)
			{
				case Constant.Choices.TrimStyle.NONE:
					TrimStyleEnum = Constant.TrimStyleEnum.None;
					break;

				case Constant.Choices.TrimStyle.LEFT:
					TrimStyleEnum = Constant.TrimStyleEnum.Left;
					break;

				case Constant.Choices.TrimStyle.RIGHT:
					TrimStyleEnum = Constant.TrimStyleEnum.Right;
					break;

				case Constant.Choices.TrimStyle.LEFT_AND_RIGHT:
					TrimStyleEnum = Constant.TrimStyleEnum.LeftAndRight;
					break;

				default:
					throw new CustomExceptions.TextExtractorException(string.Format("Trimstyle({0}) is not supported", trimStyle.Name));
			}
		}
	}
}