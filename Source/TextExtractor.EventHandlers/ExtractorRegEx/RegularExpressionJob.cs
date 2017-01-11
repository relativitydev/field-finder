using kCura.EventHandler;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.EventHandlers.Interfaces;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.EventHandlers.ExtractorRegEx
{
	public class RegularExpressionJob : IRegularExpressionJob
	{
		public IDBContext EddsDbContext { get; set; }
		public IDBContext DbContext { get; set; }
		public ISqlQueryHelper SqlQueryHelper { get; set; }
		public int ActiveArtifactId { get; set; }
		public string TempTableName { get; set; }

		public RegularExpressionJob(IDBContext eddsDbContext, ISqlQueryHelper sqlQueryHelper, int activeArtifactId)
		{
			EddsDbContext = eddsDbContext;
			SqlQueryHelper = sqlQueryHelper;
			ActiveArtifactId = activeArtifactId;
		}

		public RegularExpressionJob(IDBContext dbContext, ISqlQueryHelper sqlQueryHelper, String tempTableName)
		{
			DbContext = dbContext;
			SqlQueryHelper = sqlQueryHelper;
			TempTableName = tempTableName;
		}

		public Response ExecutePreCascadeDelete()
		{
			var response = new Response { Success = true, Message = string.Empty };

			try
			{
				var setCount = SqlQueryHelper.RetrieveRegExCountFromTargetTextTableByTempTable(DbContext, TempTableName);

				if (setCount > 0)
				{
					response.Success = false;
					response.Exception = new SystemException(Constant.ErrorMessages.REGULAR_EXPRESSION_RECORD_DEPENDENCY);
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
