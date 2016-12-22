using System;

using Moq;

using Relativity.API;

using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.ModelFactory;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class ArtifactFactoryDependency : ADependency
	{
		public ArtifactFactory ArtifactFactory;

		public Mock<IArtifactFactory> MockArtifactFactory;

		public ArtifactFactoryDependency()
		{
			// This is to keep from breaking everything; not good design!
			MockArtifactFactory = new Mock<IArtifactFactory>();
			ArtifactFactory = new ArtifactFactory(null, null, null);
		}

		public override void SharedExecute()
		{
			var artQuery = Pull<ArtifactQueriesDependency>().Queries;
			var errorQueue = Pull<ErrorQueueDependency>().ErrorLogModel;
			var helper = Pull<FakeHelper>().Helper;

			ArtifactFactory = new ArtifactFactory(artQuery, helper.GetServicesManager(), errorQueue);

			var mock = new Mock<IArtifactFactory>();

			WhenThereIsAnExtractorSet();
			WhenThereIsAnExtractorProfile();
			WhenThereAreExtractorTargetTexts();
		}

		private void WhenThereIsAnExtractorSet()
		{
			var set = Pull<ExtractorSetDependency>().ExtractorSet;

			MockArtifactFactory.Setup(
				factory =>
					factory.GetInstanceOfExtractorSet(
						It.IsAny<ExecutionIdentity>(),
						It.IsAny<int>(),
						It.IsAny<int>()
					)).Returns(set);
		}

		private void WhenThereIsAnExtractorProfile()
		{
			var profile = Pull<ExtractorProfileDependency>().ExtractorProfile;

			MockArtifactFactory.Setup(
				factory =>
					factory.GetInstanceOfExtractorProfile(
						It.IsAny<ExecutionIdentity>(),
						It.IsAny<int>(),
						It.IsAny<int>()
					)).Returns(profile);
		}

		public void WhenThereAreExtractorTargetTexts()
		{
			var fakeTarget = new FakeExtractorTargetText();

			MockArtifactFactory.Setup(
				factory =>
					factory.GetInstanceOfExtractorTargetText(
						It.IsAny<ExecutionIdentity>(),
						It.IsAny<int>(),
						It.IsAny<int>()
					)).Returns(fakeTarget);
		}
	}
}