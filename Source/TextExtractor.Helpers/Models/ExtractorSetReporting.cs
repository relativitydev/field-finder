using System;

using kCura.Relativity.Client.DTOs;
using Relativity.API;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.Helpers.Models
{
	public class ExtractorSetReporting
	{
		private readonly IArtifactQueries ArtifactQueries;
		public Int32 NumberOfUpdatesWithValues;
		private readonly IServicesMgr ServicesMgr;
		public Int32 TotalExpectedUpdates;

		public ExtractorSetReporting(IArtifactQueries artifactQueries, IServicesMgr servicesMgr)
		{
			ArtifactQueries = artifactQueries;
			ServicesMgr = servicesMgr;
		}

		/// <summary>
		///   Will update the report section on the Job RDO with the document count multiplied by the field count
		/// </summary>
		/// <param name="workspaceArtifactId"></param>
		/// <param name="extractorSetArtifactId"></param>
		/// <param name="documentCount"></param>
		/// <param name="fieldCount"></param>
		public void UpdateTotalExpectedUpdates(Int32 workspaceArtifactId, Int32 extractorSetArtifactId, Int32 documentCount, Int32 fieldCount)
		{
			if (documentCount > 0 && fieldCount > 0)
			{
				TotalExpectedUpdates = documentCount * fieldCount;

				ArtifactQueries.UpdateTotalExpectedUpdates(ServicesMgr, ExecutionIdentity.CurrentUser, workspaceArtifactId, extractorSetArtifactId, TotalExpectedUpdates);
			}
			else
			{
				// Will not update if the counts aren't accurate
				TotalExpectedUpdates = 0;
			}
		}

		public void SetNumberOfUpdatesWithValues(Int32 workspaceArtifactId, Int32 extractorSetArtifactId, Int32 value)
		{
			// If there's no update, don't bother querying R*
			if (value == 0) { return; }

			// Get the updates currently set
			var jobRdo = ArtifactQueries.GetExtractorSetRdo(ServicesMgr, ExecutionIdentity.CurrentUser, workspaceArtifactId, extractorSetArtifactId);
			var currentValue = jobRdo.Fields.Get(Constant.Guids.Fields.ExtractorSet.NumberOfUpdatesWithValues).ValueAsWholeNumber.GetValueOrDefault(0);

			// Add newest value
			Int32 newValue = currentValue + value;

			// Update set with new value
			ArtifactQueries.UpdateNumberOfUpdatesWithValues(ServicesMgr, ExecutionIdentity.CurrentUser, workspaceArtifactId, extractorSetArtifactId, newValue);
		}
	}
}