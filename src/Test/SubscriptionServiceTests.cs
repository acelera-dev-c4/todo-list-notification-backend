using Domain.Models;
using Domain.Requests;
using FluentAssertions;
using Infra;
using Infra.DB;
using Microsoft.EntityFrameworkCore;
using Moq;
using Services;
using System.Net;

namespace Test
{
    public class SubscriptionServiceTests
    {
        private readonly SubscriptionService _subService;
        private readonly Mock<MyDBContext> _dbContextMock;
        private readonly Mock<IToDoListHttpClient> _clientMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        public SubscriptionServiceTests()
        {
            var options = new DbContextOptionsBuilder<MyDBContext>()
                          .UseInMemoryDatabase(databaseName: "test_db")
                          .Options;

            _dbContextMock = new Mock<MyDBContext>(options);
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _clientMock = new Mock<IToDoListHttpClient>(MockBehavior.Strict);
            _subService = new(_dbContextMock.Object, _clientMock.Object);

            _httpClientFactoryMock.Setup(f => f.CreateClient("toDoClient"))
                                  .Returns(new HttpClient());
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

            _dbContextMock.Setup(db => db.Set<Subscriptions>().AddAsync(newSub, default));
            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);
            _clientMock.Setup(c => c.SetUrlWebhook(subRequest.MainTaskIdTopic)).ReturnsAsync((new HttpResponseMessage(HttpStatusCode.OK)));

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
