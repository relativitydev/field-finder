using System;
using Relativity.API;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.Helpers.Models
{
	/// <summary>
	///   Represents a single Job History RDO in Relativity
	/// </summary>
	public class ExtractorSetHistory
	{
		public String TargetName { get; private set; }
		public String Name { get; private set; }
		public Int32 DocumentArtifactId { get; private set; }
		public Int32 DestinationFieldArtifactId { get; private set; }
		public Int32 ExtractorSetArtifactId { get; private set; }
		public Int32 WorkspaceArtifactId { get; private set; }
		public String StartMarker { get; private set; }
		public String StopMarker { get; private set; }
		public String MarkerType { get; private set; }
		public String Status { get; private set; }
		public String Details { get; private set; }

		private readonly IArtifactQueries ArtifactQueries;
		private readonly IServicesMgr ServicesMgr;
		private readonly ExecutionIdentity ExecutionIdentity;

		public ExtractorSetHistory(IServicesMgr servicesMgr, ExecutionIdentity executionIdentity, IArtifactQueries artifactQueries, Int32 extractorSetArtifactId, Int32 documentArtifactId, Int32 destinationFieldArtifactId, Int32 workspaceArtifactId, String targetName, String startMarker, String stopMarker, String markerType, string status = "", string details = "")
		{
			ServicesMgr = servicesMgr;
			ExecutionIdentity = executionIdentity;
			Name = Guid.NewGuid().ToString();
			ExtractorSetArtifactId = extractorSetArtifactId;
			DocumentArtifactId = documentArtifactId;
			DestinationFieldArtifactId = destinationFieldArtifactId;
			WorkspaceArtifactId = workspaceArtifactId;
			TargetName = targetName;
			StartMarker = startMarker;
			StopMarker = stopMarker;
			MarkerType = markerType;
			Status = status;
			Details = details;
			ArtifactQueries = artifactQueries;
		}

		/// <summary>
		///   Inserts this history record
		/// </summary>
		public void CreateRecord(String status = "", String errorMessage = "")
		{
			ArtifactQueries.CreateExtractorSetHistoryRecord(ServicesMgr, ExecutionIdentity, Name, WorkspaceArtifactId, ExtractorSetArtifactId, DocumentArtifactId, DestinationFieldArtifactId, status, errorMessage, TargetName, StartMarker, StopMarker, MarkerType);
		}
	}
}