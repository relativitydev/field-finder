using System;
using System.Collections.Generic;

namespace TextExtractor.Helpers
{
	public class Constant
	{
		public class Tables
		{
			public static readonly String ManagerQueue = "TextExtractor_ManagerQueue";
			public static readonly String WorkerQueue = "TextExtractor_WorkerQueue";
			public static readonly String ErrorLog = "TextExtractor_ErrorLog";
			public static readonly String ExtractorSet = "ExtractorSet";
			public static readonly String ExtractorSetHistory = "ExtractorSetHistory";
			public static readonly String ExtractorRegularExpression = "ExtractorRegularExpression";
			public static readonly String ExtractorTargetText = "ExtractorTargetText";
		}

		public class Names
		{
			public static readonly String ApplicationName = "Field Finder";
			
			public static readonly String TablePrefix = "TextExtractor_";

			public class Console
			{
				public const String SUBMIT = "submit";
				public const String CANCEL = "cancel";
				public const String REFRESH = "refresh";
			}
		}

		public class Guids
		{
			public static readonly Guid ApplicationGuid = new Guid("C0B64157-796B-402F-8DE9-45407CCFE1BE");

			public class ObjectType
			{
				public static Guid TargetText = new Guid("B627DD38-9CF6-4C81-84EA-236D43BC030A");
				public static Guid ExtractorProfile = new Guid("B5E509C1-876C-4FA3-BFDC-86C4449D5199");
				public static Guid ExtractorSet = new Guid("34A3E305-A826-491A-8033-942134C08A73");
				public static Guid ExtractorSetHistory = new Guid("7741E62E-0859-4090-A1E8-6F9A4CFB8254");
				public static Guid ExtractorRegularExpression = new Guid("8B4D7333-9B45-4BD3-AA44-A4F8AB9AB444");
			}

			public class Fields
			{
				public class Document
				{
					public static readonly Guid TextExtractorDetails = new Guid("3E927F09-D34C-4A8F-9C0C-1C3BE7C90DBA");
					public static readonly Guid TextExtractorErrors = new Guid("D6D72072-FA03-4994-AA66-1210EB136830");
				}

				public class ExtractorTargetText
				{
					public static readonly Guid SystemCreatedOn = new Guid("6D0A64F1-E9CE-4CD5-A030-AB2C5FC13E50");
					public static readonly Guid SystemLastModifiedOn = new Guid("966409BA-69E2-4E13-9112-4EFDA63D14B9");
					public static readonly Guid SystemCreatedBy = new Guid("DFA1B1D1-7564-414A-B40F-8562E81DE05F");
					public static readonly Guid SystemLastModifiedBy = new Guid("45BE3B1B-5E6C-42B0-BFE1-1EDB844EF35F");
					public static readonly Guid ArtifactID = new Guid("E456DABD-001A-4C3D-A5F8-D5E69665F5F6");
					public static readonly Guid TargetName = new Guid("C2599AEB-BC8D-4509-9C50-6FC330056107");
					public static readonly Guid NumberofCharacters = new Guid("2D5B2760-6C2A-40A0-8BDA-12A9B82E6446");
					public static readonly Guid PlainTextStartMarker = new Guid("9CE99FE2-2526-4EE7-947C-A58E74F47474");
					public static readonly Guid DestinationField = new Guid("04D5BBDB-0907-48FA-8B76-948DCAB34DB6");
					public static readonly Guid TrimStyle = new Guid("978A4D84-F864-4039-82EF-3002A7F22E91");
					public static readonly Guid Occurrence = new Guid("4B6E649A-59F4-4A31-8601-A28AC85DB8D2");
					public static readonly Guid Direction = new Guid("CD054717-5494-44FE-BF6B-E8BA89BF8C96");
					public static readonly Guid CaseSensitive = new Guid("0F591EFF-C37B-4263-919C-1E8B6C79DDB3");
					public static readonly Guid TargetText = new Guid("CEB38AC5-A6E5-4A69-816E-08AB7C3C6903");
					public static readonly Guid MarkerType = new Guid("AA8772A5-359F-4D10-AC01-BB7945CB4CDA");
					public static readonly Guid RegularExpressionStartMarker = new Guid("F0819919-F042-490B-AEE7-6ED10EBFD1F6");
					public static readonly Guid MaximumExtractions = new Guid("D801E98F-AB70-4341-84E5-2B8C5ADA774C");
					public static readonly Guid MinimumExtractions = new Guid("103CDDCE-166A-4C04-9382-A59C64BA2B79");
					public static readonly Guid ResultsCustomDelimiter = new Guid("1DCFF5D2-D5C0-494B-BB64-9A7B77FAC3D6");
					public static readonly Guid RegularExpressionStopMarker = new Guid("BE9E2560-F4A3-458F-887D-FBFF42870E9E");
					public static readonly Guid PlainTextStopMarker = new Guid("81FCA6AF-5BFF-4DA8-844D-D17D5029B92F");
					public static readonly Guid ApplyStopMarker = new Guid("C0EA46EB-0402-4B37-95B2-815E3E676E6A");
					public static readonly Guid IncludeMarker = new Guid("C0B5742D-5E7C-4E49-A03F-F03550C65E02");

				}

