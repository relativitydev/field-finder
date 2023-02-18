using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TextExtractor.Helpers.Interfaces;
using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers
{
	public class TextExtractionUtility : ITextExtractionUtility
	{        
        private string currentStopMarker;
        private int currentStopMarkerIndex;
        private int currentOccurence;
        private int extractionsEnd;
        private List<string> textExtracted;

		public ITextExtractionValidator TextExtractionValidator { get; set; }
        public string StopMarker { get; set; }
		public bool IsTextTruncated { get; set; }
		public bool IsMarkerFound { get; set; }
        public ITargetRule TargetRule { get; set; }
        
		public TextExtractionUtility(ITextExtractionValidator textExtractionValidator)
		{
			TextExtractionValidator = textExtractionValidator;
			IsTextTruncated = false;
			IsMarkerFound = true;
            this.currentStopMarkerIndex = -1;
            this.currentStopMarker = String.Empty;
            this.currentOccurence = 1;
            this.textExtracted = new List<string>();
		}

		public string ExtractText(string textSource, string startMarker, string stopMarker, ITargetRule targetRule)
		{
            this.TargetRule = targetRule;
            this.StopMarker = stopMarker;
            this.extractionsEnd = targetRule.MaximumExtractions + targetRule.Occurrence;

            if (targetRule.CharacterLength > Constant.Sizes.EXTRACTOR_TARGET_TEXT_CHARACTERS_MAXIMUM || targetRule.CharacterLength < Constant.Sizes.EXTRACTOR_TARGET_TEXT_CHARACTERS_MINIMUM)
			{
				throw new CustomExceptions.TextExtractorException(string.Format("Number of Characters is not between {0} and {1}.", Constant.Sizes.EXTRACTOR_TARGET_TEXT_CHARACTERS_MINIMUM, Constant.Sizes.EXTRACTOR_TARGET_TEXT_CHARACTERS_MAXIMUM));
			}

            if (targetRule.Occurrence > Constant.Sizes.EXTRACTOR_TARGET_TEXT_OCCURENCE_MAXIMUM || targetRule.Occurrence < Constant.Sizes.EXTRACTOR_TARGET_TEXT_OCCURENCE_MINIMUM)
			{
				throw new CustomExceptions.TextExtractorException(string.Format("Occurrence is not between {0} and {1}.", Constant.Sizes.EXTRACTOR_TARGET_TEXT_OCCURENCE_MINIMUM, Constant.Sizes.EXTRACTOR_TARGET_TEXT_OCCURENCE_MAXIMUM));
			}            

			try
			{
				//validate inputs
                TextExtractionValidator.Validate(textSource, startMarker, targetRule);                  

                ExtractTextByMarker(textSource, startMarker);
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(Constant.ErrorMessages.DEFAULT_TEXT_EXTRACTION_ERROR_MESSAGE, ex);
			}

            string result = null;

            if(this.textExtracted.Count >= targetRule.MinimumExtractions)
            {
                result = String.Join(targetRule.CustomDelimiter, this.textExtracted);
                this.IsMarkerFound = true;
            }
            else
            {
                this.IsMarkerFound = false;
            }

            return result;
		}
               
        private void ExtractTextByMarker(string textSource, string regexStartMarker)
        {
            try
            {
                MatchCollection startMarkers = GetRegExMarkerMatches(textSource, regexStartMarker);
                MatchCollection stopMarkers = !String.IsNullOrEmpty(this.StopMarker) ? 
                    GetRegExMarkerMatches(textSource, this.StopMarker) : 
                    null;

                foreach (Match startMarker in startMarkers)
                {
                    //startMarker is after previous stopMarker
                    if (this.TargetRule.DirectionEnum == Constant.DirectionEnum.RightToStopMarker &&
                        startMarker.Index + startMarker.Value.Length > this.currentStopMarkerIndex)
                    {
                        foreach (Match stopMarker in stopMarkers)
                        {
                            if (startMarker.Index + startMarker.Value.Length < stopMarker.Index)
                            {
                                this.currentStopMarker = stopMarker.Value;
                                this.currentStopMarkerIndex = stopMarker.Index;
                                break;
                            }
                        }                        
                    }
                    //startMarker is before previous stopMarker
                    else if (startMarker.Index < this.currentStopMarkerIndex + this.currentStopMarker.Length)
                    {
                        continue;
                    }

                    GetExtractedText(textSource, startMarker.Value, startMarker.Index);
                }              
            }
            catch (ArgumentOutOfRangeException)
            {
                this.textExtracted = null;
            }
        }

        private void GetExtractedText(string textSource, string marker, int markerIndex)
        {
            string matchTextExtracted = GetExtractedTextByDirectionEnum(textSource, markerIndex, marker.Length);

            if (!String.IsNullOrEmpty(matchTextExtracted))
            {
                if (this.currentOccurence >= this.TargetRule.Occurrence && this.extractionsEnd > 1)
                {
                    // Trim Text
                    matchTextExtracted = TrimText(matchTextExtracted);
                    this.textExtracted.Add(matchTextExtracted);
                }

                this.extractionsEnd--;
                this.currentOccurence++;
            }
        }
       
		private string GetExtractedTextByDirectionEnum(string textSource, int matchingTextIndex, int markerLength)
		{
            int startIndex = -1;
            int characterLength = 0;
                        
            switch (this.TargetRule.DirectionEnum)
            {
                case Constant.DirectionEnum.Right:
                    startIndex = TargetRule.IncludeMarker ? matchingTextIndex : matchingTextIndex + markerLength;

                    //Check to see if the text has to be truncated
                    if (matchingTextIndex + markerLength + this.TargetRule.CharacterLength > textSource.Length)
                    {
                        characterLength = textSource.Length - startIndex;
                        IsTextTruncated = true;                       
                    }
                    else
                    {
                        characterLength = TargetRule.IncludeMarker ? markerLength + this.TargetRule.CharacterLength : this.TargetRule.CharacterLength;
                    }

                    break;
                case Constant.DirectionEnum.Left:
                    GetLeftStartIndexAndCharLenght(matchingTextIndex, markerLength, ref startIndex, ref characterLength);
                    break;
                case Constant.DirectionEnum.LeftAndRight:
                    GetLeftStartIndexAndCharLenght(matchingTextIndex, markerLength, ref startIndex, ref characterLength);

                    string leftSide = textSource.Substring(startIndex, characterLength);

                    //Check to see if the text has to be truncated
                    if (matchingTextIndex + markerLength + this.TargetRule.CharacterLength > textSource.Length)
                    {
                        characterLength = textSource.Length - matchingTextIndex - markerLength;
                        IsTextTruncated = true;
                    }
                    else
                    {
                        characterLength = this.TargetRule.CharacterLength;
                    }

                    string rightSide = textSource.Substring(matchingTextIndex + markerLength, characterLength);

                    string combinedOutput = TargetRule.IncludeMarker ? leftSide + rightSide : leftSide.Trim() + " " + rightSide.TrimStart();
                    return combinedOutput;
                case Constant.DirectionEnum.RightToStopMarker:                   
                    //stopMarker not found, so no extraction
                    if (this.currentStopMarkerIndex <= matchingTextIndex) 
                    {
                        startIndex = -1;
                        break;
                    }

                    startIndex = TargetRule.IncludeMarker ? matchingTextIndex : matchingTextIndex + markerLength;
                    
                    //stopMarker is too far by TargetRule CharLen
                    if (matchingTextIndex + markerLength + TargetRule.CharacterLength < currentStopMarkerIndex)
                    {
                        characterLength = TargetRule.IncludeMarker ? markerLength + TargetRule.CharacterLength : TargetRule.CharacterLength;                        
                    }
                    else
                    {
                        characterLength = TargetRule.IncludeMarker ? this.currentStopMarkerIndex - startIndex + this.currentStopMarker.Length :
                            this.currentStopMarkerIndex - startIndex;
                    }

                    break;
            }

            string extraction = String.Empty;

            if (startIndex > -1) 
            {
                extraction = textSource.Substring(startIndex, characterLength);   
            }           
			
			return extraction;
		}

        private void GetLeftStartIndexAndCharLenght(int matchingTextIndex, int markerLength, ref int startIndex, ref int characterLength)
        {
            //Check to see if the text has to be truncated
            if (matchingTextIndex - this.TargetRule.CharacterLength < 0)
            {
                startIndex = 0;
                characterLength = TargetRule.IncludeMarker ? matchingTextIndex + markerLength : matchingTextIndex;
                IsTextTruncated = true;
            }
            else
            {
                startIndex = matchingTextIndex - this.TargetRule.CharacterLength;
                characterLength = TargetRule.IncludeMarker ? this.TargetRule.CharacterLength + markerLength : this.TargetRule.CharacterLength;
            }
        }	        

		private string TrimText(string text)
		{
			//check for trim
			switch (this.TargetRule.TrimStyleEnum)
			{
				case Constant.TrimStyleEnum.Left:
					text = text.TrimStart();
					break;
				case Constant.TrimStyleEnum.Right:
					text = text.TrimEnd();
					break;
				case Constant.TrimStyleEnum.LeftAndRight:
					text = text.Trim();
					break;
				case Constant.TrimStyleEnum.None:
					break;
			}

			return text;
		}

        private MatchCollection GetRegExMarkerMatches(string textSource, string regex)
        {
            if (!String.IsNullOrEmpty(regex) && this.TargetRule.MarkerEnum == Constant.MarkerEnum.PlainText)
            {
                regex = Regex.Escape(regex);
            }

            if (!String.IsNullOrEmpty(regex) && this.TargetRule.MarkerEnum == Constant.MarkerEnum.PlainText && TargetRule.CaseSensitive == false)
            {
                regex = "(?i)" + regex;
            }

            var results = Regex.Matches(textSource, regex);

            return results;
        }    
	}
}