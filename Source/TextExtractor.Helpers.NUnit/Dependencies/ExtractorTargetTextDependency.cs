using System;

using Moq;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	namespace TextExtractor.Helpers.NUnit.Dependencies
	{
		public class ExtractorTargetTextDependency : ADependency
		{
			public ExtractorTargetText ExtractorTargetText;

			public Mock<IExtractorTargetText> MockTarget;

			public ExtractorTargetTextDependency()
			{
				Add(new ExtractorTargetRuleDependency());
			}

			public override void SharedExecute()
			{
				var random = Pull<RandomGenerator>();
				var mgr = Pull<FakeServicesMgr>().ServicesMgr;
				var query = Pull<ArtifactQueriesDependency>().Queries;
				var errorQueue = Pull<ErrorQueueDependency>().ErrorLogModel;
				var extractorTargetTextRdo = Pull<ArtifactQueriesReturns>().ExtractorTargetTextRdo;

				ExtractorTargetText = new ExtractorTargetText(
					query,
					mgr,
					ExecutionIdentity.CurrentUser,
					random.Number(),
					extractorTargetTextRdo, errorQueue);

				MockTarget = new Mock<IExtractorTargetText>();

				MakeProcessFieldVerifiable();
			}

			#region Standard

			private void MakeProcessFieldVerifiable()
			{
				MockTarget.Setup(target => target.Process(It.IsAny<ExtractorSetDocument>(), It.IsAny<ExtractorSet>()))
					.Verifiable();
			}

			#endregion Standard

			public void VerifyProcessWasNeverCalled()
			{
				MockTarget.Verify(
					target => target.Process(It.IsAny<ExtractorSetDocument>(), It.IsAny<ExtractorSet>()),
					Times.Never());
			}
		}

		/// <summary>
		/// To get around an issue with casting a mock object 
		/// </summary>
		public class FakeExtractorTargetText : ExtractorTargetText
		{
			public override Boolean Process(ExtractorSetDocument extractorSetDocument, ExtractorSet extractorSet)
			{
				return false;
			}
		}
	}
}