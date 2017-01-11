using NUnit.Framework;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Dependencies.Seams;
using TextExtractor.Helpers.NUnit.Dependencies.Seams.Returns;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.Fakes;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Tests
{
    [TestFixture]
    public class ExtractorRegularExpressionTests : FakesFixture
    {
        [Description("")]        
        [Category(TestCategory.UNIT)]
        [Test]
        public void Constructor()
        {
            var target = GetSystemUnderTest();

            Assert.IsFalse(target.ArtifactId == 0);
            Assert.IsNotNullOrEmpty(target.Name);
            Assert.IsNotNullOrEmpty(target.RegularExpression);
            Assert.IsNotNull(target.Description);
        }

        [Description("When the constructor receives a null Rdo, should throw argument null exception")]
        [Category(TestCategory.UNIT)]
        [Test]
        public void Constructor_NullThrows()
        {
            Assert.Throws<CustomExceptions.TextExtractorException>(() => new ExtractorRegularExpression(null, 0, null, null));
        }

        public ExtractorRegularExpression GetSystemUnderTest()
        {
            var random = Dependencies.Pull<RandomGenerator>();
            var queries = Dependencies.Pull<ArtifactQueriesDependency>().Queries;
            var errorQueue = Dependencies.Pull<ErrorQueueDependency>().ErrorLogModel;
            var extractorRegularExpressionRdo = Dependencies.Pull<ArtifactQueriesReturns>().ExtractorRegularExpressionRdo;

            return new ExtractorRegularExpression(queries, random.Number(), extractorRegularExpressionRdo, errorQueue);
        }
    }
}
