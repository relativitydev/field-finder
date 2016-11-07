using System;
using System.Collections.Generic;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;

namespace TextExtractor.Helpers.Rsapi
{
	public class ErrorQueries
	{
		#region Public Methods
		public static void WriteError(IServicesMgr svcMgr, ExecutionIdentity identity, int workspaceArtifactId, Exception ex)
		{
			using (var client = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				client.APIOptions.WorkspaceID = workspaceArtifactId;

				var res = WriteError(client, workspaceArtifactId, ex);
				if (!res.Success)
				{
					throw new Exception(res.Message);
				}
			}
		}
		#endregion

		#region Private Methods

		private static Response<IEnumerable<Error>> WriteError(IRSAPIClient proxy, int workspaceArtifactId, Exception ex)
		{
			var artifact = new Error();
			artifact.FullError = ex.StackTrace;
			artifact.Message = ex.Message;
			artifact.SendNotification = false;
			artifact.Server = Environment.MachineName;
			artifact.Source = String.Format("{0} [Guid={1}]", Constant.Names.ApplicationName, Constant.Guids.ApplicationGuid);
			artifact.Workspace = new Workspace(workspaceArtifactId);
			var theseResults = proxy.Repositories.Error.Create(artifact);
			return Response<Error>.CompileWriteResults(theseResults);
		}

		#endregion Private Methods
	}
}
