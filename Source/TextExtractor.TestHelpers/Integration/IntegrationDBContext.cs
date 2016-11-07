using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Relativity.API;
using TextExtractor.TestHelpers.Integration.Settings;

namespace TextExtractor.TestHelpers.Integration
{
	public class IntegrationDBContext : ADependency, IDBContext
	{
		public DBContextSettings Settings;

		public IntegrationDBContext(DBContextSettings settings)
		{
			this.Settings = settings;
		}

		public SqlConnection GetConnection()
		{
			var connectionString = String.Format(
			"Server={0};Database={1};User Id={2}; Password={3};",
			this.Settings.Server,
			this.Settings.Table,
			this.Settings.User,
			this.Settings.Password);

			var connection = new SqlConnection(connectionString);

			if (connection.State == ConnectionState.Closed)
			{
				connection.Open();
			}
			return connection;
		}

		public DbParameter CreateDbParameter()
		{
			throw new NotImplementedException();
		}

		public SqlConnection GetConnection(bool openConnectionIfClosed)
		{
			return this.GetConnection();
		}

		public SqlTransaction GetTransaction()
		{
			throw new NotImplementedException();
		}

		public void BeginTransaction()
		{
			throw new NotImplementedException();
		}

		public void CommitTransaction()
		{
			throw new NotImplementedException();
		}

		public void RollbackTransaction()
		{
			throw new NotImplementedException();
		}

		public void RollbackTransaction(Exception originatingException)
		{
			throw new NotImplementedException();
		}

		public void ReleaseConnection()
		{
			throw new NotImplementedException();
		}

		public void Cancel()
		{
			throw new NotImplementedException();
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement)
		{
			var table = new DataTable("Test Table");

			var con = this.GetConnection();

			var command = new SqlCommand(sqlStatement, con);

			using (var adapter = new SqlDataAdapter(command))
			{
				adapter.Fill(table);
			}

			con.Close();

			return table;
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, int timeoutValue)
		{
			return this.ExecuteSqlStatementAsDataTable(sqlStatement);
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			var table = new DataTable("Test Table");

			var con = this.GetConnection();

			var command = new SqlCommand(sqlStatement, con);

			foreach (var parameter in parameters)
			{
				command.Parameters.Add(parameter);
			}

			using (var adapter = new SqlDataAdapter(command))
			{
				adapter.Fill(table);
			}

			con.Close();

			return table;
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, int timeoutValue, IEnumerable<SqlParameter> parameters)
		{
			return this.ExecuteSqlStatementAsDataTable(sqlStatement, parameters);
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement)
		{
			var retVal = default(T);

			var con = this.GetConnection();

			var command = new SqlCommand(sqlStatement, con);

			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					retVal = reader.GetFieldValue<T>(0);
				}
			}

			con.Close();

			return retVal;
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			var retVal = default(T);

			var con = this.GetConnection();

			var command = new SqlCommand(sqlStatement, con);

			foreach (var parameter in parameters)
			{
				command.Parameters.Add(parameter);
			}

			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					retVal = reader.GetFieldValue<T>(0);
				}
			}

			con.Close();

			return retVal;
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, int timeoutValue)
		{
			return this.ExecuteSqlStatementAsScalar<T>(sqlStatement);
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			return this.ExecuteSqlStatementAsScalar<T>(sqlStatement, parameters);
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, params SqlParameter[] parameters)
		{
			return this.ExecuteSqlStatementAsScalar<T>(sqlStatement, parameters.AsEnumerable());
		}

		public object ExecuteSqlStatementAsScalar(string sqlStatement, params SqlParameter[] parameters)
		{
			var retVal = new object();

			var con = this.GetConnection();

			var command = new SqlCommand(sqlStatement, con);

			foreach (var parameter in parameters)
			{
				command.Parameters.Add(parameter);
			}

			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					retVal = reader.GetFieldValue<object>(0);
				}
			}

			con.Close();

			return retVal;
			;
		}

		public object ExecuteSqlStatementAsScalar(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			return this.ExecuteSqlStatementAsScalar(sqlStatement, parameters, 0);
		}

		public object ExecuteSqlStatementAsScalarWithInnerTransaction(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement)
		{
			var con = this.GetConnection();

			var command = new SqlCommand(sqlStatement, con);

			var retVal = command.ExecuteNonQuery();

			con.Close();

			return retVal;
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement, int timeoutValue)
		{
			return this.ExecuteNonQuerySQLStatement(sqlStatement);
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			var con = this.GetConnection();

			var command = new SqlCommand(sqlStatement, con);

			foreach (var parameter in parameters)
			{
				command.Parameters.Add(parameter);
			}

			var retVal = command.ExecuteNonQuery();

			con.Close();

			return retVal;
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			return this.ExecuteNonQuerySQLStatement(sqlStatement, parameters);
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement)
		{
			throw new NotImplementedException();
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement, IEnumerable<DbParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement, IEnumerable<DbParameter> parameters, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public DataTable ExecuteSQLStatementGetSecondDataTable(string sqlStatement, int timeout = -1)
		{
			throw new NotImplementedException();
		}

		public SqlDataReader ExecuteSQLStatementAsReader(string sqlStatement, int timeout = -1)
		{
			throw new NotImplementedException();
		}

		public DbDataReader ExecuteProcedureAsReader(string procedureName, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public int ExecuteProcedureNonQuery(string procedureName, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public SqlDataReader ExecuteParameterizedSQLStatementAsReader(
			string sqlStatement,
			IEnumerable<SqlParameter> parameters,
			int timeoutValue = -1,
			bool sequentialAccess = false)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public string Database
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string ServerName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsMasterDatabase
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}