				public class ExtractorProfile
				{
					public static readonly Guid SystemCreatedOn = new Guid("DC55A754-E289-4138-B573-82721EDF7538");
					public static readonly Guid SystemLastModifiedOn = new Guid("F8EC0D8D-42BB-4EA5-89BA-D23D60D2AD68");
					public static readonly Guid SystemCreatedBy = new Guid("7446AD46-9097-4AFF-A501-70662B4839F2");
					public static readonly Guid SystemLastModifiedBy = new Guid("34A45CAA-2ED2-4575-BA5C-0E8FEB451111");
					public static readonly Guid ArtifactID = new Guid("7D301A71-E423-4C3D-B2F2-51CE8C67D266");
					public static readonly Guid ProfileName = new Guid("D9E07AE5-ADE3-416E-BAC8-A805491F1F26");
					public static readonly Guid TargetText = new Guid("8D164BC4-5D71-48A3-A0E8-EC6540AF8F6D");
				}

				public class ExtractorSet
				{
					public static readonly Guid SystemCreatedOn = new Guid("A6A6D447-AE51-4ED1-916D-D7143C850FBD");
					public static readonly Guid SystemLastModifiedOn = new Guid("57321E50-70EC-479B-A635-4988F8F0FB2A");
					public static readonly Guid SystemCreatedBy = new Guid("C0B07454-3034-4A77-BD4F-C9F41C3D7A22");
					public static readonly Guid SystemLastModifiedBy = new Guid("201EE8ED-967A-4798-87EC-47109F50037B");
					public static readonly Guid ArtifactID = new Guid("9B66F9E3-DAA4-49CC-BC10-0DDC44032FEF");
					public static readonly Guid SetName = new Guid("9FBC4ED4-E19D-47C9-96A1-5438FB406B83");
					public static readonly Guid SourceLongTextField = new Guid("05F99CE2-842B-43C6-89E3-2F6F13E07B6F");
					public static readonly Guid TotalExpectedUpdates = new Guid("440C25AF-7CF5-435B-BEA6-F352956D1743");
					public static readonly Guid Details = new Guid("2627ADE1-58CF-4199-893A-B26B25A160C4");
					public static readonly Guid Status = new Guid("B1B61C4B-5163-40FE-A94B-E39D33C1838F");
					public static readonly Guid ExtractorProfile = new Guid("045D2E0A-74F1-46B2-91AA-CDA2B2E6A4F1");
					public static readonly Guid NumberOfUpdatesWithValues = new Guid("EE8453B8-4A29-43DF-A2DC-81566324108A");
					public static readonly Guid SavedSearch = new Guid("269C3DB2-1E9B-4F55-81C6-D06399CD7179");
					public static readonly Guid EmailNotificationRecipients = new Guid("D2786BF2-131A-4CF9-83FA-42F577265699");
				}

