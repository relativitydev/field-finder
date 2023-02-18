using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
	public class ErrorQueueDependency : ADependency
	{
		public ErrorLogModel ErrorLogModel;

		public override void SharedExecute()
		{
			var random = Pull<RandomGenerator>();
			var query = Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var helper = Pull<FakeHelper>().Helper;

			ErrorLogModel = new ErrorLogModel(query, helper.GetDBContext(-1), random.Number(), Constant.Tables.ManagerQueue);
		}
	}
}
