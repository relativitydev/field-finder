using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.Helpers.Models;
using TextExtractor.Helpers.Rsapi;

namespace TextExtractor.EventHandlers.Application
{
	[kCura.EventHandler.CustomAttributes.RunOnce(true)]
	[kCura.EventHandler.CustomAttributes.Description("Populates all predefined regular expressions.")]
	[System.Runtime.InteropServices.Guid("46497F8E-9289-4411-B1CC-D44A0A369227")]
	class PopulatePredefinedRegularExpression : kCura.EventHandler.PostInstallEventHandler
	{
		public override kCura.EventHandler.Response Execute()
		{
			var response = new kCura.EventHandler.Response { Success = true, Message = String.Empty };
			var artifactQueries = new ArtifactQueries();
			var servicesMgr = Helper.GetServicesManager();
			var workspaceArtifactId = Helper.GetActiveCaseID();

			try
			{
				List<ExtractorRegularExpression> regExList = PopulateRegExList();

				//Populates Regular Expressions
				foreach (var regEx in regExList)
				{
					artifactQueries.CreateExtractorRegularExpressionRecord(servicesMgr, Relativity.API.ExecutionIdentity.System, workspaceArtifactId, regEx.Name, regEx.RegularExpression, regEx.Description);
				}
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Message = "Post-Install regular expression population failed with message: " + ex;
			}
			return response;
		}

        private List<ExtractorRegularExpression> PopulateRegExList()
		{
			ExtractorRegularExpression emailRegEx = new ExtractorRegularExpression()
			{
				Name = "Email",
				RegularExpression = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
				Description = "Matches emails"
			};
			ExtractorRegularExpression ssnRegEx = new ExtractorRegularExpression()
			{
				Name = "SSN",
				RegularExpression = @"\b\d{3}-\d{2}-\d{4}\b*",
				Description = "This regular expression matches SSN"
			};
			ExtractorRegularExpression usPhoneNumberRegEx = new ExtractorRegularExpression()
			{
				Name = "US Phone Number",
				RegularExpression = @"(1?)(-| ?)(\()?([0-9]{3})(\)|-| |\)-|\) )?([0-9]{3})(-| )?([0-9]{4}|[0-9]{4})",
				Description = "This regular expression matches US phone numbers"
			};
			ExtractorRegularExpression ukPhoneNumberRegEx = new ExtractorRegularExpression()
			{
				Name = "UK Phone Number",
				RegularExpression = @"(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?",
				Description = "This regular expression matches UK phone numbers"
			};
			ExtractorRegularExpression auPhoneNumberRegEx = new ExtractorRegularExpression()
			{
				Name = "AU Phone Number",
				RegularExpression = @"(\+\d{2}[ \-]{0,1}){0,1}(((\({0,1}[ \-]{0,1})0{0,1}\){0,1}[2|3|7|8]{1}\){0,1}[ \-]*(\d{4}[ \-]{0,1}\d{4}))|(1[ \-]{0,1}(300|800|900|902)[ \-]{0,1}((\d{6})|(\d{3}[ \-]{0,1}\d{3})))|(13[ \-]{0,1}([\d \-]{5})|((\({0,1}[ \-]{0,1})0{0,1}\){0,1}4{1}[\d \-]{8,10})))",
				Description = "This regular expression matches AU phone numbers"
			};
			ExtractorRegularExpression dateRegEx1 = new ExtractorRegularExpression()
			{
				Name = "Date Format 1",
				RegularExpression = @"((0?[13578]|10|12)(-|\/)((0[0-9])|([12])([0-9]?)|(3[01]?))(-|\/)((\d{4})|(\d{2}))|(0?[2469]|11)(-|\/)((0[0-9])|([12])([0-9]?)|(3[0]?))(-|\/)((\d{4}|\d{2})))",
				Description = "This regular expression matches dates in following formats: XX/XX/YYYY, XX/XX/YY, X/X/YY, XX-XX-YYYY, XX-XX-YY, X-X-YY"
			};
			ExtractorRegularExpression dateRegEx2 = new ExtractorRegularExpression()
			{
				Name = "Date Format 2",
				RegularExpression = @"((0?[1-9]|[12][1-9]|3[01])\.(0?[13578]|1[02])\.20[0-9]{2}|(0?[1-9]|[12][1-9]|30)\.(0?[13456789]|1[012])\.20[0-9]{2}|(0?[1-9]|1[1-9]|2[0-8])\.(0?[123456789]|1[012])\.20[0-9]{2}|(0?[1-9]|[12][1-9])\.(0?[123456789]|1[012])\.20(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96))",
				Description = "This regular expression matches dates in the following formats: DD.MM.YYYY, D.M.YYYY"
			};
			ExtractorRegularExpression ibanRegEx = new ExtractorRegularExpression()
			{
				Name = "IBAN",
				RegularExpression = @"[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}",
				Description = "This regular expression matches IBAN"
			};
			ExtractorRegularExpression creditCardRegEx = new ExtractorRegularExpression()
			{
				Name = "Credit Card",
				RegularExpression = @"(4\d{12})|(((4|3)\d{3})|(5[1-5]\d{2})|(6011))(-?|\040?)(\d{4}(-?|\040?)){3}|((3[4,7]\d{2})((-?|\040?)\d{6}(-?|\040?)\d{5}))|(3[4,7]\d{2})((-?|\040?)\d{4}(-?|\040?)\d{4}(-?|\040?)\d{3})|(3[4,7]\d{1})(-?|\040?)(\d{4}(-?|\040?)){3}|(30[0-5]\d{1}|(36|38)\d(2))((-?|\040?)\d{4}(-?|\040?)\d{4}(-?|\040?)\d{2})|((2131|1800)|(2014|2149))((-?|\040?)\d{4}(-?|\040?)\d{4}(-?|\040?)\d{3})",
				Description = "This regular expression matches credit cards"
			};

			List<ExtractorRegularExpression> regExList = new List<ExtractorRegularExpression>() { emailRegEx, ssnRegEx, usPhoneNumberRegEx, ukPhoneNumberRegEx, auPhoneNumberRegEx,
				dateRegEx1, dateRegEx2, ibanRegEx, creditCardRegEx};
			return regExList;
		}
	}
}