				public class ExtractorSetHistory
				{
					public static readonly Guid SystemCreatedOn = new Guid("D2081BA6-476D-44F8-AAD3-94176CE6F2D5");
					public static readonly Guid SystemLastModifiedOn = new Guid("A0E02107-E0FE-42B6-BD7E-6C8BB9E1BEBC");
					public static readonly Guid SystemCreatedBy = new Guid("012D2C97-F590-4354-AC76-2FE2601A6FB8");
					public static readonly Guid SystemLastModifiedBy = new Guid("A10690A8-EE54-40DC-A07C-8798FE86AAFB");
					public static readonly Guid ArtifactID = new Guid("063E9B47-4986-4AA7-BC87-781588EE7514");
					public static readonly Guid Name = new Guid("DEB1DD79-A2FC-415E-99D8-86E5CC6FACA0");
					public static readonly Guid Details = new Guid("4A40E5DD-3316-4F4B-B76D-C229385B8A7A");
					public static readonly Guid DocumentIdentifier = new Guid("A4DD4CA2-2350-4327-B23E-5AED786CD04A");
					public static readonly Guid DestinationField = new Guid("0A2D9477-1A6F-4151-BCF5-1E939F7CF216");
					public static readonly Guid Status = new Guid("9A00B0F1-6FBD-4EB1-8220-B4E080B6A297");
					public static readonly Guid ExtractorSet = new Guid("2DD83945-E4D7-4076-A976-42F0407D9CFF");
					public static readonly Guid TargetName = new Guid("14E7E3A2-A51B-4E30-9A24-26353C00F075");
					public static readonly Guid StartMarker = new Guid("932F1442-5F95-4A39-A4C1-5C1E921D64C4");
					public static readonly Guid StopMarker = new Guid("1C239201-3EE8-4A4A-8BC3-3C3BE0C5E1E9");
					public static readonly Guid MarkerType = new Guid("C81F93C2-4A3C-4A05-ADE3-D5AFB8F31B7B");
				}

				public class ExtractorRegularExpression
				{
					public static readonly Guid SystemCreatedOn = new Guid("B4890C9E-4EEB-4404-90B4-9D44FC53ECCF");
					public static readonly Guid SystemLastModifiedOn = new Guid("7232A7D8-9B73-4962-A186-8AD1A4CBDC09");
					public static readonly Guid SystemCreatedBy = new Guid("C4494FB0-A6C5-4AEE-8FDC-751CAD795736");
					public static readonly Guid SystemLastModifiedBy = new Guid("6CEB6E29-11BB-4999-A9C1-6C13A4ACF744");
					public static readonly Guid ArtifactID = new Guid("F27E7E1B-E264-4222-A48D-9C0F5F1D10FA");
					public static readonly Guid Name = new Guid("B1F283DB-AAD8-448D-BCAA-FB36A674C482");
					public static readonly Guid RegularExpression = new Guid("F224401B-4C0E-4502-AE2D-246C653E74CD");
					public static readonly Guid Description = new Guid("AF66E543-3D4A-461F-9FCD-8BC2ACB51011");
				}
			}

			public class Choices
			{
				public class TrimStyle
				{
					public static readonly Guid None = new Guid("470E46E5-575A-4698-A091-FCB1D3EF2CCA");
					public static readonly Guid Left = new Guid("EE215AE6-89AC-4F9F-A3C3-EE438540B07A");
					public static readonly Guid Right = new Guid("2C9CBD2F-70B2-4A88-B0F5-C4746F68C6D0");
					public static readonly Guid LeftAndRight = new Guid("93F682CD-1F3A-4319-A62A-DD299264F744");
				}

				public class ExtractionDirection
				{
					public static readonly Guid Left = new Guid("BED7E276-45CC-43D9-A5C9-D714BA9CB374");
					public static readonly Guid Right = new Guid("C29DD267-C766-4C9C-9D60-8C45B262436A");
					public static readonly Guid LeftAndRight = new Guid("D71DC0B5-0151-4368-BE1F-1D31BDF83146");
				}

