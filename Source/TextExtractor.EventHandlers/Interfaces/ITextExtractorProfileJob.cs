using kCura.EventHandler;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.EventHandlers.Interfaces
{
	public interface ITextExtractorProfileJob
	{
		IDBContext EddsDbContext { get; set; }
		ISqlQueryHelper SqlQueryHelper { get; set; }
		int ActiveArtifactId { get; set; }
		Response ExecutePreSave();
		Response ExecutePreCascadeDelete();
	}
}
