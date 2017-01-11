using System;
using kCura.Relativity.Client.DTOs;
using TextExtractor.Helpers.NUnit.Data;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns
{
	public class ArtifactQueriesReturns : ADependency
	{
		public QueryResultSet<Document> FirstBatchOfDocuments;
		public RDO ExtractorProfileRdo;
		public RDO ExtractorTargetTextRdo;
		public RDO ExtractorSetRdo;
        public RDO ExtractorRegularExpressionRdo;
		public String JobStatus;
		public String DocumentTextFieldValue;

		public Int32 NumberOfDocumentsInFirstBatch;
		public Int32 NumberOfFieldsOnTemplate;

		public ArtifactQueriesReturns()
		{
			NumberOfDocumentsInFirstBatch = 100;
			NumberOfFieldsOnTemplate = 25;
			Add(new ArtifactGenerator());
			Add(new RandomGenerator());
			SharedExecute();
		}

		public override void SharedExecute()
		{
			var random = Pull<RandomGenerator>();
			var generator = Pull<ArtifactGenerator>();

			FirstBatchOfDocuments = generator.GetDocumentResultSet(NumberOfDocumentsInFirstBatch);

			ExtractorProfileRdo = generator.GetExtractorProfileRdo(NumberOfFieldsOnTemplate);
			ExtractorTargetTextRdo = generator.GetExtractorTargetTextRdo();
			ExtractorSetRdo = generator.GetExtractorSetRdo();
            ExtractorRegularExpressionRdo = generator.GetExtractorRegularExpressionRdo();


			// Returns a random status
			object[] possibleStatuses =
				{
					Constant.ExtractorSetStatus.CANCELLED, 
					Constant.ExtractorSetStatus.ERROR, 
					Constant.ExtractorSetStatus.SUBMITTED
				};

			JobStatus = random.Randomize<String>(possibleStatuses);

			DocumentTextFieldValue = random.Paragraph();
		}
	}
}