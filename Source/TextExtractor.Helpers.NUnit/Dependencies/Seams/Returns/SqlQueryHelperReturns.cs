using System;
using System.Collections.Generic;
using System.Data;
using TextExtractor.Helpers.NUnit.Data;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns
{
	public class SqlQueryHelperReturns : ADependency
	{
		public DataTable NextJobInManagerQueue;

		public DataTable NextJobInWorkerQueue;

		public Boolean DidRemoveRecordFromTableByID;

		public DataTable AllJobsInManagerQueue;

		public DataTable AllJobsInWorkerQueue;

		public Int32 ResourceGroupID;

		private RandomGenerator Random;

		public SqlQueryHelperReturns()
		{
			this.Random = new RandomGenerator();
			this.Add(new TableGenerator());
			this.SharedExecute();
		}

		public override void SharedExecute()
		{
			var generator = this.Pull<TableGenerator>();
			this.NextJobInManagerQueue = generator.GetManagerQueueTable();
			this.NextJobInWorkerQueue = generator.GetWorkerQueueTable();
			this.DidRemoveRecordFromTableByID = true;
			this.AllJobsInManagerQueue = generator.GetManagerQueueTable(this.Random.Number(10, 100));
			this.AllJobsInWorkerQueue = generator.GetWorkerQueueTable(this.Random.Number(10, 100));
			this.ResourceGroupID = TestConstants.RESOURCE_GROUP_ID;
		}
	}
}