using kCura.EventHandler;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.EventHandlers.Interfaces
{
	public interface IRegularExpressionJob
	{
		IDBContext EddsDbContext { get; set; }
		ISqlQueryHelper SqlQueryHelper { get; set; }
		int ActiveArtifactId { get; set; }
		//Response ExecutePreSave();
		Response ExecutePreCascadeDelete();
	}
}
