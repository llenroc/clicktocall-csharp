﻿using ClickToCall.Web.Controllers;
using ClickToCall.Web.Models;
using ClickToCall.Web.Services;
using ClickToCall.Web.Tests.Mocks;
using Moq;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

namespace ClickToCall.Web.Tests.Controllers
{
    [TestFixture]
    public class CallCenterControllerTest
    {
        [Test]
        public void ShouldStartCallWithRealNumber()
        {
            var mockNotificationService = new Mock<INotificationService>();
            var mockControllerProperties = new ControllerPropertiesMock();
            var controller = new CallCenterController(mockNotificationService.Object)
                {
                    ControllerContext = mockControllerProperties.ControllerContext,
                    Url = mockControllerProperties.Url
                };

            var callViewModel = new CallViewModel {UserNumber = "user-number", SalesNumber = "sales-number"};
            controller
                .WithCallTo(c => c.Call(callViewModel))
                .ShouldReturnJson(data =>
                    {
                        Assert.That(data.message, Is.EqualTo("Phone call incoming!"));
                    });

            mockNotificationService.Verify(
                s => s.MakePhoneCall("twilio-number", "user-number", "http://test.domain.com"), Times.Once());
        }
    }
}
