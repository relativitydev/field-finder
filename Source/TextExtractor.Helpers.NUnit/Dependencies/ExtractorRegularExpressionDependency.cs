using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
    public class ExtractorRegularExpressionDependency : ADependency
    {
        public ExtractorRegularExpression ExtractorRegularExpression;
        
		public override void SharedExecute()
		{
            var random = Pull<RandomGenerator>();
            var queries = Pull<ArtifactQueriesDependency>().Queries;
            var errorQueue = Pull<ErrorQueueDependency>().ErrorLogModel;
            var extractorRegularExpressionRdo = Pull<ArtifactQueriesReturns>().ExtractorRegularExpressionRdo;

            ExtractorRegularExpression = new ExtractorRegularExpression(queries, random.Number(), extractorRegularExpressionRdo, errorQueue);
		}
    }
}
