using System.Runtime.InteropServices;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using TextExtractor.Helpers;

namespace TextExtractor.EventHandlers.ExtractorProfile
{
	[Description("This Event Handler will verify that the current Extractor Profile is not currently in either the Manager or Worker queue tables.")]
	[Guid("2EC89462-98EA-4D79-8739-36B6C4A69D72")]
	public class TextExtractorProfilePreSave : PreSaveEventHandler
	{
		public override Response Execute()
		{
			var response = new Response() { Message = string.Empty, Success = true };
			var layoutArtifactIdByGuid = GetArtifactIdByGuid(Constant.Guids.Layout.ExtractorProfile);
			var layoutArtifactId = ActiveLayout.ArtifactID;
			var validator = new Validator();

			//check if this is the Text Extractor Profile layout
			if (!validator.VerifyIfNotLayout(layoutArtifactIdByGuid, layoutArtifactId))
			{
				//check if this is a new Text Extractor Profile record
				if (!ActiveArtifact.IsNew)
				{
					var sqlQueryHelper = new SqlQueryHelper();
					var workspaceDbContext = Helper.GetDBContext(-1);
					var activeArtifactId = ActiveArtifact.ArtifactID;
					var textExtractorProfileJob = new TextExtractorProfileJob(workspaceDbContext, sqlQueryHelper, activeArtifactId);

					response = textExtractorProfileJob.ExecutePreSave();
				}
			}

			return response;
		}

		public override FieldCollection RequiredFields
		{
			get
			{
				var retVal = new FieldCollection();
				return retVal;
			}
		}
	}
}