				public class MarkerType
				{
					public static readonly Guid RegularExpression = new Guid("A2084043-8762-4694-AB57-CE484557DE80");
					public static readonly Guid PlainText = new Guid("661E88B7-564D-498E-9082-1BA4F01B863C");
				}
			}

			public class Layout
			{
				public static Guid TargetText = new Guid("C745F04D-BC34-4008-A0D1-635AA165875B");
				public static Guid ExtractorProfile = new Guid("EBA6DC06-9129-4F90-BF16-CA6FF4E7A121");
				public static Guid ExtractorSet = new Guid("CEB98276-223B-4902-9F75-665F56290E70");
				public static Guid ExtractorSetHistory = new Guid("7D2F1C46-B7BC-4252-94B7-E1E893086488");
				public static Guid ExtractorRegularExpression = new Guid("9C7E6A86-11BF-45C7-B68D-E2A27AAE756D");
			}
		}

		public class Sizes
		{
			public const Int32 MANAGER_QUEUE_BATCH_SIZE = 5;
			public const Int32 SAVED_SEARCH_BATCH_SIZE = 1000;
			public const Int32 WORKER_BATCH_SIZE = 10;
            public const Int32 EXTRACTOR_TARGET_TEXT_SOURCE_LENGTH_MAXIMUM = 4000000;
			public const Int32 EXTRACTOR_TARGET_TEXT_OCCURENCE_MINIMUM = 1;
			public const Int32 EXTRACTOR_TARGET_TEXT_OCCURENCE_MAXIMUM = 100;
			public const Int32 EXTRACTOR_TARGET_TEXT_CHARACTERS_MINIMUM = 1;
			public const Int32 EXTRACTOR_TARGET_TEXT_CHARACTERS_MAXIMUM = 10000;
			public const Int32 EXTRACTOR_TARGET_TEXT_MAXIMUM_EXTRACTIONS_MAX_VALUE = 1000;
			public const Int32 EXTRACTOR_TARGET_TEXT_MAXIMUM_EXTRACTIONS_MIN_VALUE = 1;
			public const Int32 EXTRACTOR_TARGET_TEXT_MINIMUM_EXTRACTIONS_MAX_VALUE = 100;
			public const Int32 EXTRACTOR_TARGET_TEXT_MINIMUM_EXTRACTIONS_MIN_VALUE = 1;
			public const Int32 MANAGER_CHECK_CANCELLATION_INTERVAL = 10000;
			public const Boolean DEFAULT_CASE_SENSITIVITY = true;
			public const Int32 DEFAULT_OCCURENCE = 1;
			public const Int32 DEFAULT_MAXIMUM_EXTRACTIONS = 1000;
			public const Int32 DEFAULT_MINIMUM_EXTRACTIONS = 1;
			public const String DEFAULT_DELIMITER = "; ";
		}

		public class ExtractorSetStatus
		{
			public const String SUBMITTED = "Submitted";
			public const String CANCELLED = "Cancelled";
			public const String IN_PROGRESS_MANAGER_PROCESSING = "In Progress - Manager Processing";
			public const String IN_PROGRESS_MANAGER_COMPLETE = "In Progress - Manager Complete";
			public const String IN_PROGRESS_WORKER_PROCESSING = "In Progress - Worker Processing";
			public const String COMPLETE = "Complete";
			public const String COMPLETE_WITH_ERRORS = "Complete - With Errors";
			public const String ERROR = "Error";

			public static readonly List<String> JobStatusList = new List<String> { SUBMITTED, CANCELLED, IN_PROGRESS_MANAGER_PROCESSING, IN_PROGRESS_MANAGER_COMPLETE, IN_PROGRESS_WORKER_PROCESSING, COMPLETE, ERROR };

			public class DetailMessages
			{
				public const String COMPLETE_WITH_ERRORS_DETAILS = "Error encountered while processing at least one document. Please refer to the 'Text Extractor Errors' field on Document object for further detail.";
			}
		}

