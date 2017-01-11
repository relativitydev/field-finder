using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.NUnit.Dependencies;
using TextExtractor.Helpers.NUnit.Fixtures;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Tests
{
    [TestFixture]
    public class EmailUtilityTests : FakesFixture
    {
        [Description("EmailUtility throws when SmtpSettings fields are null")]
		[Category(TestCategory.UNIT)]
		[Test]
        public void SendEmailNotificationForExtractionSetThrows()
        {
            var emailUtility = GetSystemUnderTest();

            var random = Dependencies.Pull<RandomGenerator>();
            string messageBody = random.Word();
            string extractionSetName = random.Word();
            String[] emailAddresses = new String[] { random.Word() };
            
            Assert.Throws<ArgumentNullException>(() => emailUtility.SendEmailNotificationForExtractionSet(messageBody, extractionSetName, emailAddresses));
        }

        public EmailUtility GetSystemUnderTest()
        {
            var smtpSettings = new SmtpSettings();

            return new EmailUtility(smtpSettings);
        }
    }
}
