using System;
using System.Runtime.InteropServices;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using TextExtractor.EventHandlers.ExtractorSet;

namespace TextExtractor.EventHandlers.ExtractorTargetText
{
	[Description("This Event Handler will check for Extractor Target Text dependencies prior to deletion.")]
	[Guid("39DE5EB6-7EE4-42B7-91B7-791E84284B55")]
	public class TextExtractorTargetTextPreCascadeDelete : PreCascadeDeleteEventHandler
	{
		public override Response Execute()
		{

			var response = new Response() { Success = true, Message = "" };

			try
			{
				var textExtractorTargetTextJob = new TextExtractorTargetTextJob();
				response = textExtractorTargetTextJob.ExecutePreCascadeDelete();
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
