using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers
{
	public class SqlQueryHelper : ISqlQueryHelper
	{
		#region AnyQueue

		public DataTable RetrieveCommonRow(IDBContext eddsDbContext, Int32 agentId, String queueTableName)
		{
			var sql = String.Format(@"SELECT TOP 1
				[WorkspaceArtifactID],
				[ExtractorSetArtifactID]
				FROM
				[EDDSDBO].{0}
				WHERE [AgentID] = @agentID", queueTableName);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@agentID", SqlDbType.Int) { Value = agentId }
			};

			var table = eddsDbContext.ExecuteSqlStatementAsDataTable(sql, sqlParams);

			return table;
		}

		public Boolean RemoveRecordFromTableById(IDBContext eddsDbContext, String queueTableName, Int32 rowId)
		{
			var sql = String.Format(@"
				DELETE FROM EDDSDBO.[{0}]
				WHERE [ID] = @rowID", queueTableName);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@rowID", SqlDbType.Int) { Value = rowId }
			};

			var rowsAffected = eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);

			return rowsAffected > 0;
		}

		public void RemoveRecordFromTableByAgentId(IDBContext eddsDbContext, String queueTableName, Int32 agentId)
		{
			var sql = String.Format(@"
				DELETE FROM EDDSDBO.[{0}]
				WHERE [AgentID] = @agentID", queueTableName);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@agentID", SqlDbType.Int) { Value = agentId }
			};

			eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);
		}

		public void ResetUnfishedJobs(IDBContext eddsDbContext, Int32 agentId, String queueTableName)
		{
			var sql = String.Format(@"
				UPDATE [EDDSDBO].[{0}]
				SET [AgentID] = NULL,
					[QueueStatus] = @notStartedQueueStatus
				WHERE [AgentID] = @agentID", queueTableName);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@agentID", SqlDbType.Int) { Value = agentId }, 
				new SqlParameter("@notStartedQueueStatus", SqlDbType.Int) { Value = Constant.QueueStatus.NotStarted }
			};

			eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);
		}

		public void RemoveBatchFromQueue(IDBContext eddsDbContext, String uniqueTableName)
		{
			var sql = String.Format(@"
				DELETE EDDSDBO.{0}
				FROM EDDSDBO.{0} S
					INNER JOIN EDDSDBO.{1} B ON B.ID = S.ID	", Constant.Tables.WorkerQueue, uniqueTableName);

			eddsDbContext.ExecuteNonQuerySQLStatement(sql);
		}

		public void DropTable(IDBContext dbContext, String tableName)
		{
			var sql = String.Format(@"
				IF NOT OBJECT_ID('EDDSDBO.{0}') IS NULL
					BEGIN DROP TABLE EDDSDBO.{0}
				END", tableName);

			dbContext.ExecuteNonQuerySQLStatement(sql);
		}

		public DataRow RetrieveSingleInManagerQueueByArtifactId(IDBContext dbContext, Int32 artifactId, Int32 workspaceArtifactId)
		{
			var sql = String.Format(@"
				DECLARE @offset INT SET @offset = (SELECT DATEDIFF(HOUR,GetUTCDate(),GetDate()))

				SELECT
					Q.[ID]
					,DATEADD(HOUR,@offset,Q.[TimeStampUTC]) [Added On]
					,Q.WorkspaceArtifactID [Workspace Artifact ID]
					,C.Name [Workspace Name]
					,CASE Q.[QueueStatus]
						WHEN @notStartedStatusId THEN 'Waiting'
						WHEN @inProgressStatusId THEN 'In Progress'
						WHEN @errorStatusId THEN 'Error'
						END [Status]
					,Q.AgentID [Agent Artifact ID]
					,Q.ExtractorSetArtifactID [Job ID]
				FROM EDDSDBO.{0} Q
					INNER JOIN EDDS.EDDSDBO.ExtendedCase C ON Q.WorkspaceArtifactID = C.ArtifactID
				WHERE Q.ExtractorSetArtifactID = @artifactId
					AND Q.WorkspaceArtifactID = @workspaceArtifactId", Constant.Tables.ManagerQueue);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@notStartedStatusId", SqlDbType.Int) { Value = Constant.QueueStatus.NotStarted }, 
				new SqlParameter("@inProgressStatusId", SqlDbType.Int) { Value = Constant.QueueStatus.InProgress }, 
				new SqlParameter("@errorStatusId", SqlDbType.Int) { Value = Constant.QueueStatus.Error }, 
				new SqlParameter("@artifactId", SqlDbType.Int) { Value = artifactId }, 
				new SqlParameter("@workspaceArtifactId", SqlDbType.Int) { Value = workspaceArtifactId }
			};

			var dt = dbContext.ExecuteSqlStatementAsDataTable(sql, sqlParams);
			if (dt.Rows.Count > 0)
			{
				return dt.Rows[0];
			}
			return null;
		}

		public void InsertRowIntoManagerQueue(IDBContext eddsDbContext, Int32 workspaceArtifactId, Int32? savedSearchId, Int32 extractorSetArtifactId, Int32? extractorProfileArtifactId, Int32? sourceLongTextFieldArtifactId)
		{
			var sql = String.Format(@"
			INSERT INTO EDDSDBO.{0}
			(
				[TimeStampUTC]
				,WorkspaceArtifactID
				,QueueStatus
				,AgentID
				,SavedSearchArtifactID
				,ExtractorSetArtifactID
				,ExtractorProfileArtifactID
				,SourceLongTextFieldArtifactID
			)
			VALUES
			(
				GetUTCDate()
				,@workspaceArtifactId
				,@queueStatus
				,NULL
				,@savedSearchId
				,@extractorSetArtifactID
				,@extractorProfileArtifactID
				,@sourceLongTextFieldArtifactID
			)", Constant.Tables.ManagerQueue);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@workspaceArtifactId", SqlDbType.Int) { Value = workspaceArtifactId }, 
				new SqlParameter("@queueStatus", SqlDbType.VarChar) { Value = Constant.QueueStatus.NotStarted }, 
				new SqlParameter("@savedSearchId", SqlDbType.Int) { Value = savedSearchId }, 
				new SqlParameter("@extractorSetArtifactID", SqlDbType.Int) { Value = extractorSetArtifactId }, 
				new SqlParameter("@extractorProfileArtifactID", SqlDbType.Int) { Value = extractorProfileArtifactId }, 
				new SqlParameter("@sourceLongTextFieldArtifactID", SqlDbType.Int) { Value = sourceLongTextFieldArtifactId }
			};

			eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);
		}

		public Int32 RetrieveJobCountInQueue(IDBContext eddsDbContext, String extractorSetArtifactIds, String queueTableName)
		{
			var sql = String.Format(@"
			SELECT COUNT(ID) [JobCount]
			FROM [EDDSDBO].[{0}] WITH(NOLOCK)
			WHERE 
				ExtractorSetArtifactID IN ({1})", queueTableName, extractorSetArtifactIds);

			var jobCount = Convert.ToInt32(eddsDbContext.ExecuteSqlStatementAsScalar(sql));
			return jobCount;
		}

		public Int32 RetrieveExtractorProfileCountInQueue(IDBContext eddsDbContext, String extractorProfileArtifactIds, String queueTableName)
		{
			var sql = String.Format(@"
			SELECT COUNT(ID) [JobCount]
			FROM [EDDSDBO].[{0}] WITH(NOLOCK)
			WHERE 
				ExtractorProfileArtifactID IN ({1})", queueTableName, extractorProfileArtifactIds);

			var profileCount = Convert.ToInt32(eddsDbContext.ExecuteSqlStatementAsScalar(sql));
			return profileCount;
		}

		public Int32 GetResourceServerByAgentId(IDBContext eddsDbContext, Int32 agentArtifactId)
		{
			var sql = String.Format(@"
			SELECT ISNULL([ServerArtifactID], 0) 
			FROM [EDDSDBO].[Agent] WITH(NOLOCK)	
			WHERE ArtifactID = {0}", agentArtifactId);

			return eddsDbContext.ExecuteSqlStatementAsScalar<Int32>(sql);
		}

		public Int32 RetrieveRegExCountFromTargetTextTableByTempTable(IDBContext dbContext, String tempTableName)
		{
			var sql = String.Format(@"
			SELECT COUNT(TargetT.ArtifactID) [SetCount]
			FROM 
				[EDDSDBO].[{0}] TargetT WITH(NOLOCK)
				INNER JOIN [EDDSResource].[EDDSDBO].[{1}] TT WITH(NOLOCK) 
				ON TargetT.RegularExpressionStartMarker = TT.ArtifactID OR TargetT.RegularExpressionStopMarker = TT.ArtifactID", Constant.Tables.ExtractorTargetText, tempTableName);

			var setCount = Convert.ToInt32(dbContext.ExecuteSqlStatementAsScalar(sql));
			return setCount;
		}

		public Int32 RetrieveProfileCountFromSetTableByTempTable(IDBContext dbContext, String tempTableName)
		{
			var sql = String.Format(@"
			SELECT COUNT(ES.ArtifactID) [SetCount]
			FROM 
				[EDDSDBO].[{0}] ES WITH(NOLOCK)
				INNER JOIN [EDDSResource].[EDDSDBO].[{1}] TT WITH(NOLOCK) ON ES.ExtractorProfile = TT.ArtifactID", Constant.Tables.ExtractorSet, tempTableName);

			var setCount = Convert.ToInt32(dbContext.ExecuteSqlStatementAsScalar(sql));
			return setCount;
		}

		public String RetrieveExtractorSetStatusBySetArtifactId(IDBContext dbContext, Int32 activeArtifactId)
		{
			var sql = String.Format(@"
			SELECT [Status] 
			FROM [EDDSDBO].[{0}] WITH(NOLOCK)
			WHERE ArtifactID = {1}
			", Constant.Tables.ExtractorSet, activeArtifactId);

			var setStatus = dbContext.ExecuteSqlStatementAsScalar(sql).ToString();
			return setStatus;
		}

		public Int32 RetrieveExtractorSetStatusCountByTempTable(IDBContext dbContext, String tempTableName)
		{
			var sql = String.Format(@"
			SELECT COUNT(ES.ArtifactID) [SetCount]
			FROM 
				[EDDSDBO].[{0}] ES WITH(NOLOCK)
				INNER JOIN [EDDSResource].[EDDSDBO].[{1}] TT WITH(NOLOCK) ON ES.ArtifactID = TT.ArtifactID
			WHERE ES.[Status] IS NOT NULL", Constant.Tables.ExtractorSet, tempTableName);

			var setStatus = Convert.ToInt32(dbContext.ExecuteSqlStatementAsScalar(sql));
			return setStatus;
		}

		#endregion AnyQueue

		#region ManagerQueue

		public void UpdateManagerRecordStatus(IDBContext eddsDbContext, Int32 statusId, Int32 recordID)
		{
			var sql = String.Format(@"
				UPDATE EDDSDBO.[{0}] SET [QueueStatus] = @statusId WHERE [ID] = @ID", Constant.Tables.ManagerQueue);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@statusId", SqlDbType.Int) { Value = statusId }, 
				new SqlParameter("@ID", SqlDbType.Int) { Value = recordID }
			};

			eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);
		}

		public void CreateManagerQueueTable(IDBContext eddsDbContext)
		{
			var sql = String.Format(@"
				IF OBJECT_ID('EDDSDBO.{0}') IS NULL BEGIN
					CREATE TABLE EDDSDBO.{0}
					(
						[ID] INT IDENTITY(1,1) PRIMARY KEY
						,[TimeStampUTC] DATETIME
						,[WorkspaceArtifactID] INT
						,[QueueStatus] INT
						,[AgentID] INT
						,[SavedSearchArtifactID] INT
						,[ExtractorSetArtifactID] INT
						,[ExtractorProfileArtifactID] INT
						,[SourceLongTextFieldArtifactID] INT
					)
				END

				IF NOT EXISTS (SELECT * FROM sys.sysindexes WHERE id = OBJECT_ID('{0}') AND name = 'i_{0}')
				BEGIN
					CREATE NONCLUSTERED INDEX [i_{0}] ON [EDDSDBO].{0}
					(
						[WorkspaceArtifactID] ASC
						,[ExtractorSetArtifactID] ASC
					) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
				END", Constant.Tables.ManagerQueue);

			eddsDbContext.ExecuteNonQuerySQLStatement(sql);
		}

		public DataTable RetrieveNextBatchInManagerQueue(IDBContext eddsDbContext, Int32 batchSize, Int32 agentId)
		{
			var sql = String.Format(@"

				-- Assigns a batch of rows to the Agent, respecting resource pool

				UPDATE [EDDSDBO].[{0}]
				 SET [AgentID] = @agentID,
					 [QueueStatus] = @inProgressQueueStatus
				 FROM (SELECT TOP (@batchSize)
					[ID],
					[AgentID],
					[QueueStatus],
					[WorkspaceArtifactID]
				FROM [EDDSDBO].[TextExtractor_ManagerQueue] WITH (UPDLOCK, READPAST)
				WHERE [QueueStatus] = @notStartedQueueStatus
				AND [WorkspaceArtifactID] IN (
					SELECT [ArtifactID] AS 'WorkspacesInResourceGroup'
					FROM [EDDSDBO].[ExtendedCase] 
					WHERE [ResourceGroupArtifactID] IN (
						 SELECT [ResourceGroupArtifactID] AS 'AgentsResourceGroup'
						 FROM [EDDSDBO].[ServerResourceGroup]
						 WHERE [ResourceServerArtifactID] = (
							SELECT [ServerArtifactID]
							FROM [EDDSDBO].[Agent]
							WHERE ArtifactID = @agentID 
							)
						)
					)
					ORDER BY [TimeStampUTC]
						) innerTable
				 WHERE innerTable.[ID] = [EDDSDBO].[{0}].[ID]

				 SELECT TOP (@batchSize)
					 [ID],
					 [WorkspaceArtifactID],
					 [QueueStatus],
					 [SavedSearchArtifactID],
					 [ExtractorSetArtifactID],
					 [ExtractorProfileArtifactID],
					 [SourceLongTextFieldArtifactID]
				 FROM [EDDSDBO].[{0}] WITH (NOLOCK)
				 WHERE [AgentID] = @agentID
				 ORDER BY [TimeStampUTC]

			", Constant.Tables.ManagerQueue);

			var sqlParams = new List<SqlParameter> 
			{ 
				new SqlParameter("@agentID", SqlDbType.Int) { Value = agentId }, 
				new SqlParameter("@batchSize", SqlDbType.Int) { Value = batchSize }, 
				new SqlParameter("@notStartedQueueStatus", SqlDbType.Int) { Value = Constant.QueueStatus.NotStarted }, 
				new SqlParameter("@inProgressQueueStatus", SqlDbType.Int) { Value = Constant.QueueStatus.InProgress } 
			};

			return eddsDbContext.ExecuteSqlStatementAsDataTable(sql, sqlParams);
		}

		public DataTable RetrieveNextBatchInManagerQueue(IDBContext eddsDbContext, Int32 agentId, Int32 batchSize, String uniqueTableName)
		{
			var sql = String.Format(@"
				BEGIN TRAN
					IF NOT OBJECT_ID('EDDSDBO.[{1}]') IS NULL BEGIN
						DROP TABLE EDDSDBO.[{1}]
					END
					CREATE TABLE EDDSDBO.[{1}](ID INT)

					DECLARE @workspaceArtifactID AS INT
					DECLARE @extractorSetArtifactID AS INT

					SELECT TOP 1
						@workspaceArtifactID = [WorkspaceArtifactID],
						@extractorSetArtifactID = [ExtractorSetArtifactID]
					FROM EDDSDBO.[{0}]
					WHERE 
						QueueStatus = @notStartedQueueStatus
						AND [WorkspaceArtifactID] IN (
							SELECT [ArtifactID]
							FROM [EDDSDBO].[ExtendedCase] WITH(NOLOCK) 
							WHERE [ResourceGroupArtifactID] IN (
								SELECT [ResourceGroupArtifactID]
								FROM [EDDSDBO].[ServerResourceGroup] WITH(NOLOCK) 
								WHERE [ResourceServerArtifactID] = (
									SELECT [ServerArtifactID]
									FROM [EDDSDBO].[Agent] WITH(NOLOCK)
									WHERE [ArtifactID] = @agentID
								)
							)
						)
					ORDER BY [ID] ASC

					INSERT INTO EDDSDBO.[{1}](ID)
					SELECT TOP (@batchSize) ID
					FROM EDDSDBO.[{0}] WITH(UPDLOCK,READPAST)
					WHERE
						[WorkspaceArtifactID] = @workspaceArtifactID
						AND
						[ExtractorSetArtifactID] = @extractorSetArtifactID
						AND
						[QueueStatus] = @notStartedQueueStatus
					ORDER BY
						[ID] ASC,
						[ExtractorSetArtifactID] ASC

					UPDATE S
						SET AgentID = 1016997,
						QueueStatus = @inProgressQueueStatus
					FROM EDDSDBO.[{1}] B
						INNER JOIN EDDSDBO.[{0}] S ON B.ID = S.ID
				COMMIT

				SELECT
					S.ID [QueueID]
					,S.[WorkspaceArtifactID]
					,S.[QueueStatus]
					,S.[AgentID]
					,S.[SavedSearchArtifactID]
					,S.[ExtractorSetArtifactID]
					,S.[ExtractorProfileArtifactID]
					,S.[SourceLongTextFieldArtifactID]
				FROM EDDSDBO.[{1}] B
					INNER JOIN EDDSDBO.[{0}] S ON B.ID = S.ID	", Constant.Tables.ManagerQueue, uniqueTableName);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@agentID", SqlDbType.Int) { Value = agentId },
				new SqlParameter("@batchSize", SqlDbType.Int) { Value = batchSize },
				new SqlParameter("@notStartedQueueStatus", SqlDbType.Int) { Value = Constant.QueueStatus.NotStarted },
				new SqlParameter("@inProgressQueueStatus", SqlDbType.Int) { Value = Constant.QueueStatus.InProgress }
			};

			return eddsDbContext.ExecuteSqlStatementAsDataTable(sql, sqlParams);
		}

		#endregion ManagerQueue

		#region WorkerQueue

		public void CreateWorkerQueueTable(IDBContext eddsDbContext)
		{
			var sql = String.Format(@"
				IF OBJECT_ID('EDDSDBO.{0}') IS NULL BEGIN
					CREATE TABLE EDDSDBO.{0}
					(
						[ID] INT IDENTITY(1,1) PRIMARY KEY
						,[TimeStampUTC] DATETIME
						,[WorkspaceArtifactID] INT
						,[QueueStatus] INT
						,[AgentID] INT
						,[ExtractorSetArtifactID] INT
						,[DocumentArtifactID] INT
						,[ExtractorProfileArtifactID] INT
						,[SourceLongTextFieldArtifactID] INT
					)
				END

				IF NOT EXISTS (SELECT * FROM sys.sysindexes WHERE id = OBJECT_ID('{0}') AND name = 'i_{0}')
				BEGIN
					CREATE NONCLUSTERED INDEX [i_{0}] ON [EDDSDBO].{0}
					(
						[WorkspaceArtifactID] ASC
						,[ExtractorSetArtifactID] ASC
						,[DocumentArtifactID] ASC
					) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
				END", Constant.Tables.WorkerQueue);

			eddsDbContext.ExecuteNonQuerySQLStatement(sql);
		}

		public void UpdateStatusInWorkerQueue(IDBContext eddsDbContext, Int32 statusId, String uniqueTableName)
		{
			var sql = String.Format(@"
					UPDATE S
						SET QueueStatus = @statusId
					FROM EDDSDBO.{1} B
						INNER JOIN EDDSDBO.{0} S ON B.ID = S.ID", Constant.Tables.WorkerQueue, uniqueTableName);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@statusId", SqlDbType.Int) { Value = statusId }
			};

			eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);
		}

		public DataTable RetrieveNextBatchInWorkerQueue(IDBContext eddsDbContext, Int32 agentId, Int32 batchSize, String uniqueTableName, Int32 resourceServerArtifactId)
		{
			var sql = String.Format(@"
				BEGIN TRAN
					IF NOT OBJECT_ID('EDDSDBO.{1}') IS NULL BEGIN
						DROP TABLE EDDSDBO.{1}
					END
					CREATE TABLE EDDSDBO.{1}(ID INT)

					DECLARE @workspaceArtifactID INT
					DECLARE @extractorSetArtifactID INT

					SELECT TOP 1
						@workspaceArtifactID = [WorkspaceArtifactID],
						@extractorSetArtifactID = [ExtractorSetArtifactID]
					FROM EDDSDBO.{0}
					WHERE 
						QueueStatus = @notStartedQueueStatus 
						AND [WorkspaceArtifactID] IN (
							SELECT ArtifactID FROM [EDDSDBO].[ExtendedCase] WITH(NOLOCK) WHERE ResourceGroupArtifactID IN (
							SELECT ResourceGroupArtifactID FROM [EDDSDBO].[ServerResourceGroup] WITH(NOLOCK) WHERE ResourceServerArtifactID = @resourceServerArtifactId)
						)
					ORDER BY [ID] ASC

					INSERT INTO EDDSDBO.{1}(ID)
					SELECT TOP (@batchSize) ID
					FROM EDDSDBO.{0} WITH(UPDLOCK,READPAST)
					WHERE
						[WorkspaceArtifactID] = @workspaceArtifactID
						AND
						[ExtractorSetArtifactID] = @extractorSetArtifactID
						AND
						[QueueStatus] = @notStartedQueueStatus
						AND
						[DocumentArtifactID] NOT IN
						(
							SELECT DISTINCT [DocumentArtifactID]
							FROM EDDSDBO.{0}
							WHERE [AgentID] IS NOT NULL
						)
					ORDER BY
						[ID] ASC,
						[ExtractorSetArtifactID] ASC,
						[DocumentArtifactID] ASC

					UPDATE S
						SET AgentID = @agentID,
						QueueStatus = @inProgressQueueStatus
					FROM EDDSDBO.{1} B
						INNER JOIN EDDSDBO.{0} S ON B.ID = S.ID
				COMMIT

				SELECT
					S.ID [QueueID]
					,S.[WorkspaceArtifactID]
					,S.[QueueStatus]
					,S.[AgentID]
					,S.[ExtractorSetArtifactID]
					,S.[DocumentArtifactID]
					,S.[ExtractorProfileArtifactID]
					,S.[SourceLongTextFieldArtifactID]
				FROM EDDSDBO.{1} B
					INNER JOIN EDDSDBO.{0} S ON B.ID = S.ID	", Constant.Tables.WorkerQueue, uniqueTableName);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@agentID", SqlDbType.Int) { Value = agentId },
				new SqlParameter("@batchSize", SqlDbType.Int) { Value = batchSize },
				new SqlParameter("@notStartedQueueStatus", SqlDbType.Int) { Value = Constant.QueueStatus.NotStarted },
				new SqlParameter("@inProgressQueueStatus", SqlDbType.Int) { Value = Constant.QueueStatus.InProgress },
				new SqlParameter("@resourceServerArtifactID", SqlDbType.Int) {Value = resourceServerArtifactId}
			};

			return eddsDbContext.ExecuteSqlStatementAsDataTable(sql, sqlParams);
		}

		public Boolean RemoveRecordFromWorkerQueue(IDBContext eddsDbContext, String queueTableName, Int32 queueId)
		{
			var sql = String.Format(@"
				DELETE FROM EDDSDBO.[{0}]
				WHERE [ID] = @queueId", queueTableName);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@queueId", SqlDbType.Int) { Value = queueId }
			};

			var rowsAffected = eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);
			return rowsAffected > 0;
		}

		public Boolean VerifyIfWorkerQueueContainsDocumentsForJob(IDBContext eddsDbContext, Int32 workspaceArtifactId, Int32 extractorSetArtifactId)
		{
			var sql = String.Format(@"SELECT COUNT(0) FROM [EDDSDBO].[{0}] WITH(NOLOCK) WHERE [ExtractorSetArtifactID]= @extractorSetArtifactId AND [WorkspaceArtifactID] = @workspaceArtifactId", Constant.Tables.WorkerQueue);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@extractorSetArtifactId", SqlDbType.Int) { Value = extractorSetArtifactId  },
				new SqlParameter("@workspaceArtifactId", SqlDbType.Int) { Value = workspaceArtifactId } 
			};

			var rows = eddsDbContext.ExecuteSqlStatementAsScalar<int>(sql, sqlParams);
			return rows > 0;
		}

		public void DeleteRecordsInWorkerQueueForCancelledExtractorSet(IDBContext eddsDbContext, Int32 workspaceArtifactId, Int32 extractorSetArtifactId)
		{
			var sql = String.Format(@"
				BEGIN TRAN
					DELETE 
					FROM EDDSDBO.[{0}]
					WHERE 
						[AgentID] IS NULL 
						AND [WorkspaceArtifactID] = @workspaceArtifactId
						AND [ExtractorSetArtifactID] = @extractorSetArtifactId
				COMMIT
				", Constant.Tables.WorkerQueue);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@workspaceArtifactId", SqlDbType.Int) { Value = workspaceArtifactId }, 
				new SqlParameter("@extractorSetArtifactId", SqlDbType.Int) { Value = extractorSetArtifactId }
			};

			eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);
		}

		public void DeleteRecordsInWorkerQueueForCancelledExtractorSetAndAgentId(IDBContext eddsDbContext, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, Int32 agentId)
		{
			var sql = String.Format(@"
				BEGIN TRAN
					DELETE 
					FROM EDDSDBO.[{0}]
					WHERE 
						[AgentID] = @agentId 
						AND [WorkspaceArtifactID] = @workspaceArtifactId
						AND [ExtractorSetArtifactID] = @extractorSetArtifactId
				COMMIT
				", Constant.Tables.WorkerQueue);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@agentId", SqlDbType.Int) { Value = agentId }, 
				new SqlParameter("@workspaceArtifactId", SqlDbType.Int) { Value = workspaceArtifactId }, 
				new SqlParameter("@extractorSetArtifactId", SqlDbType.Int) { Value = extractorSetArtifactId }
			};

			eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);
		}

		#endregion WorkerQueue

		#region ErrorQueue

		public void CreateErrorLogTable(IDBContext eddsDbContext)
		{
			var sql = String.Format(@"
				IF OBJECT_ID('EDDSDBO.{0}') IS NULL BEGIN
					CREATE TABLE EDDSDBO.{0}
					(
						ID INT IDENTITY(1,1)
						,[TimeStampUTC] DATETIME
						,WorkspaceArtifactID INT
						,ApplicationName VARCHAR(500)
						,ApplicationGuid uniqueidentifier
						,QueueTableName NVARCHAR(MAX)
						,QueueRecordID INT
						,AgentID INT
						,[Message] NVARCHAR(MAX)
					)
				END", Constant.Tables.ErrorLog);

			eddsDbContext.ExecuteNonQuerySQLStatement(sql);
		}

		public void InsertRowIntoErrorLog(IDBContext eddsDbContext, Int32 workspaceArtifactId, String queueTableName, Int32 queueRecordId, Int32 agentId, String errorMessage)
		{
			var sql = String.Format(@"
			INSERT INTO EDDSDBO.{0}
			(
				[TimeStampUTC]
				,WorkspaceArtifactID
				,ApplicationName
				,ApplicationGuid
				,QueueTableName
				,QueueRecordID
				,AgentID
				,[Message]
			)
			VALUES
			(
				GetUTCDate()
				,@workspaceArtifactId
				,@applicationName
				,@applicationGuid
				,@queueTableName
				,@queueRecordID
				,@agentID
				,@message
			)", Constant.Tables.ErrorLog);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@workspaceArtifactId", SqlDbType.Int) { Value = workspaceArtifactId }, 
				new SqlParameter("@applicationName", SqlDbType.VarChar) { Value = Constant.Names.ApplicationName },
				new SqlParameter("@applicationGuid", SqlDbType.UniqueIdentifier) {  Value = Constant.Guids.ApplicationGuid },
				new SqlParameter("@queueTableName", SqlDbType.VarChar) {  Value = queueTableName },
				new SqlParameter("@queueRecordID", SqlDbType.Int) { Value = queueRecordId },
				new SqlParameter("@agentID", SqlDbType.Int) { Value = agentId },
				new SqlParameter("@message", SqlDbType.NVarChar) { Value = errorMessage }
			};

			eddsDbContext.ExecuteNonQuerySQLStatement(sql, sqlParams);
		}

		#endregion ErrorQueue

		#region SQL Bulk Insert

		//Bulk Insert data
		public void BulkInsertIntoTable(IDBContext dbContext, DataTable sourceDataTable, List<SqlBulkCopyColumnMapping> columnMappings, String destinationTableName)
		{
			using (var bulkCopy = new SqlBulkCopy(dbContext.GetConnection()))
			{
				bulkCopy.DestinationTableName = destinationTableName;
				foreach (var columnMapping in columnMappings)
				{
					bulkCopy.ColumnMappings.Add(columnMapping);
				}
				bulkCopy.WriteToServer(sourceDataTable);
			}
		}

		//Sql column mappings for Bulk Insert data
		public List<SqlBulkCopyColumnMapping> GetMappingsForWorkerQueue(List<string> columnNameList)
		{
			return columnNameList.Select(column => new SqlBulkCopyColumnMapping(column, column)).ToList();
		}

		#endregion SQL Bulk Insert

		#region Extractor Set Profiles

		public List<Int32> RetrieveExtractorProfilesForField(IDBContext eddsDbContext, String fieldGuid1, String fieldGuid2, Int32 fieldArtifactId)
		{
			var sql = String.Format(@"
				DECLARE @FieldArtifactID1 INT, @FieldArtifactID2 INT
				DECLARE @MultiObjectTableName VARCHAR(50), @MultiObjectFieldName1 VARCHAR(50), @MultiObjectFieldName2 VARCHAR(50)

				SELECT @FieldArtifactID1 = ArtifactID FROM [EDDSDBO].[ArtifactGuid] WITH(NOLOCK)
				WHERE ArtifactGuid = @FieldGuid1

				SELECT @FieldArtifactID2 = ArtifactID FROM [EDDSDBO].[ArtifactGuid] WITH(NOLOCK)
				WHERE ArtifactGuid = @FieldGuid2

				SELECT
					@MultiObjectTableName = RelationalTableSchemaName,
					@MultiObjectFieldName1 = RelationalTableFieldColumnName1,
					@MultiObjectFieldName2 = RelationalTableFieldColumnName2
				FROM [EDDSDBO].[ObjectsFieldRelation] WITH(NOLOCK)
				WHERE 
					FieldArtifactId1 = @FieldArtifactID1
						AND FieldArtifactId2 = @FieldArtifactID2

				DECLARE @SQL NVARCHAR(MAX)
				SET @SQL = '
					SELECT ' + @MultiObjectFieldName2 + ' [TextExtractorProfileArtifactID]
					FROM [EDDSDBO].[' + @MultiObjectTableName + '] WITH(NOLOCK)
					WHERE ' + @MultiObjectFieldName1 + ' = ' + CONVERT(VARCHAR(20), {0})

				EXEC(@sql)", fieldArtifactId);

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@FieldGuid1", SqlDbType.VarChar) { Value = fieldGuid1 },
				new SqlParameter("@FieldGuid2", SqlDbType.VarChar) { Value = fieldGuid2 },
			};

			DataTable dtProfiles = eddsDbContext.ExecuteSqlStatementAsDataTable(sql, sqlParams);

			if (dtProfiles.Rows.Count > 0)
			{
				var list = dtProfiles.Rows.OfType<DataRow>().Select(dr => dr.Field<int>("TextExtractorProfileArtifactID")).ToList();
				return list;
			}
			return null;
		}

		#endregion Extractor Set Profiles

		#region SMTP settings
		public SmtpSettings GetSmptSettings(IDBContext context)
		{
			SmtpSettings settings = null;

			DataTable result = null;
			string sql = @"SELECT [Name],[Value] FROM [EDDS].[EDDSDBO].[Configuration] WITH (NOLOCK) WHERE Section='kCura.Notification'";

			result = context.ExecuteSqlStatementAsDataTable(sql);

			if (result.Rows.Count > 0)
			{
				settings = new SmtpSettings();

				string name;
				string value;
				foreach (DataRow dataRow in result.Rows)
				{
					name = dataRow["Name"] as string;
					value = dataRow["Value"] as string;
					if ((name == "AuthenticationEmailFrom" || name == "SMTPPassword" || name == "SMTPPort" || name == "SMTPServer" || name == "SMTPUserName") && String.IsNullOrEmpty(value) == true)
					{
						throw new TextExtractor.Helpers.CustomExceptions.IncorrectSmtpSettingsException("There are one or more missing SMTP setting. ");
					}

					switch (name)
					{
						case "AuthenticationEmailFrom":
							settings.RelativityInstanceFromEmailAddress = value;
							break;
						case "SMTPPassword":
							settings.Password = value;
							break;
						case "SMTPPort":
							settings.Port = Convert.ToInt32(value);
							break;
						case "SMTPServer":
							settings.Server = value;
							break;
						case "SMTPUserName":
							settings.UserName = value;
							break;
					}
				}
			}

			return settings;
		}

		#endregion
	}
}
