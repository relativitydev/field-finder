using TextExtractor.Helpers.Models;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class WorkerQueueRecordDependency : ADependency
	{
		public ManagerQueueRecord WorkerRecord;

		public override void SharedExecute()
		{
			var random = Pull<RandomGenerator>();
		}
	}
}
