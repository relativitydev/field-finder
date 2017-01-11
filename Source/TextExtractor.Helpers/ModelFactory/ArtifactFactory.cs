using System;
using Relativity.API;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.Helpers.ModelFactory
{
	public class ArtifactFactory : IArtifactFactory
	{
		private readonly IArtifactQueries ArtifactQueries;
		private readonly IServicesMgr ServicesMgr;
		private readonly ErrorLogModel ErrorLogModel;

		public ArtifactFactory(IArtifactQueries artifactQueries, IServicesMgr servicesMgr, ErrorLogModel errorLogModel)
		{
			ArtifactQueries = artifactQueries;
			ServicesMgr = servicesMgr;
			ErrorLogModel = errorLogModel;
		}

		public ExtractorSet GetInstanceOfExtractorSet(ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId)
		{
			var extractorSetRdo = ArtifactQueries.GetExtractorSetRdo(ServicesMgr, identity, workspaceArtifactId, extractorSetArtifactId);

			var set = new ExtractorSet(ArtifactQueries, ServicesMgr, ExecutionIdentity.CurrentUser, workspaceArtifactId, new ExtractorSetReporting(ArtifactQueries, ServicesMgr), extractorSetRdo);

			return set;
		}

		public ExtractorProfile GetInstanceOfExtractorProfile(ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorProfileArtifactId)
		{
			var extractorProfileRdo = ArtifactQueries.GetExtractorProfileRdo(ServicesMgr, identity, workspaceArtifactId, extractorProfileArtifactId);

			var profile = new ExtractorProfile(this, workspaceArtifactId, extractorProfileRdo, ErrorLogModel);

			return profile;
		}

		public ExtractorTargetText GetInstanceOfExtractorTargetText(ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorTargetTextArtifactId)
		{
			var extractorTargetTextRdo = ArtifactQueries.GetExtractorTargetTextRdo(ServicesMgr, identity, workspaceArtifactId, extractorTargetTextArtifactId);

			var target = new ExtractorTargetText(ArtifactQueries, ServicesMgr, identity, workspaceArtifactId, extractorTargetTextRdo, ErrorLogModel);

			return target;
		}

		public ExtractorRegularExpression GetInstanceOfExtractorRegularExpression(ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorRegularExpressionArtifactId)
		{
			var extractorRegullarExpressionRdo = ArtifactQueries.GetExtractorRegularExpressionRdo(ServicesMgr, identity, workspaceArtifactId, extractorRegularExpressionArtifactId);

			var regEx = new ExtractorRegularExpression(ArtifactQueries, workspaceArtifactId, extractorRegullarExpressionRdo, ErrorLogModel);

			return regEx;
		}
	}
}