using Api.Controllers;
using Domain.Models;
using Domain.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [Theory]
        [MemberData(nameof(ExceptionTestData))]
        public async Task Create_ReturnsException_OnServiceException(Exception ex)
        {
            // Arrange
            var subscriptionsRequest = new SubscriptionsRequest
            {
                MainTaskIdTopic = 999,
                SubTaskIdSubscriber = 9999
            };

            _subServMock.Setup(service => service.Create(subscriptionsRequest))
                        .ThrowsAsync(ex);

            // Act 
            Exception caughtException = null;
            try
            {
                await _controller.Create(subscriptionsRequest);
            }
            catch (Exception e)
            {
                caughtException = e;
            }

            // Assert
            caughtException.Should().NotBeNull();
            caughtException.Should().BeOfType(ex.GetType());
            caughtException!.Message.Should().Be(ex.Message);
        }

        public static IEnumerable<object[]> ExceptionTestData =>
        new List<object[]>
        {
            new object[] { new OperationCanceledException() },
            new object[] { new InvalidOperationException() },
            new object[] { new DbUpdateException() },
            new object[] { new DbUpdateConcurrencyException() }
        };
    }
}
