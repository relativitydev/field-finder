using System;

using kCura.Relativity.Client.DTOs;
using Moq;
using Relativity.API;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.Rsapi;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies.Seams
{
	public class ArtifactQueriesDependency : ADependency
	{
		public IArtifactQueries Queries;

		public Mock<IArtifactQueries> MockQueries;

		private ArtifactQueriesReturns Returns;
		private RandomGenerator Random;

		public ArtifactQueriesDependency()
		{
			Random = new RandomGenerator();
			Returns = new ArtifactQueriesReturns();
			Add(Random);
			Add(Returns);
			SharedExecute();
		}

		public override void SharedExecute()
		{

			MockQueries = new Mock<IArtifactQueries>();

			// Standard Stubs
			WhenThereAreExtractorSetDetails();
			WhenThereAreExtractorTargetRdos();
			WhenThereIsAnExtractorSetRdo();
			WhenThereisAnExtractorProfileRdo();
			WhenThereAreDocumentsInTheSavedSearch();
			WhenThereIsADocumentTextFieldValue();
			WhenTheExtractorSetIsSubmitted();

			Queries = MockQueries.Object;
		}

		#region Standard

		private void WhenThereAreExtractorSetDetails()
		{
			MockQueries.Setup(
				query =>
					query.GetExtractorSetDetails(
						It.IsAny<IServicesMgr>(),
						It.IsAny<ExecutionIdentity>(),
						It.IsAny<int>(),
						It.IsAny<int>()
					)).Returns(Random.Word);
		}

		private void WhenThereAreExtractorTargetRdos()
		{
			// Populate the number of fields on the Template; used to return different
			// Fields when querying by field, since the Worker queries both Template
			// and then individual field (not sure why)
			var maxFields =
				Returns.ExtractorProfileRdo.Fields
					.Get(Constant.Guids.Fields.ExtractorProfile.TargetText)
					.GetValueAsMultipleObject<Artifact>()
					.Count;

			MockQueries.Setup(
				query =>
				query.GetExtractorTargetTextRdo(
					It.IsAny<IServicesMgr>(),
					It.IsAny<ExecutionIdentity>(),
					It.IsAny<int>(),
					It.IsAny<int>()
					)).Returns(() =>
						(RDO)Returns.ExtractorProfileRdo.Fields
						.Get(Constant.Guids.Fields.ExtractorProfile.TargetText)
						.GetValueAsMultipleObject<Artifact>()[Random.Number(maxSize: maxFields)]);
		}

		private void WhenThereIsAnExtractorSetRdo()
		{
			MockQueries.Setup(
				query =>
				query.GetExtractorSetRdo(
					It.IsAny<IServicesMgr>(),
					It.IsAny<ExecutionIdentity>(),
					It.IsAny<int>(),
					It.IsAny<int>()
					)).Returns(Returns.ExtractorSetRdo);
		}

		private void WhenThereisAnExtractorProfileRdo()
		{
			MockQueries.Setup(
				query =>
				query.GetExtractorProfileRdo(
					It.IsAny<IServicesMgr>(),
					It.IsAny<ExecutionIdentity>(),
					It.IsAny<int>(),
					It.IsAny<int>()
					)).Returns(Returns.ExtractorProfileRdo);
		}

		private void WhenThereAreDocumentsInTheSavedSearch()
		{
			MockQueries.Setup(
				query =>
				query.GetFirstBatchOfDocuments(
					It.IsAny<IServicesMgr>(),
					It.IsAny<ExecutionIdentity>(),
					It.IsAny<int>(),
					It.IsAny<Query<Document>>(),
					It.IsAny<int>()
					)).Returns(Returns.FirstBatchOfDocuments).Verifiable();

			MockQueries.Setup(
				query =>
				query.GetSubsequentBatchOfDocuments(
					It.IsAny<IServicesMgr>(),
					It.IsAny<ExecutionIdentity>(),
					It.IsAny<int>(),
					It.IsAny<int>(),
					It.IsAny<string>(),
					It.IsAny<Query<Document>>(),
					It.IsAny<int>()
					)).Returns(Returns.FirstBatchOfDocuments);
		}

		private void WhenThereIsADocumentTextFieldValue()
		{
			MockQueries.Setup(
				query =>
				query.GetDocumentTextFieldValue(
					It.IsAny<IServicesMgr>(),
					It.IsAny<ExecutionIdentity>(),
					It.IsAny<int>(),
					It.IsAny<int>(),
					It.IsAny<int>()
					)).Returns(Returns.DocumentTextFieldValue);
		}

		private void WhenTheExtractorSetIsSubmitted()
		{
			MockQueries.Setup(
				query =>
				query.GetExtractorSetStatus(
					It.IsAny<IServicesMgr>(),
					It.IsAny<ExecutionIdentity>(),
					It.IsAny<int>(),
					It.IsAny<int>()
					)).Returns(Constant.ExtractorSetStatus.SUBMITTED);
		}

		#endregion Standard

		#region Edge Cases

		public void WhenThereIsNoExtractorSet()
		{
			MockQueries.Setup(
				query =>
				query.GetExtractorSetRdo(
					It.IsAny<IServicesMgr>(),
					It.IsAny<ExecutionIdentity>(),
					It.IsAny<int>(),
					It.IsAny<int>()
				)).Returns((RDO)null);
		}

		public void WhenTheExtractorSetsAreCancelled()
		{
			MockQueries.Setup(
				query =>
					query.GetExtractorSetStatus(
						It.IsAny<IServicesMgr>(),
						It.IsAny<ExecutionIdentity>(),
						It.IsAny<int>(),
						It.IsAny<int>()
						)).Returns(Constant.ExtractorSetStatus.CANCELLED);
		}

		public void WhenTheRsapiThrowsWhileRetrievingAnExtractorSet()
		{
			MockQueries.Setup(
				query =>
					query.GetExtractorSetRdo(
						It.IsAny<IServicesMgr>(),
						It.IsAny<ExecutionIdentity>(),
						It.IsAny<int>(),
						It.IsAny<int>()
					)).Throws<Exception>();
		}

		#endregion Edge Cases

	}
}
