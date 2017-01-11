using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TextExtractor.Helpers.Models;

namespace TextExtractor.Helpers
{
	public class EmailUtility
	{
		private SmtpSettings smtpSettings;

		public EmailUtility(SmtpSettings smtpSettings)
		{
			this.smtpSettings = smtpSettings;
		}

		public void SendEmailNotificationForExtractionSet(string messageBody, string extractionSetName, String[] emailAddresses)
		{
			if (emailAddresses != null && emailAddresses.Count() > 0)
			{
				MailMessage message = new MailMessage()
				{
					Subject = string.Format("Automatic notification for Extraction Set: {0}", extractionSetName),
					Body = messageBody,
					From = new MailAddress(smtpSettings.RelativityInstanceFromEmailAddress),
					IsBodyHtml = false
				};

				foreach (string userEmail in emailAddresses)
				{
					message.To.Add(new MailAddress(userEmail));
				}

				SendMessage(message);
			}
		}

		private void SendMessage(MailMessage message)
		{
			using (SmtpClient smtpClient = new SmtpClient()
			{
				Host = smtpSettings.Server,
				Port = smtpSettings.Port,
				Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password),
				EnableSsl = smtpSettings.Port == 587 || smtpSettings.Port == 465
			})
			{
				try
				{
					smtpClient.Send(message);
				}
				catch (Exception ex)
				{
					throw new TextExtractor.Helpers.CustomExceptions.SmtpServerErrorException("Failed to send notifications. ", ex);
				}
			}
		}
	}
}
