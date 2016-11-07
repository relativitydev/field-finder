using System;
using System.Runtime.InteropServices;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using TextExtractor.Helpers;

namespace TextExtractor.EventHandlers.ExtractorSet
{
	[Description("This Event Handler will verifiy that the current Extractor Set cannot be deleted if already Submitted.")]
	[Guid("7AB673E3-FF38-45EE-B764-808F84C45DBB")]
	public class TextExtractorSetPreDelete : PreDeleteEventHandler
	{
		public override Response Execute()
		{
			var response = new Response() { Success = true, Message = "" };

			try
			{
				var sqlQueryHelper = new SqlQueryHelper();
				var workspaceArtifactId = Helper.GetActiveCaseID();
				var workspaceDbContext = Helper.GetDBContext(workspaceArtifactId);
				var activeArtifactId = ActiveArtifact.ArtifactID;

				var textExtractorSetJob = new TextExtractorSetJob(workspaceDbContext, sqlQueryHelper, activeArtifactId);
				response = textExtractorSetJob.ExecutePreDelete();
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Exception = new SystemException("Pre Delete Failure: " + ex.Message);
			}

			return response;
		}

		public override FieldCollection RequiredFields
		{
			get { return new FieldCollection(); }
		}

		public override void Rollback() { }

		public override void Commit() { }
	}
}
