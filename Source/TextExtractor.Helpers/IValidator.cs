using System.Collections.Generic;
using System.Threading.Tasks;
using Relativity.API;

namespace TextExtractor.Helpers
{
	public interface IValidator
	{
		bool VerifyIfNotLayout(int layoutArtifactId, int layoutArtifactIdByGuid);
	}
}
