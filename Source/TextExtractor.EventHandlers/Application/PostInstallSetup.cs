using System;
using TextExtractor.Helpers;

namespace TextExtractor.EventHandlers.Application
{
	[kCura.EventHandler.CustomAttributes.RunOnce(false)]
	[kCura.EventHandler.CustomAttributes.Description("Creates the underlying tables for the application.")]
	[System.Runtime.InteropServices.Guid("968f9710-a2bb-46fd-bf40-bc07390dee51")]
	class PostInstallSetup : kCura.EventHandler.PostInstallEventHandler
	{
		public override kCura.EventHandler.Response Execute()
		{
			var response = new kCura.EventHandler.Response { Success = true, Message = String.Empty };
			var queryHelper = new SqlQueryHelper();

			try
			{
				//Create manager queue table if it doesn't already exist
				queryHelper.CreateManagerQueueTable(Helper.GetDBContext(-1));

				//Create worker queue table if it doesn't already exist
				queryHelper.CreateWorkerQueueTable(Helper.GetDBContext(-1));

				//Create error log table if it doesn't already exist
				queryHelper.CreateErrorLogTable(Helper.GetDBContext(-1));
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Message = "Post-Install queue table creation failed with message: " + ex;
			}
			return response;
		}
	}
}
