﻿using ClickToCall.Web.Controllers;
using ClickToCall.Web.Models;
using ClickToCall.Web.Services;
using FluentMvcTesting.Extensions.Mocks;
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
                    Url = mockControllerProperties.Url(RouteConfig.RegisterRoutes)
                };

            var callViewModel = new CallViewModel {UserNumber = "user-number", SalesNumber = "sales-number"};
            controller
                .WithCallTo(c => c.Call(callViewModel))
                .ShouldReturnJson(data =>
                    {
                        Assert.That(data.message, Is.EqualTo("Phone call incoming!"));
                    });

            const string expectedUriHandler = "http://www.example.com/Call/Connect?salesNumber=sales-number";
            mockNotificationService.Verify(
                s => s.MakePhoneCallAsync("user-number", "twilio-number", expectedUriHandler), Times.Once());
        }
    }
}
