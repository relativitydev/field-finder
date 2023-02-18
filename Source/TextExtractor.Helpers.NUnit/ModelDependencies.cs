using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit
{
	public class ModelDependencies : ADependency
	{
		public ModelDependencies()
		{
			// Data Generators
			Add(new RandomGenerator());

			// Relativity Access Fakes 
			Add(new FakeHelper());
			Add(new SqlQueryHelperDependency());
			Add(new ArtifactQueriesDependency());

			// Create dependencies 
			var errorQueueDependency = new ErrorQueueDependency();
			var managerJobDependency = new ManagerJobDependency();
			var managerQueueDependency = new ManagerQueueDependency();
			var workerQueueDependency = new WorkerQueueRecordDependency();
			var documentBatchDependency = new DocumentBatchDependency();
			var extractorSetReportingDependency = new ExtractorSetReportingDependency();
			var extractorSetDocumentDependency = new TextExtractorDocumentDependency();
			var extractorTargetRuleDependency = new ExtractorTargetRuleDependency();
			var extractorTargetTextDependency = new ExtractorTargetTextDependency();
			var extractorProfileDependency = new ExtractorProfileDependency();
			var extractorSetDependency = new ExtractorSetDependency();
			var artifactFactoryDependency = new ArtifactFactoryDependency();

			// Add dependencies 
			Add(errorQueueDependency);
			Add(managerJobDependency);
			Add(managerQueueDependency);
			Add(workerQueueDependency);
			Add(documentBatchDependency);
			Add(extractorSetDependency);
			Add(extractorSetReportingDependency);
			Add(extractorSetDocumentDependency);
			Add(extractorTargetRuleDependency);
			Add(extractorTargetTextDependency);
			Add(extractorProfileDependency);
			Add(extractorSetDependency);
			Add(artifactFactoryDependency);

			// Instantiate dependencies
			// Necessary to separate from constructor because SharedExecute() depends on RandomGenerator
			errorQueueDependency.SharedExecute();
			extractorSetDocumentDependency.SharedExecute();
			extractorTargetTextDependency.SharedExecute();
			managerJobDependency.SharedExecute();
			managerQueueDependency.SharedExecute();
			workerQueueDependency.SharedExecute();
			documentBatchDependency.SharedExecute();
			extractorSetDependency.SharedExecute();
			extractorSetReportingDependency.SharedExecute();
			extractorTargetRuleDependency.SharedExecute();
			extractorProfileDependency.SharedExecute();
			extractorSetDependency.SharedExecute();
			artifactFactoryDependency.SharedExecute();
		}
	}
}