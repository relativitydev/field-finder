using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.Helpers.Models;
using TextExtractor.TestHelpers;
using TextExtractor.TestHelpers.TestingTools;

namespace TextExtractor.Helpers.NUnit.Dependencies
{
    public class SmtpSettingsDependency : ADependency
    {
        public SmtpSettings SmtpSettings;

        public override void SharedExecute()
        {
            var random = Pull<RandomGenerator>();

            SmtpSettings = new SmtpSettings()
            {
                Password = random.Word(),
                Port = random.Number(),
                RelativityInstanceFromEmailAddress = random.Word(),
                Server = random.Word(),
                UserName = random.Word()
            };
        }
    }
}
