namespace TextExtractor.Helpers.NUnit.Data
{
	using System;
	using System.Data;

	using TextExtractor.TestHelpers;
	using TextExtractor.TestHelpers.TestingTools;

	public class TableGenerator : ADependency
	{
		public RandomGenerator Random;

		public TableGenerator()
		{
			Random = new RandomGenerator();
		}

		#region DataTableGenerator

		#region DataTableDefinitions

		private DataTable GetManagerQueueDataDefinition()
		{
			var table = new DataTable("Manager QueueJob");

			table.Columns.Add("ID", typeof(Int32));
			table.Columns.Add("TimeStampUTC", typeof(DateTime));
			table.Columns.Add("WorkspaceArtifactID", typeof(Int32));
			table.Columns.Add("QueueStatus", typeof(Int32));
			table.Columns.Add("AgentID", typeof(Int32));
			table.Columns.Add("SavedSearchArtifactID", typeof(Int32));
			table.Columns.Add("ExtractorSetArtifactID", typeof(Int32));
			table.Columns.Add("ExtractorProfileArtifactID", typeof(Int32));
			table.Columns.Add("SourceLongTextFieldArtifactID", typeof(Int32));

			return table;
		}

		private DataTable GetWorkerQueueDataDefinition()
		{
			var table = new DataTable("Worker QueueJob");

			table.Columns.Add("QueueID", typeof(Int32));
			table.Columns.Add("TimeStampUTC", typeof(DateTime));
			table.Columns.Add("WorkspaceArtifactID", typeof(Int32));
			table.Columns.Add("QueueStatus", typeof(Int32));
			table.Columns.Add("AgentID", typeof(Int32));
			table.Columns.Add("ExtractorSetArtifactID", typeof(Int32));
			table.Columns.Add("DocumentArtifactID", typeof(Int32));
			table.Columns.Add("ExtractorProfileArtifactID", typeof(Int32));
			table.Columns.Add("SourceLongTextFieldArtifactID", typeof(Int32));

			return table;
		}

		#endregion DataTableDefinitions

		public DataTable GetManagerQueueTable(Int32 rows = 1)
		{
			var table = this.GetManagerQueueDataDefinition();

			for (int i = 0; i < rows; i++)
			{
				table.Rows.Add(
					i + 1,								 // ID
					DateTime.Now,					 // TimeStampUTC
					this.Random.Number(),  // WorkspaceArtifactID
					Constant.QueueStatus.NotStarted,    // QueueStatus
					this.Random.Number(),  // AgentID
					this.Random.Number(),  // SavedSearchArtifactID
					this.Random.Number(),  // ExtractorSetArtifactID
					this.Random.Number(),  // ExtractorProfileArtifactID
					this.Random.Number()   // SourceLongTextFieldArtifactID
					);
			}

			return table;
		}

		public DataTable GetWorkerQueueTable(Int32 rows = 1)
		{
			var table = this.GetWorkerQueueDataDefinition();

			for (int i = 0; i < rows; i++)
			{
				table.Rows.Add(
					this.Random.Number(maxSize: 100), // QueueID
					DateTime.Now,							// TimeStampUTC
					this.Random.Number(),  // WorkspaceArtifactID
					Constant.QueueStatus.NotStarted,    // QueueStatus
					this.Random.Number(),  // AgentID
					this.Random.Number(),  // ExtractorSetArtifactID
					this.Random.Number(),  // DocumentArtifactID
					this.Random.Number(),  // ExtractorProfileArtifactID
					this.Random.Number()   // SourceLongTextFieldArtifactID
					);
			}

			return table;
		}

		#endregion DataTableGenerator
	}
}