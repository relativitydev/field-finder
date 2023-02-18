using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextExtractor.Helpers.Interfaces
{
    public interface ITargetRule
    {
      Constant.MarkerEnum MarkerEnum { get; set; }
      Constant.DirectionEnum DirectionEnum { get; set; }
      Int32 CharacterLength { get; set; }
      Constant.TrimStyleEnum TrimStyleEnum { get; set; }
      Boolean CaseSensitive { get; set; }
      Boolean IncludeMarker { get; set; }
      Int32 Occurrence { get; set; }

      Int32 MaximumExtractions { get; set; }
      Int32 MinimumExtractions { get; set; }
      String CustomDelimiter { get; set; }
    }
}
