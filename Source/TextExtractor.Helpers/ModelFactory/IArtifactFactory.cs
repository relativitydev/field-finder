using System;
using Relativity.API;
using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers.ModelFactory
{
	public interface IArtifactFactory
	{
		ExtractorSet GetInstanceOfExtractorSet(ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId);

		ExtractorProfile GetInstanceOfExtractorProfile(ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorProfileArtifactId);

		ExtractorTargetText GetInstanceOfExtractorTargetText(ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorTargetTextArtifactId);

		ExtractorRegularExpression GetInstanceOfExtractorRegularExpression(ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorTargetTextArtifactId);
	}
}
