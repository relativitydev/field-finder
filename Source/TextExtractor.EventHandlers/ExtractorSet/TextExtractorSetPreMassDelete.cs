using System;
using System.Runtime.InteropServices;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using TextExtractor.Helpers;

namespace TextExtractor.EventHandlers.ExtractorSet
{
	[Description("This mass operation Event Handler will attempt to delete all selected Extractor Set records that have not been submitted for processing.")]
	[Guid("9862DD0C-8DFB-4014-971E-907FDC2EF9FC")]
	public class TextExtractorSetPreMassDelete : PreMassDeleteEventHandler
	{
		public override Response Execute()
		{
			var response = new Response() { Success = true, Message = "" };

			try
			{
				var sqlQueryHelper = new SqlQueryHelper();
				var workspaceArtifactId = Helper.GetActiveCaseID();
				var eddsDbContext = Helper.GetDBContext(-1);
				var workspaceDbContext = Helper.GetDBContext(workspaceArtifactId);
				var tempTableName = TempTableNameWithParentArtifactsToDelete;
				var textExtractorSetJob = new TextExtractorSetJob(eddsDbContext, workspaceDbContext, sqlQueryHelper, tempTableName);

				response = textExtractorSetJob.ExecutePreMassDelete();
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Exception = new SystemException("Pre Mass Delete Failure: " + ex.Message);
			}

			return response;
		}

		public override FieldCollection RequiredFields
		{
			get { return null; }
		}

		public override void Rollback() { }

		public override void Commit() { }
	}
}
