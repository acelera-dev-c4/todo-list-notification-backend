using Domain.Models;
using Domain.Requests;
using FluentAssertions;
using Infra;
using Infra.DB;
using Microsoft.EntityFrameworkCore;
using Moq;
using Services;

namespace Test
{
    public class SubscriptionServiceTests
    {
        private readonly SubscriptionService _subService;
        private readonly Mock<MyDBContext> _dbContextMock;
        private readonly Mock<ToDoListHttpClient> _clientMock;
        public SubscriptionServiceTests()
        {
            _dbContextMock = new Mock<MyDBContext>(new DbContextOptions<MyDBContext>());
            _clientMock = new Mock<ToDoListHttpClient>();
            _subService = new(_dbContextMock.Object, _clientMock.Object);
        }

        [Fact]
        public async void Create_ReturnsNewSubscription_WhenRequestIsGood()
        {
            // Arrange
            SubscriptionsRequest subRequest = new()
            {
                MainTaskIdTopic = 95,
                SubTaskIdSubscriber = 115
            };

            Subscriptions newSub = new Subscriptions()
            {
                Id = 555,
                MainTaskIdTopic = subRequest.MainTaskIdTopic,
                SubTaskIdSubscriber = subRequest.SubTaskIdSubscriber
            };

            _dbContextMock.Setup(db => db.Subscriptions.AddAsync(newSub, default));
            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _subService.Create(subRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Subscriptions>();
            result.Id.Should().BeGreaterThan(0);
            result.SubTaskIdSubscriber.Should().Be(subRequest.SubTaskIdSubscriber);
            result.MainTaskIdTopic.Should().Be(subRequest.MainTaskIdTopic);
        }

    }
}
