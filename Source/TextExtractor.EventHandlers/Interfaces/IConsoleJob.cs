using Relativity.API;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.EventHandlers.Interfaces
{
	public interface IConsoleJob
	{
		IServicesMgr SvcMgr { get; set; }
		ExecutionIdentity ExecutionCurrentUserIdentity { get; set; }
		IDBContext EddsDbContext { get; set; }
		IArtifactQueries ArtifactQueries { get; set; }
		ISqlQueryHelper SqlQueryHelper { get; set; }
		int ActiveArtifactId { get; set; }
		int? SavedSearchArtifactId { get; set; }
		int? ExtractorProfileArtifactId { get; set; }
		int? SourceLongTextFieldArtifactId { get; set; }
		void Execute();
	}
}
