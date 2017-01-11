using System;
using System.Collections.Generic;
using kCura.EventHandler;
using Relativity.API;
using TextExtractor.EventHandlers.Interfaces;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;

namespace TextExtractor.EventHandlers.ExtractorTargetText
{
	public class TextExtractorTargetTextJob : ITextExtractorTargetTextJob
	{
		public IDBContext EddsDbContext { get; set; }
		public IDBContext DbContext { get; set; }
		public ISqlQueryHelper SqlQueryHelper { get; set; }
		public int ActiveArtifactId { get; set; }
		public int? Characters { get; set; }
		public int? Occurence { get; set; }
		public int? MaxExtractions { get; set; }
		public int? MinExtractions { get; set; }
		public String SelectedMarkerType{ get; set; }
		public bool? CaseSensitive { get; set; }
		public String SelectedDirection{ get; set; }
		public bool ApplyStopMarker { get; set; }
		public int? SelectedStartRegExArtifactId { get; set; }
		public int? SelectedStopRegExArtifactId { get; set; }
		public String PlainTextStartMarker{ get; set; }
		public String PlainTextStopMarker { get; set; }


		public TextExtractorTargetTextJob(IDBContext eddsDbContext, IDBContext dbContext, ISqlQueryHelper sqlQueryHelper, int activeArtifactId, 
			int? characters, int? occurence, int? maxExtractions, int? minExtractions, String selectedMarkerType, bool? caseSensitive,
			String selectedDirection, bool applyStopMarker, int? selectedRegExStartMarkerArtifactId, int? selectedRegExStopMarkerArtifactId,
			String plainTextStartMarker, String plainTextStop)
		{
			EddsDbContext = eddsDbContext;
			DbContext = dbContext;
			SqlQueryHelper = sqlQueryHelper;
			ActiveArtifactId = activeArtifactId;
			Characters = characters;
			Occurence = occurence;
			MaxExtractions = maxExtractions;
			MinExtractions = minExtractions;
			SelectedMarkerType = selectedMarkerType;
			CaseSensitive = caseSensitive;
			SelectedDirection = selectedDirection;
			ApplyStopMarker = applyStopMarker;
			SelectedStartRegExArtifactId = selectedRegExStartMarkerArtifactId;
			SelectedStopRegExArtifactId = selectedRegExStopMarkerArtifactId;
			PlainTextStartMarker = plainTextStartMarker;
			PlainTextStopMarker = plainTextStop;
		}

		public TextExtractorTargetTextJob() { }

		public Response ExecutePreSave()
		{
			var response = new Response
			{
				Success = true,
				Message = string.Empty
			};

			try
			{
				//Verify that no Profiles exist in the Manager and Worker queue tables
				List<int> profiles = SqlQueryHelper.RetrieveExtractorProfilesForField(DbContext, Constant.Guids.Fields.ExtractorTargetText.TargetText.ToString(), Constant.Guids.Fields.ExtractorProfile.TargetText.ToString(), ActiveArtifactId);

				if (profiles != null)
				{
					if (profiles.Count > 0)
					{
						var jobCountManager = SqlQueryHelper.RetrieveExtractorProfileCountInQueue(EddsDbContext, string.Join(",", profiles), Constant.Tables.ManagerQueue);
						var jobCountWorker = SqlQueryHelper.RetrieveExtractorProfileCountInQueue(EddsDbContext, string.Join(",", profiles), Constant.Tables.WorkerQueue);

						if (jobCountManager > 0 || jobCountWorker > 0)
						{
							response.Success = false;
							response.Message = Constant.ErrorMessages.TARGET_TEXT_RECORD_DETECTED;
						}
					}
				}

				//Verify that no negative Characters have been entered
				VerifyTargetTextFields(response, Characters, Constant.Sizes.EXTRACTOR_TARGET_TEXT_CHARACTERS_MINIMUM, Constant.Sizes.EXTRACTOR_TARGET_TEXT_CHARACTERS_MAXIMUM, Constant.ErrorMessages.TARGET_TEXT_CHARACTERS_NEGATIVE, Constant.ErrorMessages.TARGET_TEXT_CHARACTERS_MAXIMUM_EXCEEDED);
				if (response.Success == false)
				{
					return response;
				}

				//Verify that no negative Occurrences have been entered
				VerifyTargetTextFields(response, Occurence, Constant.Sizes.EXTRACTOR_TARGET_TEXT_OCCURENCE_MINIMUM, Constant.Sizes.EXTRACTOR_TARGET_TEXT_OCCURENCE_MAXIMUM, Constant.ErrorMessages.TARGET_TEXT_OCCURENCE_NEGATIVE, Constant.ErrorMessages.TARGET_TEXT_OCCURENCE_MAXIMUM_EXCEEDED);
				if (response.Success == false)
				{
					return response;
				}

				//Verify that no negative MaximumExtractions have been entered
				VerifyTargetTextFields(response, MaxExtractions, Constant.Sizes.EXTRACTOR_TARGET_TEXT_MAXIMUM_EXTRACTIONS_MIN_VALUE, Constant.Sizes.EXTRACTOR_TARGET_TEXT_MAXIMUM_EXTRACTIONS_MAX_VALUE, Constant.ErrorMessages.TARGET_TEXT_MAXIMUM_EXTRACTIONS_NEGATIVE, Constant.ErrorMessages.TARGET_TEXT_MAXIMUM_EXTRACTIONS_MAXIMUM_EXCEEDED);
				if (response.Success == false)
				{
					return response;
				}

				//Verify that no negative MaximumExtractions have been entered
				VerifyTargetTextFields(response, MinExtractions, Constant.Sizes.EXTRACTOR_TARGET_TEXT_MINIMUM_EXTRACTIONS_MIN_VALUE, Constant.Sizes.EXTRACTOR_TARGET_TEXT_MINIMUM_EXTRACTIONS_MAX_VALUE, Constant.ErrorMessages.TARGET_TEXT_MINIMUM_EXTRACTIONS_NEGATIVE, Constant.ErrorMessages.TARGET_TEXT_MINIMUM_EXTRACTIONS_MAXIMUM_EXCEEDED);
				if (response.Success == false)
				{
					return response;
				}

				//Verify if Minimum EXtraction value is bigger than Maximum Extraction 
				VerifyMinimumAndMaximumExtractionsFieldsDependencies(response, MinExtractions, MaxExtractions);
				if (response.Success == false)
				{
					return response;
				}

				//Verify that all fields required according MarkerType and ApplyStopMarker options are populated
				VerifyTargetTextFieldsConsistency(response, SelectedMarkerType, CaseSensitive, SelectedDirection, ApplyStopMarker, SelectedStartRegExArtifactId,
					SelectedStopRegExArtifactId, PlainTextStartMarker, PlainTextStopMarker);
				if (response.Success == false)
				{
					return response;
				}

				return response;
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Message = string.Format("{0}, Error Message: {1}", Constant.ErrorMessages.DEFAULT_ERROR_PREPEND, ex);
				return response;
			}
		}