		public class ExtractionSetHistoryStatus
		{
			public const String COMPLETE_TEXT_EXTRACTED = "Complete - Text Extracted";
			public const String COMPLETE_TEXT_NOT_FOUND = "Complete - Text Not Found";
			public const String COMPLETE_MARKER_NOT_FOUND = "Complete - Marker Not Found";
			public const String ERROR = "Error";
		}

		public class QueueStatus
		{
			public static readonly Int32 NotStarted = 0;
			public static readonly Int32 InProgress = 1;
			public static readonly Int32 Error = -1;
		}

		public class Messages
		{
			public const String PRIORITY_REQUIRED = "Please enter a priority";
			public const String ARTIFACT_ID_REQUIRED = "Please enter an artifact ID";
		}

		public enum AgentType
		{
			Manager,
			Worker
		}

		public class ErrorMessages
		{
			public const String DEFAULT_ERROR_PREPEND = "Error encountered: ";
			public const String DEFAULT_ERROR_MESSAGE = "An error occured when extracting text.";
			public const String EXTRACTION_PROFILE_RECORD_DETECTED = "The Extractor Profile record is currently being processed.  Please wait for execution to complete or cancel the Extraction Set.";
			public const String EXTRACTION_PROFILE_RECORD_DEPENDENCY = "At least one Extractor Profile record is currently associated to one or more Extractor Sets and cannot be deleted.";
			public const String EXTRACTION_SET_RECORD_DETECTED = "The Extractor Set record is currently being processed.  Please wait for execution to complete or cancel the Extraction Set.";
			public const String EXTRACTION_SET_RECORD_COMPLETE = "The Extractor Set cannot be cancelled as it has already completed.";
			public const String EXTRACTION_SET_MISSING_FIELDS = "Please make sure to provide all required fields.";
			public const String EXTRACTION_SET_CANNOT_DELETE_STATUS_NOT_NULL = "The current Extractor Set record cannot be deleted as it has already been submitted for processing.";
			public const String EXTRACTION_SET_CANNOT_DELETE_CURRENT_RECORD_QUEUE = "The current Extractor Set record cannot be deleted as records exist in the Agent queue tables.";
			public const String EXTRACTION_SET_CANNOT_DELETE_MULTIPLE_RECORD_HISTORY = "At least one Extractor Set record cannot be deleted as it has associated History records.";
			public const String EXTRACTION_SET_CANNOT_DELETE_MULTIPLE_RECORD_QUEUE = "At least one Extractor Set record cannot be deleted as records exist in the Agent queue tables.";
			public const String EXTRACTION_SET_CANNOT_DELETE_MULTIPLE_RECORD = "At least one selected Extractor Set record cannot be deleted as it has already been submitted for processing.";
			public const String EXTRACTION_SET_CANNOT_DELETE_CURRENT_RECORD_HISTORY = "At least one Extractor Set record cannot be deleted as dependencies currently exist.";
			public const String EXTRACTION_PROFILE_CANNOT_DELETE_MULTIPLE_RECORD_QUEUE = "At least one Extractor Profile record cannot be deleted as records exist in the Agent queue tables.";
            public const String TARGET_TEXT_SOURCE_LENGTH_EXCEEDS_MAXIMUM = "Target Text Source Field exceeds Maximum length of {0} characters";
			public const String EXTRACTION_TARGET_TEXT_RECORD_DEPENDENCY = "At least one Extractor Target Text record is currently associated to one or more Extractor Profiles and cannot be deleted.";
			public const String FAILEDQUERY_SAVEDSEARCH = "Saved search query failed to return document results";
			public const String CHARACTER_LENGTH_IS_NEGATIVE = "CharacterLength cannot be negative.";
			public const String OCCURRENCE_LENGTH_IS_NEGATIVE = "Occurrence cannot be negative.";
			public const String MATCHING_TEXT_LENGTH_GREATER_THAN_TEXT_SOURCE_LENGTH = "Target Text Marker length is greater than the Source Long Text Field length.";
			public const String DEFAULT_TEXT_EXTRACTION_ERROR_MESSAGE = "An error occured when extracting text.";
			public const String TARGET_TEXT_RECORD_DETECTED = "The Target Text record is currently being processed.  Please wait for execution to complete or cancel the associated Extraction Set.";
			public const String TARGET_TEXT_OCCURENCE_NEGATIVE = "Please make sure to enter a positive value for the Occurrence field.";
			public const String TARGET_TEXT_MAXIMUM_EXTRACTIONS_NEGATIVE = "Please make sure to enter a positive value for the Maximum Extractions field.";
			public const String TARGET_TEXT_MINIMUM_EXTRACTIONS_NEGATIVE = "Please make sure to enter a positive value for the Minimum Extractions field.";
			public const String TARGET_TEXT_OCCURENCE_MAXIMUM_EXCEEDED = "Please make sure to enter a positive value less than or equal to {0} for the Occurrence field.";
			public const String TARGET_TEXT_MAXIMUM_EXTRACTIONS_MAXIMUM_EXCEEDED = "Please make sure to enter a positive value less than or equal to {0} for the Maximum Extractioms field.";
			public const String TARGET_TEXT_MINIMUM_EXTRACTIONS_MAXIMUM_EXCEEDED = "Please make sure to enter a positive value less than or equal to {0} for the Minimum Extractioms field.";
            public const String TARGET_TEXT_MINIMUM_EXTRACTIONS_EXCEED_MAXIMUM_EXTRACTIONS = "Maximum Extractions must be greater than or equal to Minimum Extractions. ";
			public const String TARGET_TEXT_CHARACTERS_NEGATIVE = "Please make sure to enter a positive value for the Number of Characters field.";
			public const String TARGET_TEXT_CHARACTERS_MAXIMUM_EXCEEDED = "Please make sure to enter a positive value less than or equal to {0} for the Number of Characters field.";
			public const String DEFAULT_CONVERT_TO_EXTRACTOR_TARGET_TEXT_ERROR_MESSAGE = "An error occured when converting extractorTargetTextRdo to ExtractorTargetText";
			public const String DEFAULT_CONVERT_TO_EXTRACTOR_PROFILE_ERROR_MESSAGE = "An error occured when converting extractorProfileRdo to ExtractorProfile";
			public const String DEFAULT_CONVERT_TO_EXTRACTOR_SET_ERROR_MESSAGE = "An error occured when converting extractorSetRdo to ExtractorSet";
			public const String DEFAULT_CONVERT_TO_EXTRACTOR_REGULAR_EXPRESSION_ERROR_MESSAGE = "An error occured when converting extractorRegularExpression to ExtractorRegularExpression";
			public const String DOCUMENT_ERROR_ENCOUNTERED = "Document error encountered ({0}) in Extractor Set ({1})";
			public const String SAVED_SEARCH_IS_EMPTY = "An error encountered. The selected Saved Search for the Extractor Set does not contain any documents.";
			public const String EXTRACTOR_SET_SOURCE_LONG_TEXT_FIELD_IS_EMPTY = "Extractor Set - Source Long Text Field is Empty";
			public const String TARGET_TEXT_DESTINATION_FIELD_IS_EMPTY = "Target Text - Destination Field is Empty";
			public const String TARGET_TEXT_START_MARKER_IS_EMPTY = "Target Text - Start Marker is Empty";
			public const String TARGET_TEXT_REGULAR_EXPRESSION_IS_EMPTY = "Target Text - Regular Expression is Empty";
			public const String TARGET_TEXT_PLAIN_TEXT_MARKER_IS_EMPTY = "Target Text - Plain Text Marker is Empty";
			public const String TARGET_TEXT_TARGET_NAME_IS_EMPTY = "Target Text - Target Name is Empty";
			public const String TARGET_TEXT_NUMBER_OF_CHARACTERS_IS_EMPTY = "Target Text - Number of Characters is Empty";
			public const String TARGET_TEXT_CASE_SENSITIVE_IS_EMPTY = "Target Text - Case Sensitive is Empty";
			public const String TARGET_TEXT_DIRECTION_IS_EMPTY = "Target Text - Direction is Empty";
			public const String TARGET_TEXT_MARKER_TYPE_IS_EMPTY = "Target Text - Marker Type is Empty";
			public const String TARGET_TEXT_TRIM_STYLE_IS_EMPTY = "Target Text - Trim Style is Empty";
			public const String TARGET_TEXT_ARTIFACT_ID_CANNOT_BE_LESS_THAN_OR_EQUAL_TO_ZERO = "Target Text - ArtifactID cannot be less than 1";
			public const String REGULAR_EXPRESSION_NAME_IS_EMPTY = "Regular Expression - Name is Empty";
			public const String REGULAR_EXPRESSION_REGULAR_EXPRESSION_IS_EMPTY = "Regular Expression - Regular Expression is Empty";
			public const String REGULAR_EXPRESSION_DESCRIPTION_IS_EMPTY = "Regular Expression - Description is Empty";
			public const String REGULAR_EXPRESSION_ARTIFACT_ID_CANNOT_BE_LESS_THAN_OR_EQUAL_TO_ZERO = "Regular Expression - ArtifactID cannot be less than 1";
			public const String REGULAR_EXPRESSION_RECORD_DEPENDENCY = "At least one Extractor Regular Expression record is currently associated to one or more Extractor Target Texts and cannot be deleted.";
			public const String EXTRACTOR_PROFILE_ARTIFACT_ID_CANNOT_BE_LESS_THAN_OR_EQUAL_TO_ZERO = "Extractor Profile - ArtifactID cannot be less than 1";
			public const String EXTRACTOR_PROFILE_PROFILE_NAME_IS_EMPTY = "Extractor Profile - Profile Name is Empty";
			public const String EXTRACTOR_PROFILE_TARGET_TEXT_IS_EMPTY = "Extractor Profile - Target Text is Empty";
			public const String REQUIRED_FIELDS_ARE_MISSING = "At least one of the required fields for the associated Extractor Profile or Target Text is missing.";
		}

