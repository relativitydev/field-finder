using System;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using TextExtractor.Helpers.Rsapi;
using System.Collections.Generic;

namespace TextExtractor.Helpers.Models
{
	/// <summary>
	/// Represents a single job from the Relativity RDO 
	/// </summary>
	public class ExtractorSet
	{
		public String SetName { get; private set; }
		public Int32 ArtifactId { get; private set; }
		public Int32 SavedSearchArtifactId { get; private set; }
		public Int32 ExtractorProfileArtifactId { get; private set; }
		public Int32 SourceLongTextFieldArtifactId { get; private set; }
		public ExtractorSetReporting ExtractorSetReporting { get; private set; }
		public String Details { get; private set; }
		public String Status { get; private set; }
		public Boolean Exists { get; private set; }
		public String[] EmailRecepients { get; private set; }

		private Boolean cancelled;
		private readonly Int32 WorkspaceArtifactId;
		private readonly ExecutionIdentity ExecutionIdentity;
		private readonly IArtifactQueries ArtifactQueries;
		private readonly IServicesMgr ServicesMgr;

		public ExtractorSet(IArtifactQueries artifactQueries, IServicesMgr servicesMgr, ExecutionIdentity executionIdentity, Int32 workspaceArtifactId, ExtractorSetReporting extractorSetReporting, RDO extractorSetRdo)
		{
			ServicesMgr = servicesMgr;
			ExtractorSetReporting = extractorSetReporting;
			ArtifactQueries = artifactQueries;
			WorkspaceArtifactId = workspaceArtifactId;
			ExecutionIdentity = executionIdentity;

			SetMyProperties(extractorSetRdo);
		}

		/// <summary>
		/// Updates the status of this extractor set 
		/// </summary>
		/// <param name="status"></param>
		public void UpdateStatus(String status)
		{
			Status = status;
			ArtifactQueries.UpdateExtractorSetStatus(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, ArtifactId, status);
		}

		/// <summary>
		/// Updates the details of this extractor set 
		/// </summary>
		/// <param name="details"></param>
		public void UpdateDetails(String details)
		{
			Details = details;
			ArtifactQueries.UpdateExtractorSetDetails(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, ArtifactId, details);
		}

		/// <summary>
		/// Checks this job in Relativity to determine if it's been cancelled 
		/// </summary>
		/// <returns></returns>
		public Boolean IsCancellationRequested()
		{
			// Don't bother querying again if it's been cancelled
			if (cancelled) { return true; }

			try
			{
				var status = ArtifactQueries.GetExtractorSetStatus(ServicesMgr, ExecutionIdentity, WorkspaceArtifactId, ArtifactId);

				if (status == Constant.ExtractorSetStatus.CANCELLED)
				{
					cancelled = true;
				}

				return cancelled;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when checking for job cancellation.", ex);
			}
		}

		private void SetMyProperties(RDO extractorSetRdo)
		{
			if (extractorSetRdo == null)
			{
				Exists = false;
				return;
			}

			try
			{
				var savedSearchArtifact = extractorSetRdo.Fields.Get(Constant.Guids.Fields.ExtractorSet.SavedSearch).ValueAsSingleObject;
				var extractorProfileArtifact = extractorSetRdo.Fields.Get(Constant.Guids.Fields.ExtractorSet.ExtractorProfile).ValueAsSingleObject;
				var sourceLongTextFieldArtifact = extractorSetRdo.Fields.Get(Constant.Guids.Fields.ExtractorSet.SourceLongTextField).ValueAsSingleObject;

				String emailRecepients = extractorSetRdo.Fields.Get(Constant.Guids.Fields.ExtractorSet.EmailNotificationRecipients).ValueAsLongText;
				EmailRecepients = (String.IsNullOrWhiteSpace(emailRecepients) == true) ? null : emailRecepients.Split(';');

				SetName = extractorSetRdo.Fields.Get(Constant.Guids.Fields.ExtractorSet.SetName).ValueAsFixedLengthText;
				ArtifactId = extractorSetRdo.ArtifactID;
				Status = extractorSetRdo.Fields.Get(Constant.Guids.Fields.ExtractorSet.Status).ValueAsFixedLengthText;
				SavedSearchArtifactId = savedSearchArtifact.ArtifactID;
				ExtractorProfileArtifactId = extractorProfileArtifact.ArtifactID;
				SourceLongTextFieldArtifactId = sourceLongTextFieldArtifact.ArtifactID;
				Exists = true;
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.DEFAULT_CONVERT_TO_EXTRACTOR_SET_ERROR_MESSAGE + ". " + ExceptionMessageFormatter.GetInnerMostExceptionMessage(ex), ex);
			}
		}
	}
}