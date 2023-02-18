using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.Helpers;

namespace TextExtractor.EventHandlers.ExtractorRegEx
{
	[Description("This Event Handler will check for Extractor Regular Expression dependencies prior to deletion.")]
	[System.Runtime.InteropServices.Guid("D13F6D95-0C4B-4445-ABB2-E51DE166AA19")]
	public class RegularExpressionPreCascadeDelete : PreCascadeDeleteEventHandler
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
				var regularExpressionJob = new RegularExpressionJob(workspaceDbContext, sqlQueryHelper, tempTableName);

				response = regularExpressionJob.ExecutePreCascadeDelete();
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
