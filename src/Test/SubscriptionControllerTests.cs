using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Api.Controllers;
using Domain.Models;
using Domain.Requests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services;
using FluentAssertions;

namespace Test
{
    public class SubscriptionControllerTests
    {
        private readonly Mock<ISubscriptionService> _mockSubscriptionServiceContext;
        private readonly Mock<IUnsubscriptionService> _mockUnsubscriptionServiceContext;
        private readonly SubscriptionController _controller;
        public SubscriptionControllerTests()
        {
            _mockSubscriptionServiceContext = new Mock<ISubscriptionService>();
            _mockUnsubscriptionServiceContext = new Mock<IUnsubscriptionService>();
            _controller = new SubscriptionController(_mockUnsubscriptionServiceContext.Object, _mockSubscriptionServiceContext.Object);
        }

        [Fact]
        public async Task CreateSubscriptionReturnsOk()
        {
            // arrange
            var request = new SubscriptionsRequest();
            var expectedSubscription = new Subscriptions();
            _mockSubscriptionServiceContext.Setup(service => service.Create(request))
                                           .ReturnsAsync(expectedSubscription);
            var controller = new SubscriptionController(_mockUnsubscriptionServiceContext.Object, _mockSubscriptionServiceContext.Object);

            // act
            IActionResult result = await controller.Create(request);

            //assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateSubscriptionInvalidRequestReturnsBadRequest()
        {
            // arrange
            var request = new SubscriptionsRequest() { MainTaskIdTopic = 5, SubTaskIdSubscriber = 5 };
            var expectedSubscription = new Subscriptions();
            _mockSubscriptionServiceContext.Setup(service => service.Create(request))
                                           .ReturnsAsync(expectedSubscription);
            var controller = new SubscriptionController(_mockUnsubscriptionServiceContext.Object, _mockSubscriptionServiceContext.Object);

            // act
            IActionResult result = await controller.Create(request);

            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

    }
}
