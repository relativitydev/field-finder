using System;
using System.Collections.Generic;
using kCura.Relativity.Client.DTOs;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Data
{
	public class ArtifactGenerator : ADependency
	{
		public RandomGenerator Random;

		public ArtifactGenerator()
		{
			Random = new RandomGenerator();
		}

		#region ResultSetGenerator

		public QueryResultSet<Document> GetDocumentResultSet(Int32 results = 1)
		{
			var resultSet = new QueryResultSet<Document>();
			resultSet.Success = true;
			resultSet.Results = this.GetDocumentResults(results);
			resultSet.TotalCount = results;

			return resultSet;
		}

		private List<Result<Document>> GetDocumentResults(Int32 number = 1)
		{
			var results = new List<Result<Document>>();

			for (int i = 0; i < number; i++)
			{
				var result = new Result<Document>();
				result.Success = true;
				result.Artifact = this.GetDocument();
				results.Add(result);
			}

			return results;
		}

		#endregion ResultSetGenerator

		#region RdoGenerator

		public RDO GetExtractorSetRdo()
		{
			var rdo = new RDO(this.Random.Number());
			rdo.ArtifactTypeGuids = new List<Guid>() { Constant.Guids.ObjectType.ExtractorSet };

			// Populate dependent Rdos 
			var savedSearch = this.GetSavedSearchRdo();
			var template = this.GetExtractorProfileRdo();
			var textField = this.GetTextFieldRdo();

			// Generate all fake fields 
			var textExtractorJobFields = new List<FieldValue>()
			 {
				 new FieldValue(Constant.Guids.Fields.ExtractorSet.SetName)
					 {
						 ValueAsFixedLengthText = "Test Name"
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorSet.Status)
					 {
						 ValueAsFixedLengthText = Constant.ExtractorSetStatus.SUBMITTED
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorSet.SavedSearch)
					 {
						 ValueAsSingleObject = savedSearch
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorSet.ExtractorProfile)
					 {
						 ValueAsSingleObject = template
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorSet.SourceLongTextField)
					 {
						 ValueAsSingleObject = textField
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorSet.NumberOfUpdatesWithValues)
					 {
						 ValueAsWholeNumber = Random.Number(1, 100)
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorSet.TotalExpectedUpdates)
					 {
						 ValueAsWholeNumber = Random.Number(1, 300)
					 },
                 new FieldValue(Constant.Guids.Fields.ExtractorSet.EmailNotificationRecipients)
				     {
					     ValueAsFixedLengthText = String.Empty
				     }
			 };

			rdo.Fields = textExtractorJobFields;

			return rdo;
		}

		public RDO GetExtractorSetHistoryRdo()
		{
			var rdo = new RDO(this.Random.Number());
			rdo.ArtifactTypeGuids = new List<Guid>() { Constant.Guids.ObjectType.ExtractorSetHistory };

			// Generate all fake fields 
			var textExtractorJobHistoryFields = new List<FieldValue>()
			 {
				 new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.Name)
					 {
						 ValueAsFixedLengthText = "Test Name"
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.DocumentIdentifier)
					 {
						 Value = "Test Document Identifier"
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.Status)
					 {
						 ValueAsLongText = "Test Status"
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.Details)
					 {
						 ValueAsLongText = "Test Error"
					 }
			 };

			rdo.Fields = textExtractorJobHistoryFields;

			return rdo;
		}

		private RDO GetSavedSearchRdo()
		{
			var rdo = new RDO(this.Random.Number());

			return rdo;
		}

		public RDO GetExtractorProfileRdo(int numberOfFields = 1)
		{
			var rdo = new RDO(this.Random.Number());
			rdo.ArtifactTypeGuids = new List<Guid>() { Constant.Guids.ObjectType.ExtractorProfile };

			// Generate all fake fields 
			var textExtractorTemplateFields = new List<FieldValue>()
			 {
				 new FieldValue(Constant.Guids.Fields.ExtractorProfile.ProfileName)
					 {
						 ValueAsFixedLengthText = "Test Name"
					 }
			 };

			// Add multi-object field; can't do it any other way 
			var multiObj = new FieldValue(Constant.Guids.Fields.ExtractorProfile.TargetText);
			var fields = GetTextExtractorFields(numberOfFields);
			multiObj.SetValueAsMultipleObject(fields);
			textExtractorTemplateFields.Add(multiObj);

			rdo.Fields = textExtractorTemplateFields;

			return rdo;
		}

        public RDO GetExtractorRegularExpressionRdo()
        {
            var rdo = new RDO(this.Random.Number());

            rdo.ArtifactTypeGuids = new List<Guid>() { Constant.Guids.ObjectType.ExtractorRegularExpression };

            // Generate all fake fields 
            var extractorRegularExpressionFields = new List<FieldValue>()
			 {
				 new FieldValue(Constant.Guids.Fields.ExtractorRegularExpression.Name)
					 {
						 ValueAsFixedLengthText = "Test Name"
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorRegularExpression.RegularExpression)
					 {
						 Value = Random.Word()
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorRegularExpression.Description)
					 {
						 ValueAsLongText = "Test Description"
					 }
			 };

            rdo.Fields = extractorRegularExpressionFields;

            return rdo;
        }

		private FieldValueList<Artifact> GetTextExtractorFields(int number = 1)
		{
			var fields = new FieldValueList<Artifact>();

			for (int i = 0; i < number; i++)
			{
				var rdo = GetExtractorTargetTextRdo();
				fields.Add(rdo);
			}

			return fields;
		}

		public RDO GetExtractorTargetTextRdo()
		{
			var rdo = new RDO(this.Random.Number());
			rdo.ArtifactTypeGuids = new List<Guid>() { Constant.Guids.ObjectType.TargetText };

			// TrimStyle choice 
			var trimStyleChoice = GetTrimStyleChoice();
			var extractionDirectionChoice = GetExtractionDirectionChoice();
            var markerType = GetMarkerTypeChoice();

			// Generate all fake fields 
			var textExtractorFieldFields = new List<FieldValue>()
			 {
				 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.TargetName)
					 {
						 ValueAsFixedLengthText = "Test Field Name"
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.NumberofCharacters)
					 {
						 ValueAsWholeNumber = Random.Number(minSize: 0, maxSize: 50)
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.Direction)
					 {
						 ValueAsSingleChoice = extractionDirectionChoice
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.DestinationField)
					 {
						 ValueAsSingleObject = GetTextFieldRdo()
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.PlainTextStartMarker)
					 {
						 Value = Random.Word() // Doesn't implement Long Text
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.CaseSensitive)
					 {
						 ValueAsYesNo = Random.Boolean()
					 },
                 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.IncludeMarker)
				     {
					     ValueAsYesNo = Random.Boolean()
				     },
				 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.TrimStyle)
					 {
						 ValueAsSingleChoice = trimStyleChoice
					 },
				 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.Occurrence)
					 {
						 ValueAsWholeNumber = Random.Number(0, 10)
					 },
                 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.MarkerType)
				     {
					     Value =  markerType
				     },                     
                 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.PlainTextStopMarker)
				     {
					     Value =  String.Empty
				     },                     
                 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.MinimumExtractions)
				     {
					     Value =  Random.Number(1, 5)
				     },                     
                 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.MaximumExtractions)
				     {
					     Value =  Random.Number(5, 10)
				     },                     
                 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.ResultsCustomDelimiter)
				     {
					     Value =  "; "
				     },                     
                 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStartMarker)
				     {
					     Value =  String.Empty // Doesn't implement Long Text
				     },                     
                 new FieldValue(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStopMarker)
				     {
					     Value =  String.Empty // Doesn't implement Long Text
				     }
			 };

			rdo.Fields = textExtractorFieldFields;

			return rdo;
		}

		private RDO GetTextFieldRdo()
		{
			var rdo = new RDO(this.Random.Number());

			return rdo;
		}

		#endregion RdoGenerator

		#region DocumentGenerator

		private Document GetDocument()
		{
			var doc = new Document(Random.Number());

			return doc;
		}

		#endregion DocumentGenerator

		#region ChoiceGenerator

		private Choice GetExtractionDirectionChoice()
		{
			// Randomizes the selected choice 
			var possibleDirections = new object[]
			 {
				 Constant.Choices.Direction.LEFT,
				 Constant.Choices.Direction.RIGHT
			 };

			var direction = Random.Randomize<String>(possibleDirections);

			var choice = new Choice(Random.Number())
			 {
				 ArtifactTypeGuids =
					 new List<Guid>() { Constant.Guids.Fields.ExtractorTargetText.Direction },
				 Name = direction
			 };
			return choice;
		}

		private Choice GetExtractionDirectionChoice_Invalid()
		{
			var choice = new Choice(Random.Number())
			{
				ArtifactTypeGuids =
					new List<Guid>() { Constant.Guids.Fields.ExtractorTargetText.Direction },
				Name = "abc"
			};
			return choice;
		}

		private Choice GetTrimStyleChoice()
		{
			// Randomizes the selected choice 
			var possibleTrimStyles = new object[]
			 {
				 Constant.Choices.TrimStyle.NONE,
				 Constant.Choices.TrimStyle.LEFT,
				 Constant.Choices.TrimStyle.RIGHT,
				 Constant.Choices.TrimStyle.LEFT_AND_RIGHT
			 };

			var trimStyle = Random.Randomize<String>(possibleTrimStyles);

			var choice = new Choice(Random.Number())
			 {
				 ArtifactTypeGuids =
					 new List<Guid>() { Constant.Guids.Fields.ExtractorTargetText.TrimStyle },
				 Name = trimStyle
			 };
			return choice;
		}

		private Choice GetTrimStyleChoice_Invalid()
		{
			var choice = new Choice(Random.Number())
			{
				ArtifactTypeGuids =
					new List<Guid>() { Constant.Guids.Fields.ExtractorTargetText.TrimStyle },
				Name = "abc"
			};
			return choice;
		}

        private Choice GetMarkerTypeChoice()
        {
            // Randomizes the selected choice 
            var possibleMarkerTypes = new object[]
			 {
				 Constant.Choices.MarkerType.PLAIN_TEXT,
				 //Constant.Choices.MarkerType.REGULAR_EXPRESSION				
			 };

            var markerType = Random.Randomize<String>(possibleMarkerTypes);

            var choice = new Choice(Random.Number())
            {
                ArtifactTypeGuids =
                    new List<Guid>() { Constant.Guids.Fields.ExtractorTargetText.MarkerType },
                Name = markerType
            };
            return choice;
        }

        private Choice GetMarkerTypeChoice_Invalid()
        {
            var choice = new Choice(Random.Number())
            {
                ArtifactTypeGuids =
                    new List<Guid>() { Constant.Guids.Fields.ExtractorTargetText.MarkerType },
                Name = "abc"
            };
            return choice;
        }

		#endregion ChoiceGenerator

		#region invalid data

		public RDO GetExtractorTargetTextRdo_Invalid_TargetName()
		{
			var rdo = GetExtractorTargetTextRdo();
			rdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.TargetName).Value = null;
			return rdo;
		}

		public RDO GetExtractorTargetTextRdo_Invalid_Marker()
		{
			var rdo = GetExtractorTargetTextRdo();
			rdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.PlainTextStartMarker).Value = null;
			return rdo;
		}

		public RDO GetExtractorTargetTextRdo_Invalid_DestinationField()
		{
			var rdo = GetExtractorTargetTextRdo();
			rdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.DestinationField).Value = null;
			return rdo;
		}

		public RDO GetExtractorTargetTextRdo_Invalid_Direction()
		{
			var rdo = GetExtractorTargetTextRdo();
			rdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.Direction).ValueAsSingleChoice = GetExtractionDirectionChoice_Invalid();
			return rdo;
		}

		public RDO GetExtractorTargetTextRdo_Invalid_NumberOfCharacters()
		{
			var rdo = GetExtractorTargetTextRdo();
			rdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.NumberofCharacters).Value = null;
			return rdo;
		}

		public RDO GetExtractorTargetTextRdo_Invalid_TrimStyle()
		{
			var rdo = GetExtractorTargetTextRdo();
			rdo.Fields.Get(Constant.Guids.Fields.ExtractorTargetText.TrimStyle).ValueAsSingleChoice = GetTrimStyleChoice_Invalid();
			return rdo;
		}

		public RDO GetExtractorProfileRdo_Invalid_ProfileName(int numberOfFields = 1)
		{
			var rdo = GetExtractorProfileRdo();
			rdo.Fields.Get(Constant.Guids.Fields.ExtractorProfile.ProfileName).Value = null;
			return rdo;
		}

		public RDO GetExtractorProfileRdo_Invalid_TargetText()
		{
			var rdo = GetExtractorProfileRdo();
			rdo.Fields.Get(Constant.Guids.Fields.ExtractorProfile.TargetText).Value = null;
			return rdo;
		}

		#endregion
	}
}