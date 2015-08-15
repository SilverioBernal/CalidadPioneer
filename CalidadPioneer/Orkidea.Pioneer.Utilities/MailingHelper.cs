using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Pioneer.Utilities
{
    public static class MailingHelper
    {
        public static void SendMail(List<MailAddress> to, string subject, string body)
        {
            try
            {
                var smtp = getSmtpSettings();
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(ConfigurationManager.AppSettings["emailFromAddress"].ToString(), ConfigurationManager.AppSettings["emailFromAilas"].ToString());
                    foreach (MailAddress item in to)
                    {
                        message.To.Add(item);
                    }
                    message.Body = body;
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    smtp.Send(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void SendMail(List<MailAddress> to, List<MailAddress> cc, string subject, string body)
        {
            try
            {
                var smtp = getSmtpSettings();
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(ConfigurationManager.AppSettings["emailFromAddress"].ToString(), ConfigurationManager.AppSettings["emailFromAilas"].ToString());
                    foreach (MailAddress item in to)
                    {
                        message.To.Add(item);
                    }
                    foreach (MailAddress item in cc)
                    {
                        message.CC.Add(item);
                    }
                    message.Body = body;
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    smtp.Send(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void SendMail(List<MailAddress> to, List<MailAddress> cc, List<MailAddress> bcc, string subject, string body)
        {
            try
            {
                var smtp = getSmtpSettings();
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(ConfigurationManager.AppSettings["emailFromAddress"].ToString(), ConfigurationManager.AppSettings["emailFromAilas"].ToString());
                    foreach (MailAddress item in to)
                    {
                        message.To.Add(item);
                    }
                    foreach (MailAddress item in cc)
                    {
                        message.CC.Add(item);
                    }
                    foreach (MailAddress item in bcc)
                    {
                        message.Bcc.Add(item);
                    }
                    message.Body = body;
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    smtp.Send(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void SendMail(List<MailAddress> to, string subject, string htmlBodyPath, string plainTextBodyPath, string logoPath)
        {
            try
            {
                var smtp = getSmtpSettings();
                // create the mail message
                MailMessage mail = new MailMessage();
                // set the addresses
                mail.From = new MailAddress(ConfigurationManager.AppSettings["emailFromAddress"].ToString(), ConfigurationManager.AppSettings["emailFromAilas"].ToString());
                foreach (MailAddress item in to)
                {
                    mail.To.Add(item);
                }
                // set the content
                mail.Subject = subject;
                StreamReader srPlainText = new StreamReader(plainTextBodyPath);
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(srPlainText.ReadToEnd(), null, "text/plain");
                // then we create the Html part to embed images,
                // we need to use the prefix 'cid' in the img src value
                StreamReader srHtmlText = new StreamReader(htmlBodyPath);
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(srHtmlText.ReadToEnd(), null, "text/html");
                // create image resource from image path using LinkedResource class..
                LinkedResource imageResource = new LinkedResource(logoPath, "image/png");
                imageResource.ContentId = "logoImg";
                imageResource.TransferEncoding = TransferEncoding.Base64;
                // adding the imaged linked to htmlView...
                htmlView.LinkedResources.Add(imageResource);
                // add the views
                mail.AlternateViews.Add(plainView);
                mail.AlternateViews.Add(htmlView);
                // send mail
                smtp.Send(mail);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void SendMail(List<MailAddress> to, string subject, string htmlBodyPath, string plainTextBodyPath, string logoPath, Dictionary<string, string> dynamicValues)
        {
            try
            {
                var smtp = getSmtpSettings();
                // create the mail message
                MailMessage mail = new MailMessage();
                // set the addresses
                mail.From = new MailAddress(ConfigurationManager.AppSettings["emailFromAddress"].ToString(), ConfigurationManager.AppSettings["emailFromAilas"].ToString());
                foreach (MailAddress item in to)
                {
                    mail.To.Add(item);
                }
                // set the content
                mail.Subject = subject;
                StreamReader srPlainText = new StreamReader(plainTextBodyPath);
                string plainText = srPlainText.ReadToEnd();
                srPlainText.Close();
                // then we create the Html part to embed images,
                // we need to use the prefix 'cid' in the img src value
                StreamReader srHtmlText = new StreamReader(htmlBodyPath);
                string htmlText = srHtmlText.ReadToEnd();
                srHtmlText.Close();
                //Set dinamyc values
                foreach (var item in dynamicValues)
                {
                    htmlText = htmlText.Replace(item.Key, item.Value);
                    plainText = plainText.Replace(item.Key, item.Value);
                }
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(plainText, null, "text/plain");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlText, null, "text/html");
                // create image resource from image path using LinkedResource class..
                LinkedResource imageResource = new LinkedResource(logoPath, "image/png");
                //LinkedResource imageResource = new LinkedResource(logoPath);
                imageResource.ContentId = "logoImg";
                imageResource.TransferEncoding = TransferEncoding.Base64;
                // adding the imaged linked to htmlView...
                htmlView.LinkedResources.Add(imageResource);
                // add the views
                mail.AlternateViews.Add(plainView);
                mail.AlternateViews.Add(htmlView);
                // send mail
                smtp.Send(mail);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static SmtpClient getSmtpSettings()
        {
            return new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["emailServer"].ToString(),
                Port = int.Parse(ConfigurationManager.AppSettings["emailPort"].ToString()),
                EnableSsl = bool.Parse(ConfigurationManager.AppSettings["emailEnableSSL"].ToString()),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailUsr"].ToString(), ConfigurationManager.AppSettings["emailPass"].ToString())
            };
        }
    }
}
