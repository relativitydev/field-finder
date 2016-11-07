using System;
using kCura.EventHandler;
using Relativity.API;
using TextExtractor.EventHandlers.Interfaces;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.EventHandlers.ExtractorSet
{
	public class TextExtractorSetJob : ITextExtractorSetJob
	{
		public IDBContext EddsDbContext { get; set; }
		public IDBContext EddsDBContext { get; set; }
		public ISqlQueryHelper SqlQueryHelper { get; set; }
		public int ActiveArtifactId { get; set; }
		public string TempTableName { get; set; }

		public TextExtractorSetJob(IDBContext eddsDBContext, ISqlQueryHelper sqlQueryHelper, int activeArtifactId)
		{
			EddsDBContext = eddsDBContext;
			SqlQueryHelper = sqlQueryHelper;
			ActiveArtifactId = activeArtifactId;
		}

		public TextExtractorSetJob(IDBContext eddsDbContext, IDBContext eddsDBContext, ISqlQueryHelper sqlQueryHelper, string tempTableName)
		{
			EddsDbContext = eddsDbContext;
			EddsDBContext = eddsDBContext;
			SqlQueryHelper = sqlQueryHelper;
			TempTableName = tempTableName;
		}

		public TextExtractorSetJob(IDBContext eddsDbContext, IDBContext eddsDBContext, ISqlQueryHelper sqlQueryHelper, int activeArtifactId)
		{
			EddsDbContext = eddsDbContext;
			EddsDBContext = eddsDBContext;
			SqlQueryHelper = sqlQueryHelper;
			ActiveArtifactId = activeArtifactId;
		}
	
		public TextExtractorSetJob()
		{}

		public Response ExecutePreSave()
		{
			var response = new Response { Success = true, Message = string.Empty };

			try
			{
				var jobCountManager = SqlQueryHelper.RetrieveJobCountInQueue(EddsDBContext, ActiveArtifactId.ToString(), Constant.Tables.ManagerQueue);
				var jobCountWorker = SqlQueryHelper.RetrieveJobCountInQueue(EddsDBContext, ActiveArtifactId.ToString(), Constant.Tables.WorkerQueue);

				if (jobCountManager > 0 || jobCountWorker > 0)
				{
					response.Success = false;
					response.Message = Constant.ErrorMessages.EXTRACTION_SET_RECORD_DETECTED;
				}
				
				return response;
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Message = string.Format("{0}, Error Message: {1}", Constant.ErrorMessages.DEFAULT_ERROR_PREPEND, ex);
				return response;
			}
		}

		public Response ExecutePreCascadeDelete()
		{
			var response = new Response { Success = false, Exception = new SystemException(Constant.ErrorMessages.EXTRACTION_SET_CANNOT_DELETE_CURRENT_RECORD_HISTORY) };
			return response;
		}

		public Response ExecutePreDelete()
		{
			var response = new Response { Success = true, Message = string.Empty };

			try
			{
				var setStatus = SqlQueryHelper.RetrieveExtractorSetStatusBySetArtifactId(EddsDBContext, ActiveArtifactId);
				if (!string.IsNullOrEmpty(setStatus))
				{
					response.Success = false;
					response.Exception = new SystemException(Constant.ErrorMessages.EXTRACTION_SET_CANNOT_DELETE_STATUS_NOT_NULL);
				}
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Exception = new SystemException(string.Format("{0}, Error Message: {1}", Constant.ErrorMessages.DEFAULT_ERROR_PREPEND, ex));
			}

			return response;
		}

		public Response ExecutePreMassDelete()
		{
			var response = new Response { Success = true, Message = string.Empty };

			try
			{

				var setStatusCount = SqlQueryHelper.RetrieveExtractorSetStatusCountByTempTable(EddsDBContext, TempTableName);

				if (setStatusCount > 0)
				{
					response.Success = false;
					response.Exception = new SystemException(Constant.ErrorMessages.EXTRACTION_SET_CANNOT_DELETE_MULTIPLE_RECORD);
					return response;
				}
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Exception = new SystemException(string.Format("{0}, Error Message: {1}", Constant.ErrorMessages.DEFAULT_ERROR_PREPEND, ex));
			}

			return response;
		}
	}
}
