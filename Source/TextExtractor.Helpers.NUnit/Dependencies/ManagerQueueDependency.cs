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
	public class ManagerQueueDependency : ADependency
	{
		public ManagerQueue Queue;

		public ManagerQueueDependency()
		{
			Add(new ManagerQueueRecordDependency());
		}

		public override void SharedExecute()
		{
			var random = Pull<RandomGenerator>();
			var query = Pull<SqlQueryHelperDependency>().SqlQueryHelper;
			var context = Pull<FakeDBContext>().DBContext;

			Queue = new ManagerQueue(query, context, random.Number());
		}
	}
}
