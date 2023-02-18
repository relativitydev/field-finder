using global::NUnit.Framework;

using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Fixtures
{
	public class FakesFixture
	{
		public ModelDependencies Dependencies;
		public RandomGenerator Random;

		[TestFixtureSetUp]
		public void RunBeforeAnyTests()
		{
			Dependencies = new ModelDependencies();
			Random = Dependencies.Pull<RandomGenerator>();
		}

		[TestFixtureTearDown]
		public void RunAfterAnyTests()
		{
			Dependencies = null;
			Random = null;
		}
	}
}