using Api.Controllers;
using Domain.Models;
using Domain.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services;

namespace Test
{
    public class SubscriptionControllerTests
    {
        private readonly Mock<IUnsubscriptionService> _unSubServMock;
        private readonly Mock<ISubscriptionService> _subServMock;
        private readonly SubscriptionController _controller;

        public SubscriptionControllerTests()
        {
            _unSubServMock = new Mock<IUnsubscriptionService>();
            _subServMock = new Mock<ISubscriptionService>();
            _controller = new SubscriptionController(_unSubServMock.Object, _subServMock.Object);
        }
        // Create method (null / empty body and null values are being handled by the middleware on the call for the route, and can only be tested by integration tests)
        [Theory]
        [InlineData(5, 20, 5, 20, 1)]
        [InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, 2)]
        [InlineData(50, 1, 50, 1, 3)]
        public async Task Create_ReturnsOk_WhenRequestIsCorrect(int requestMainTaskIdTopic,
                                                                int requestSubTaskIdSubscriber,
                                                                int expectedMainTaskId,
                                                                int expectedSubTaskId,
                                                                int expectedSubscriptionId)
        {
            // Arrange
            SubscriptionsRequest subscriptionsRequest = new()
            {
                MainTaskIdTopic = requestMainTaskIdTopic,
                SubTaskIdSubscriber = requestSubTaskIdSubscriber
            };
            Subscriptions expectedSubscription = new()
            {
                Id = expectedSubscriptionId,
                MainTaskIdTopic = expectedMainTaskId,
                SubTaskIdSubscriber = expectedSubTaskId
            };
            _subServMock.Setup(service => service.Create(subscriptionsRequest)).ReturnsAsync(expectedSubscription);

            // Act
            var result = await _controller.Create(subscriptionsRequest) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull().And.BeOfType<Subscriptions>();

            var returnedSub = result.Value as Subscriptions;
            returnedSub!.Should().NotBeNull();
            returnedSub!.MainTaskIdTopic.Should().Be(subscriptionsRequest.MainTaskIdTopic);
            returnedSub!.SubTaskIdSubscriber.Should().Be(subscriptionsRequest.SubTaskIdSubscriber);
            returnedSub!.Id.Should().NotBeNull().And.BeGreaterThan(0);
        }

        [Fact]
        public async Task Create_ReturnsInternalServerError_OnServiceException()
        {
            // Arrange
            var subscriptionsRequest = new SubscriptionsRequest
            {
                MainTaskIdTopic = 5,
                SubTaskIdSubscriber = 1
            };
            _subServMock.Setup(service => service.Create(subscriptionsRequest))
                        .ThrowsAsync(new OperationCanceledException("Database error"));

            // Act
            var result = await _controller.Create(subscriptionsRequest) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().Be("Database error");
        }
    }
}
