using Api.Controllers;
using Domain.Models;
using Domain.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services;
using System.Text;
using System.Text.Json.Nodes;

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

        [Theory]
        [InlineData(5, 20, 5, 20)]
        [InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue)]
        [InlineData(50, 1, 50, 1)]
        public async Task Create_ReturnsOk_WhenRequestIsCorrect(int requestMainTaskIdTopic,
                                                                int requestSubTaskIdSubscriber,
                                                                int expectedMainTaskId,
                                                                int expectedSubTaskId)
        {
            // Arrange
            SubscriptionsRequest subscriptionsRequest = new()
            {
                MainTaskIdTopic = requestMainTaskIdTopic,
                SubTaskIdSubscriber = requestSubTaskIdSubscriber
            };
            Subscriptions expectedSubscription = new()
            {
                Id = 0,
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
#pragma warning disable CS8604
        [Fact]
        public async Task Create_ReturnsBadRequest_WhenReceivingInvalidRequest()
        {
            // Arrange

            // Act
            var result = await _controller.Create(null) as BadRequestResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
#pragma warning restore CS8604
    }
}
