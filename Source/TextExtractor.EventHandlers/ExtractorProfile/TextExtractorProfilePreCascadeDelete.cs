using System;
using System.Runtime.InteropServices;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using TextExtractor.Helpers;

namespace TextExtractor.EventHandlers.ExtractorProfile
{
	[Description("This Event Handler will check for Extractor Profile dependencies prior to deletion.")]
	[Guid("59E62333-BFF2-4E1D-A04B-0AD940C724E6")]
	public class TextExtractorProfilePreCascadeDelete : PreCascadeDeleteEventHandler
	{
		public override Response Execute()
		{
			var response = new Response() { Success = true, Message = "" };

			try
			{
				var sqlQueryHelper = new SqlQueryHelper();
				var workspaceArtifactId = Helper.GetActiveCaseID();
				var workspaceDbContext = Helper.GetDBContext(workspaceArtifactId);
				var tempTableName = TempTableNameWithParentArtifactsToDelete;
				var textExtractorProfileJob = new TextExtractorProfileJob(workspaceDbContext, sqlQueryHelper, tempTableName);

				response = textExtractorProfileJob.ExecutePreCascadeDelete();
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Exception = new SystemException("Pre Cascade Delete Failure: " + ex.Message);
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
