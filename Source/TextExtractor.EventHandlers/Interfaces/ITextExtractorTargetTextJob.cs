using kCura.EventHandler;
using Relativity.API;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.EventHandlers.Interfaces
{
	public interface ITextExtractorTargetTextJob
	{
		IDBContext EddsDbContext { get; set; }
		IDBContext DbContext { get; set; }
		ISqlQueryHelper SqlQueryHelper { get; set; }
		int ActiveArtifactId { get; set; }
		int? Characters { get; set; }
		int? Occurence { get; set; }
		Response ExecutePreSave();
		Response ExecutePreCascadeDelete();
	}
}
