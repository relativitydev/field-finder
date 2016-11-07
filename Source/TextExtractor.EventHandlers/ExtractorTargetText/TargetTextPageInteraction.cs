using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.EventHandler;
using TextExtractor.Helpers;

namespace TextExtractor.EventHandlers.ExtractorTargetText
{
    [kCura.EventHandler.CustomAttributes.Description("This Event Handler allows a user to Submit or Cancel an Extractor Set.")]
    [System.Runtime.InteropServices.Guid("3c53f5e7-464a-43be-85a1-c84b1dd89a56")]
    public class TargetTextPageInteraction : PageInteractionEventHandler
    {
        public override Response PopulateScriptBlocks()
        {
            Response retVal = new kCura.EventHandler.Response();
            retVal.Success = true;
            retVal.Message = String.Empty;

            string markerTypeArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.MarkerType).ToString();
            string startMarkerPlainTextArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.PlainTextStartMarker).ToString();
            string stopMarkerPlainTextArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.PlainTextStopMarker).ToString();
            string startRegexArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStartMarker).ToString();
            string stopRegexArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.RegularExpressionStopMarker).ToString();
            string applyStopmarkerArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.ApplyStopMarker).ToString();
            string caseSensitiveArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.CaseSensitive).ToString();
            string directionArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.Direction).ToString();
            string choicePlainTextArtifactId = GetArtifactIdByGuid(Constant.Guids.Choices.MarkerType.PlainText).ToString();
            string choiceRegExArtifactId = GetArtifactIdByGuid(Constant.Guids.Choices.MarkerType.RegularExpression).ToString();
            string occurrenceArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.Occurrence).ToString();
            string charachterLengthArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.NumberofCharacters).ToString();
            string minExtractionsArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.MinimumExtractions).ToString();
            string maxExtractionsArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.MaximumExtractions).ToString();
            string delimiterArtifactId = GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.ResultsCustomDelimiter).ToString();

            String pageInteractionScript = "<script type=\"text/javascript\"></script>";

            var layoutArtifactIdByGuid = GetArtifactIdByGuid(Constant.Guids.Layout.TargetText);
            var layoutArtifactId = ActiveLayout.ArtifactID;

            var validator = new Validator();

            //check if this is the Text Extractor Target Text layout
            if (!validator.VerifyIfNotLayout(layoutArtifactIdByGuid, layoutArtifactId))
            {
                if (PageMode == kCura.EventHandler.Helper.PageMode.Edit)
                {
                    pageInteractionScript = "<script type=\"text/javascript\">$(document).ready(function(){ var isStopMarkerApplied = false; var isPlainText = false; var $markerType = $('[faartifactid=" + markerTypeArtifactId + "]').parent().find('td select'); var $startMarkerTextBox = $('[faartifactid=" + startMarkerPlainTextArtifactId + "]').closest('tr'); var $stopMarkerTextBox = $('[faartifactid=" + stopMarkerPlainTextArtifactId + "]').closest('tr'); var $regExStartContainer = $('[faartifactid=" + startRegexArtifactId + "]').closest('tr');var $regExStopContainer = $('[faartifactid=" + stopRegexArtifactId + "]').closest('tr'); var $applyStopMarkerInput = $('[faartifactid=" + applyStopmarkerArtifactId + "]').closest('tr'); var $caseSensitiveContainer = $('[faartifactid=" + caseSensitiveArtifactId + "]').closest('tr');var $directionContainer = $('[faartifactid=" + directionArtifactId + "]').closest('tr'); var $occurrenceContainer = $('[faartifactid=" + occurrenceArtifactId + "]').closest('tr'); var $charLenContainer = $('[faartifactid=" + charachterLengthArtifactId + "]').closest('tr'); var $minExtractionsContainer = $('[faartifactid=" + minExtractionsArtifactId + "]').closest('tr'); var $maxExtractionsContainer = $('[faartifactid=" + maxExtractionsArtifactId + "]').closest('tr');var $delimiterContainer = $('[faartifactid=" + delimiterArtifactId + "]').closest('tr'); var initialViewChanged = false; function hideInitialFields(){var radioValue = $applyStopMarkerInput.find('input[checked]').val(); if(radioValue === 'True'){ isStopMarkerApplied = true; } if($markerType.find('option[selected]').val() == " + choicePlainTextArtifactId + "){ isPlainText  = true; showCommonFields(); toggleContainersVisibility(); } else if ( $markerType.find('option[selected]').val() == " + choiceRegExArtifactId + "){ showCommonFields(); toggleContainersVisibility(); } else { $startMarkerTextBox.hide(); $stopMarkerTextBox.hide(); $regExStartContainer.hide(); $regExStopContainer.hide(); $caseSensitiveContainer.hide(); $directionContainer.hide(); $occurrenceContainer.hide(); $charLenContainer.hide(); $minExtractionsContainer.hide(); $maxExtractionsContainer.hide(); $delimiterContainer.hide();}} hideInitialFields(); $markerType.change(function(){showCommonFields(); if(this.value === '" + choicePlainTextArtifactId + "'){isPlainText = true;} else if(this.value == '" + choiceRegExArtifactId + "'){ isPlainText = false;}toggleContainersVisibility();});$applyStopMarkerInput.find('input:nth-child(1)').change(function(){ showCommonFields(); var radioValue = $applyStopMarkerInput.find('input:last').val();if(radioValue === '0'){ isStopMarkerApplied = true;} else {isStopMarkerApplied = false;}toggleContainersVisibility();}); function toggleContainersVisibility(){ if(isPlainText){ $startMarkerTextBox.show(); $caseSensitiveContainer.show(); $regExStartContainer.hide(); $regExStopContainer.hide(); if(isStopMarkerApplied){ $stopMarkerTextBox.show(); } else { $stopMarkerTextBox.hide(); } } else { $caseSensitiveContainer.hide(); $startMarkerTextBox.hide(); $regExStartContainer.show(); $stopMarkerTextBox.hide(); if (isStopMarkerApplied) { $regExStopContainer.show(); } else { $regExStopContainer.hide(); } } if(isStopMarkerApplied){ $directionContainer.hide(); } else { $directionContainer.show(); } }  function showCommonFields(){ if(!initialViewChanged){$occurrenceContainer.show(); $charLenContainer.show(); $minExtractionsContainer.show(); $maxExtractionsContainer.show(); $delimiterContainer.show(); } initialViewChanged = true;} });</script>";

                }
                else if (PageMode == kCura.EventHandler.Helper.PageMode.View)
                {
                    var markerTypeFieldValue = (kCura.EventHandler.ChoiceCollection)ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.MarkerType)].Value.Value;

                    String markerType = null;
                    foreach (kCura.EventHandler.Choice markerChoice in markerTypeFieldValue)
                    {
                        if (markerChoice.IsSelected)
                        {
                            markerType = markerChoice.Name;
                            break;
                        }
                    }

                    var applyStopMarkerFieldValue = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Fields.ExtractorTargetText.ApplyStopMarker)].Value.Value;
                    var applyStopMarker = Convert.ToBoolean(applyStopMarkerFieldValue);

                    switch (markerType)
                    {
                        case Constant.Choices.MarkerType.REGULAR_EXPRESSION:
                            if (applyStopMarker == false)
                            {
                                pageInteractionScript = "<script type=\"text/javascript\">$('[faartifactid=" + startMarkerPlainTextArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + stopMarkerPlainTextArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + caseSensitiveArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + stopRegexArtifactId + "]').closest('tr').hide();</script>";

                            }
                            else
                            {
                                pageInteractionScript = "<script type=\"text/javascript\">$('[faartifactid=" + startMarkerPlainTextArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + stopMarkerPlainTextArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + caseSensitiveArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + directionArtifactId + "]').closest('tr').hide();</script>";
                            }
                            break;
                        case Constant.Choices.MarkerType.PLAIN_TEXT:

                            if (applyStopMarker == false)
                            {
                                pageInteractionScript = "<script type=\"text/javascript\">$('[faartifactid=" + startRegexArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + stopMarkerPlainTextArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + stopRegexArtifactId + "]').closest('tr').hide(); </script>";
                            }
                            else
                            {
                                pageInteractionScript = "<script type=\"text/javascript\">$('[faartifactid=" + startRegexArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + directionArtifactId + "]').closest('tr').hide(); $('[faartifactid=" + stopRegexArtifactId + "]').closest('tr').hide(); </script>";
                            }
                            break;
                    }
                }
            }

            this.RegisterStartupScriptBlock(new kCura.EventHandler.ScriptBlock() { Key = "targetTextPageInteraction", Script = pageInteractionScript });

            return retVal;
        }
    }
}
