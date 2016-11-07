using System;
using System.Runtime.InteropServices;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;

namespace TextExtractor.EventHandlers.ExtractorSet
{
	[Description("This Event Handler will check for Extractor Set History dependencies prior to deletion.")]
	[Guid("77A0F45A-789C-450E-9A7B-0425CF2AB1BC")]
	public class TextExtractorSetPreCascadeDelete : PreCascadeDeleteEventHandler
	{
		public override Response Execute()
		{
			var response = new Response() { Success = true, Message = "" };

			try
			{
				var textExtractorSetJob = new TextExtractorSetJob();
				response = textExtractorSetJob.ExecutePreCascadeDelete();
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

		public override void Rollback() {}

		public override void Commit() { }
	}
}
