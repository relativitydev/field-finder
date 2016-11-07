using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.VSUnitTest.Fakes;

namespace TextExtractor.Helpers.VSUnitTest
{
    [TestClass]
    public class TextExtractionUtilityTests
    {
        public FakeTargetRule FakeTargetRule { get; set; }
        public TextExtractionUtility TextExtractionUtility { get; set; }

        [TestInitialize]
        public void InitializeTextExtractionUtility()
        {
            this.FakeTargetRule = new FakeTargetRule();
            this.TextExtractionUtility = new TextExtractionUtility(new TextExtractionValidator());
        }

        [TestMethod]
        public void StartMarkerPlaintTextTest()
        {
            this.FakeTargetRule.MarkerEnum = Constant.MarkerEnum.PlainText;
            this.FakeTargetRule.CaseSensitive = false;
            this.FakeTargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            this.FakeTargetRule.CharacterLength = 6;
            this.FakeTargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            this.FakeTargetRule.Occurrence = 1;
            this.FakeTargetRule.MaximumExtractions = 10000;

            string textSource = "Plain Text right direction extraction";
            string startMarker = "text";

            string result = this.TextExtractionUtility.ExtractText(textSource, startMarker, String.Empty, this.FakeTargetRule);

            Assert.AreEqual("Text right", result);
        }

        [TestMethod]
        public void StartMarkerRegexTest()
        {
            this.FakeTargetRule.MarkerEnum = Constant.MarkerEnum.RegEx;
            this.FakeTargetRule.CaseSensitive = false;
            this.FakeTargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            this.FakeTargetRule.CharacterLength = 6;
            this.FakeTargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            this.FakeTargetRule.Occurrence = 1;
            this.FakeTargetRule.MaximumExtractions = 10000;

            string textSource = "Plain Text 33 right direction extraction";
            string startMarker = @"[\d]+";

            string result = this.TextExtractionUtility.ExtractText(textSource, startMarker, String.Empty, this.FakeTargetRule);

            Assert.AreEqual("33 right", result);
        }

        [TestMethod]
        public void StartMarkerRegexOccurrenceTest()
        {
            this.FakeTargetRule.MarkerEnum = Constant.MarkerEnum.RegEx;
            this.FakeTargetRule.CaseSensitive = false;
            this.FakeTargetRule.DirectionEnum = Constant.DirectionEnum.Right;
            this.FakeTargetRule.CharacterLength = 6;
            this.FakeTargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            this.FakeTargetRule.Occurrence = 2;
            this.FakeTargetRule.CustomDelimiter = "; ";
            this.FakeTargetRule.MaximumExtractions = 2;

            string textSource = "Plain Text 11 right 22 direction 33 extraction 44 forth";
            string startMarker = @"[\d]+";

            string result = this.TextExtractionUtility.ExtractText(textSource, startMarker, String.Empty, this.FakeTargetRule);

            Assert.AreEqual("22 direc; 33 extra", result);
        }

        [TestMethod]
        public void StartStopMarkersPlainTextTest()
        {
            this.FakeTargetRule.MarkerEnum = Constant.MarkerEnum.PlainText;
            this.FakeTargetRule.CaseSensitive = false;
            this.FakeTargetRule.DirectionEnum = Constant.DirectionEnum.RightToStopMarker;
            this.FakeTargetRule.CharacterLength = 100;
            this.FakeTargetRule.TrimStyleEnum = Constant.TrimStyleEnum.None;
            this.FakeTargetRule.Occurrence = 1;
            this.FakeTargetRule.MaximumExtractions = 10000;

            string textSource = "Plain Text right direction extraction end";
            string startMarker = "text";
            string stopMarker = "extra";
            string result = this.TextExtractionUtility.ExtractText(textSource, startMarker, stopMarker, this.FakeTargetRule);

            Assert.AreEqual("Text right direction extra", result);
        }
    }
}
