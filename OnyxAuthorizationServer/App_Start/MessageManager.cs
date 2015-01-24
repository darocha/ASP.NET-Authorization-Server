using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using Microsoft.AspNet.Identity;

namespace Onyx.Authorization
{
    public  class MessageManager
    {
        public static void SendMail(IdentityMessage message)
        {
            message.Destination = "marcelo@darocha.com";
            
            string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
            string html = "<p>Please confirm your account by clicking this link: <a href=\"" + message.Body + "\">link</a></p>";

            html += HttpUtility.HtmlEncode(@"Or copy and paste the following link on the browser:" + message.Body);

            var MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            var MailAccount = ConfigurationManager.AppSettings["MailAccount"];
            var MailPassword = ConfigurationManager.AppSettings["MailPassword"];
            var MailSmtpServer = ConfigurationManager.AppSettings["MailSmtpServer"];
            var MailSmtpPort = ConfigurationManager.AppSettings["MailSmtpPort"];

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(MailFrom);
            msg.To.Add(new MailAddress(message.Destination));
            msg.Subject = message.Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient();//465 | 587
            smtpClient.Credentials = new System.Net.NetworkCredential(MailAccount, MailPassword);
            smtpClient.Host = MailSmtpServer;
            smtpClient.Port = Convert.ToInt32(587);
            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
        }

       
    }
}