		private static void VerifyTargetTextFields(Response response, int? fieldValue, int minimumCount, int maximumCount, string negativeMessage, string maximumCountExceededMessage)
		{
			if (fieldValue != null)
			{
				if (fieldValue < minimumCount)
				{
					response.Success = false;
					response.Message = negativeMessage;
				}

				//Verify that maximum count have not been exceeded
				if (fieldValue > maximumCount)
				{
					response.Success = false;
					response.Message = string.Format(maximumCountExceededMessage, Convert.ToString(maximumCount));
				}
			}
		}

		private static void VerifyMinimumAndMaximumExtractionsFieldsDependencies(Response response, int? minExtractions, int? maxExtractions)
		{
			if (minExtractions != null && maxExtractions != null)
			{
				if (maxExtractions < minExtractions)
				{
					response.Success = false;
					response.Message = Constant.ErrorMessages.TARGET_TEXT_MINIMUM_EXTRACTIONS_EXCEED_MAXIMUM_EXTRACTIONS;
				}
			}
		}

		private static void VerifyTargetTextFieldsConsistency(Response response, String selectedMarkerType, bool? caseSensitive,
			String selectedDirection, bool applyStopMarker, int? selectedRegExStartMarkerArtifactId,
			int? selectedRegExStopMarkerArtifactId, String plainTextStartMarker, String plainTextStop)
		{
			switch(selectedMarkerType)
			{
				case Constant.Choices.MarkerType.REGULAR_EXPRESSION:
					string errorMessageForRegex = null;
					if (selectedRegExStartMarkerArtifactId == null || selectedRegExStartMarkerArtifactId < 1)
					{
						response.Success = false;
						errorMessageForRegex = "You have to select a start marker. ";
						response.Message = errorMessageForRegex;
					}
					if (applyStopMarker == true && (selectedRegExStopMarkerArtifactId == null || selectedRegExStopMarkerArtifactId < 1))
					{
						response.Success = false;
						errorMessageForRegex = errorMessageForRegex + "You have to select a stop marker. ";
						response.Message = errorMessageForRegex;
					}
					if (applyStopMarker == false && String.IsNullOrWhiteSpace(selectedDirection))
					{
						response.Success = false;
						errorMessageForRegex = errorMessageForRegex + "You have to select a direction. ";
						response.Message = errorMessageForRegex;
					}
					break;
				case Constant.Choices.MarkerType.PLAIN_TEXT:
					string errorMessage = null;
					if (String.IsNullOrWhiteSpace(plainTextStartMarker))
					{
						response.Success = false;
						errorMessage = "You have to type a start marker. ";
						response.Message = errorMessage;
					}
					if (applyStopMarker == true && String.IsNullOrWhiteSpace(plainTextStop))
					{
						response.Success = false;
						errorMessage = errorMessage + "You have to type a stop marker. ";
						response.Message = errorMessage;
					}
					if (applyStopMarker == false && String.IsNullOrWhiteSpace(selectedDirection))
					{
						response.Success = false;
						errorMessage = errorMessage + "You have to select a direction. ";
						response.Message = errorMessage;
					}
					if (caseSensitive == null)
					{
						response.Success = false;
						errorMessage = errorMessage + "You have to select case sensitivity. ";
						response.Message = errorMessage;
					}
					break;
			}
		}

		public Response ExecutePreCascadeDelete()
		{
			var response = new Response { Success = false, Exception = new SystemException(Constant.ErrorMessages.EXTRACTION_TARGET_TEXT_RECORD_DEPENDENCY) };
			return response;
		}
	}
}
