using System;

namespace TextExtractor.Helpers.Models
{
	public class ExtractorSetDocument
	{
		public Int32 ArtifactId { get; set; }
		public String TextSource { get; set; }

		public ExtractorSetDocument(Int32 artifactId, String textSource)
		{
			ArtifactId = artifactId;
			TextSource = textSource;
		}

		public ExtractorSetDocument GetInstance()
		{
			return this;
		}
	}
}