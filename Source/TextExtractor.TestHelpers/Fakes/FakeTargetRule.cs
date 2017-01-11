using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.Helpers;
using TextExtractor.Helpers.Interfaces;

namespace TextExtractor.TestHelpers.Fakes
{
    public class FakeTargetRule : ITargetRule
    {
        public Constant.MarkerEnum MarkerEnum { get; set; }

        public Constant.DirectionEnum DirectionEnum { get; set; }

        public int CharacterLength { get; set; }

        public Constant.TrimStyleEnum TrimStyleEnum { get; set; }

        public bool CaseSensitive { get; set; }

        public bool IncludeMarker { get; set; }

        public int Occurrence { get; set; }

        public int MaximumExtractions { get; set; }

        public int MinimumExtractions { get; set; }

        public string CustomDelimiter { get; set; }
    }
}
