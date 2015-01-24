using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Twilio;


namespace Onyx.Authorization {

    public static class Keys
    {
        public static string TwilioSid = ConfigurationManager.AppSettings["Twilio_AccountSid"];
        public static string TwilioToken = ConfigurationManager.AppSettings["Twilio_AuthToken"];
        public static string FromPhone = "+15005550006";
        public static string ToPhone = "+15005550006";
    }
}


namespace Onyx.Authorization.Controllers
{
   

    public class SMSController : ApiController
    {
        [HttpGet]
        public SMSMessage Send()
        {
            
            string AccountSid = ConfigurationManager.AppSettings["Twilio_AccountSid"];
            string AuthToken = ConfigurationManager.AppSettings["Twilio_AuthToken"];

            var twilio = new TwilioRestClient(AccountSid, AuthToken);
            
            //var message = twilio.SendMessage("+14158141829", "+14159352345", "Jenny please?! I love you <3", "");

            var sms = twilio.SendSmsMessage("+15005550006", "+14108675309", "All in the game, yo", "");

            return sms;
            
        }
    }
}
