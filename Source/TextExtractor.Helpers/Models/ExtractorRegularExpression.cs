using System;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using TextExtractor.Helpers.Rsapi;
using TextExtractor.Helpers.ModelFactory;

namespace TextExtractor.Helpers.Models
{
	public class ExtractorRegularExpression
	{
		public String Name { get; set; }
		public Int32 ArtifactId { get; private set; }
		public String RegularExpression { get; set; }
		public String Description { get; set; }

		private readonly IArtifactQueries ArtifactQueries;
		private readonly Int32 WorkspaceArtifactId;
		private ErrorLogModel ErrorLogModel { get; set; }

		public ExtractorRegularExpression(IArtifactQueries artifactQueries, Int32 workspaceArtifactId, RDO extractorRegullarExpressionRdo, ErrorLogModel errorLogModel)
		{
			WorkspaceArtifactId = workspaceArtifactId;
			ArtifactQueries = artifactQueries;
			ErrorLogModel = errorLogModel;

			SetMyProperties(extractorRegullarExpressionRdo);
		}

		public ExtractorRegularExpression() { }

		public void SetMyProperties(RDO extractorRegullarExpressionRdo)
		{
			try
			{
				ArtifactId = extractorRegullarExpressionRdo.ArtifactID;
				if (ArtifactId < 1)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.REGULAR_EXPRESSION_ARTIFACT_ID_CANNOT_BE_LESS_THAN_OR_EQUAL_TO_ZERO);
				}

				Name = extractorRegullarExpressionRdo.Fields.Get(Constant.Guids.Fields.ExtractorRegularExpression.Name).ValueAsFixedLengthText;
				if (Name == null)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.REGULAR_EXPRESSION_NAME_IS_EMPTY);
				}

				RegularExpression = extractorRegullarExpressionRdo.Fields.Get(Constant.Guids.Fields.ExtractorRegularExpression.RegularExpression).ValueAsFixedLengthText;
				if (RegularExpression == null)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.REGULAR_EXPRESSION_REGULAR_EXPRESSION_IS_EMPTY);
				}

				Description = extractorRegullarExpressionRdo.Fields.Get(Constant.Guids.Fields.ExtractorRegularExpression.Description).ValueAsFixedLengthText;
				if (Description == null)
				{
					throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.REGULAR_EXPRESSION_DESCRIPTION_IS_EMPTY);
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.DEFAULT_CONVERT_TO_EXTRACTOR_REGULAR_EXPRESSION_ERROR_MESSAGE + ". " + ExceptionMessageFormatter.GetInnerMostExceptionMessage(ex), ex);
			}
		}
	}
}
