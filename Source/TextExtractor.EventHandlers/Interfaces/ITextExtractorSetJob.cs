
using kCura.EventHandler;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.EventHandlers.Interfaces
{
	public interface ITextExtractorSetJob
	{
		IDBContext EddsDBContext { get; set; }
		ISqlQueryHelper SqlQueryHelper { get; set; }
		int ActiveArtifactId { get; set; }
		Response ExecutePreSave();
		Response ExecutePreCascadeDelete();
		Response ExecutePreDelete();
		Response ExecutePreMassDelete();
	}
}
