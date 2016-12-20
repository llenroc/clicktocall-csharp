﻿using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using ClickToCall.Web.Services;
using Twilio.TwiML.Mvc;

namespace ClickToCall.Web.Controllers
{
    public class CallCenterController : TwilioController
    {
        private readonly INotificationService _notificationService;

        public CallCenterController() : this(new NotificationService())
        {
        }

        public CallCenterController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handle a POST from our web form and connect a call via REST API
        /// </summary>
        [HttpPost]
        public ActionResult Call(string userNumber, string salesNumber)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.First().Errors.First().ErrorMessage;
                return Json(new { success = false, message = errorMessage });
            }

            var twilioNumber = ConfigurationManager.AppSettings["TwilioNumber"];
            var uriHandler = GetUri(salesNumber);
            _notificationService.MakePhoneCall(twilioNumber, userNumber, uriHandler);

            return Json(new { success = true, message = "Phone call incoming!"});
        }

        private string GetUri(string salesNumber)
        {
            if (IsProductionHost(Request.Url.Host))
            {
                return Url.Action("Connect", "Call", null, Request.Url.Scheme);
            }

            var requestUrlScheme = Request.Url.Scheme;
            var domain = ConfigurationManager.AppSettings["TestDomain"];
            var urlAction = Url.Action("Connect", "Call", new { salesNumber });

            return $"{requestUrlScheme}://{domain}{urlAction}";
        }

        private static bool IsProductionHost(string host)
        {
            var isNgrok = host.Contains("ngrok.io");
            var isExample = host.Equals("www.example.com");

            return !(isNgrok || isExample);
        }
    }
}