		public class TextExtractorDetailsMessages
		{
			public const String TRUNCATED = "Field ({0}) has been truncated";
		}

		public class SqlTableColumns
		{
			public class WorkerQueue
			{
				public const String QUEUE_ID = "QueueID";
				public const String WORKSPACE_ARTIFACT_ID = "WorkspaceArtifactID";
				public const String QUEUE_STATUS = "QueueStatus";
				public const String AGENT_ID = "AgentID";
				public const String EXTRACTOR_SET_ARTIFACT_ID = "ExtractorSetArtifactID";
				public const String DOCUMENT_ARTIFACT_ID = "DocumentArtifactID";
				public const String EXTRACTOR_PROFILE_ARTIFACT_ID = "ExtractorProfileArtifactID";
				public const String SOURCE_LONG_TEXT_FIELD_ARTIFACT_ID = "SourceLongTextFieldArtifactID";

				public static readonly List<String> ColumnList = new List<String> { QUEUE_ID, WORKSPACE_ARTIFACT_ID, QUEUE_STATUS, AGENT_ID, EXTRACTOR_SET_ARTIFACT_ID, DOCUMENT_ARTIFACT_ID, EXTRACTOR_PROFILE_ARTIFACT_ID, SOURCE_LONG_TEXT_FIELD_ARTIFACT_ID };
			}
		}

		public enum TrimStyleEnum
		{
			None,
			Left,
			Right,
			LeftAndRight,
		}

		public enum DirectionEnum
		{
			Right,
			Left,
            LeftAndRight,
            RightToStopMarker
		}

        public enum MarkerEnum
        {
            PlainText,
            RegEx
        }

		public class Choices
		{
			public class TrimStyle
			{
				public const String NONE = "None";
				public const String LEFT = "Left";
				public const String RIGHT = "Right";
				public const String LEFT_AND_RIGHT = "Left And Right";
			}

			public class Direction
			{
				public const String LEFT = "Left";
				public const String RIGHT = "Right";
				public const String LEFT_AND_RIGHT = "Left And Right";
			}

			public class MarkerType
			{
				public const String REGULAR_EXPRESSION = "Regular Expression";
				public const String PLAIN_TEXT = "Plain Text";
			}
		}
	}
}