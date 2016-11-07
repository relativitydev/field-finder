using System.Runtime.InteropServices;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using TextExtractor.Helpers;

namespace TextExtractor.EventHandlers.ExtractorSet
{
	[Description("This Event Handler will verify that the current Extractor Set is not currently in either the Manager or Worker queue tables.")]
	[Guid("F8DCC19E-EC21-44C6-9C22-1C65C632963D")]
	public class TextExtractorSetPreSave : PreSaveEventHandler
	{
		public override Response Execute()
		{
			var response = new Response() { Message = string.Empty, Success = true };
			var layoutArtifactIdByGuid = GetArtifactIdByGuid(Constant.Guids.Layout.ExtractorSet);
			var layoutArtifactId = ActiveLayout.ArtifactID;
			var validator = new Validator();

			//check if this is the Text Extractor Set layout
			if (!validator.VerifyIfNotLayout(layoutArtifactIdByGuid, layoutArtifactId))
			{
				//check if this is a new Text Extractor Set record
				if (!ActiveArtifact.IsNew)
				{
					var sqlQueryHelper = new SqlQueryHelper();
					var eddsDbContext = Helper.GetDBContext(-1);
					var activeArtifactId = ActiveArtifact.ArtifactID;
					var textExtractorSetJob = new TextExtractorSetJob(eddsDbContext, sqlQueryHelper, activeArtifactId);

					response = textExtractorSetJob.ExecutePreSave();
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
