namespace TextExtractor.Helpers
{
	public class Validator : IValidator
	{
		public bool VerifyIfNotLayout(int layoutArtifactId, int layoutArtifactIdByGuid)
		{
			return layoutArtifactId != layoutArtifactIdByGuid;
		}
	}
}