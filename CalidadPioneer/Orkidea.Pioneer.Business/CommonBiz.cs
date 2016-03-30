using Orkidea.Pioneer.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Pioneer.Business
{
    public class CommonBiz
    {
        public void sendContactMessage(string type, string fromMail, string subject, string message, string rootPath)
        {
            List<System.Net.Mail.MailAddress> to = new List<System.Net.Mail.MailAddress>();
            string[] receivers = null;
            if (ConfigurationManager.AppSettings["testMail"].ToString() == "N")
            {
                switch (type)
                {
                    case "Sugerencia":
                        receivers = ConfigurationManager.AppSettings["emailContactAddress"].ToString().Split('|');
                        break;
                    case "Copaso":
                        receivers = ConfigurationManager.AppSettings["emailCopasoAddress"].ToString().Split('|');                        
                        break;
                    case "RRHH":
                        receivers = ConfigurationManager.AppSettings["emailRHAddress"].ToString().Split('|');                        
                        break;
                    default:
                        receivers = ConfigurationManager.AppSettings["emailContactAddress"].ToString().Split('|');
                        break;
                }

                foreach (string item in receivers)                
                    to.Add(new System.Net.Mail.MailAddress(item));
                
            }
            else
                to.Add(new System.Net.Mail.MailAddress("silverio.bernal@orkidea.co"));

            Dictionary<string, string> dynamicValues = new Dictionary<string, string>();
            dynamicValues.Add("[correo]", fromMail);
            dynamicValues.Add("[fecha]", DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss"));
            dynamicValues.Add("[mensaje]", message);
            MailingHelper.SendMail(to, subject,
            rootPath + ConfigurationManager.AppSettings["MailContactoHtml"].ToString(),
            rootPath + ConfigurationManager.AppSettings["MailContactoText"].ToString(),
            rootPath + ConfigurationManager.AppSettings["emailLogoPath"].ToString(), dynamicValues);
        }
    }
}
