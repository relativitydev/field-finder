using System;
using kCura.EventHandler;
using Relativity.API;
using TextExtractor.EventHandlers.Interfaces;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.EventHandlers.ExtractorProfile
{
	public class TextExtractorProfileJob : ITextExtractorProfileJob
	{
		public IDBContext EddsDbContext { get; set; }
		public IDBContext DbContext { get; set; }
		public ISqlQueryHelper SqlQueryHelper { get; set; }
		public int ActiveArtifactId { get; set; }
		public string TempTableName { get; set; }

		public TextExtractorProfileJob(IDBContext eddsDbContext, ISqlQueryHelper sqlQueryHelper, int activeArtifactId)
		{
			EddsDbContext = eddsDbContext;
			SqlQueryHelper = sqlQueryHelper;
			ActiveArtifactId = activeArtifactId;
		}

		public TextExtractorProfileJob(IDBContext dbContext, ISqlQueryHelper sqlQueryHelper, String tempTableName)
		{
			DbContext = dbContext;
			SqlQueryHelper = sqlQueryHelper;
			TempTableName = tempTableName;
		}

		public Response ExecutePreSave()
		{
			var response = new Response
			{
				Success = true,
				Message = string.Empty
			};

			try
			{
				var jobCountManager = SqlQueryHelper.RetrieveExtractorProfileCountInQueue(EddsDbContext, ActiveArtifactId.ToString(), Constant.Tables.ManagerQueue);
				var jobCountWorker = SqlQueryHelper.RetrieveExtractorProfileCountInQueue(EddsDbContext, ActiveArtifactId.ToString(), Constant.Tables.WorkerQueue);

				if (jobCountManager > 0 || jobCountWorker > 0)
				{
					response.Success = false;
					response.Message = Constant.ErrorMessages.EXTRACTION_PROFILE_RECORD_DETECTED;
				}
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Message = string.Format("{0}, Error Message: {1}", Constant.ErrorMessages.DEFAULT_ERROR_PREPEND, ex);
			}

			return response;
		}

		public Response ExecutePreCascadeDelete()
		{
			var response = new Response { Success = true, Message = string.Empty };

			try
			{
				var setCount = SqlQueryHelper.RetrieveProfileCountFromSetTableByTempTable(DbContext, TempTableName);

				if (setCount > 0)
				{
					response.Success = false;
					response.Exception = new SystemException(Constant.ErrorMessages.EXTRACTION_PROFILE_RECORD_DEPENDENCY);
				}
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Exception  = new SystemException(string.Format("{0}, Error Message: {1}", Constant.ErrorMessages.DEFAULT_ERROR_PREPEND, ex));
			}

			return response;
		}
	}
}
