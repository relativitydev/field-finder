using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Dependencies.TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class ExtractorProfileDependency : ADependency
	{
		public ExtractorProfile ExtractorProfile;

		public ExtractorProfileDependency()
		{
			Add(new ExtractorTargetTextDependency());
		}

		public override void SharedExecute()
		{
			var random = Pull<RandomGenerator>();
			var factory = Pull<ArtifactFactoryDependency>().MockArtifactFactory.Object;
			var profileRdo = Pull<ArtifactQueriesReturns>().ExtractorProfileRdo;

			ExtractorProfile = new ExtractorProfile(factory, random.Number(), profileRdo, null);
		}
	}
}