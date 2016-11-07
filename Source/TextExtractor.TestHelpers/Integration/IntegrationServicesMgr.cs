using System;

using Relativity.API;

using TextExtractor.TestHelpers.Integration.Settings;

namespace TextExtractor.TestHelpers.Integration
{
	public class IntegrationServicesMgr : ADependency, IServicesMgr
	{
		public IntegrationServicesMgr(RsapiSettings settings)
		{
			var client = new IntegrationRsapiClient(settings);

			base.Add(client);
		}

		public Uri GetServicesURL()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Presently only returns Rsapi, not other services 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ident"></param>
		/// <returns></returns>
		public T CreateProxy<T>(ExecutionIdentity ident) where T : IDisposable
		{
			var client = base.Pull<IntegrationRsapiClient>();

			if (client.Client is T)
			{
				return (T)client.Client;
			}
			return default(T);
		}

		public Uri GetRESTServiceUrl()
		{
			throw new NotImplementedException();
		}
	}
}