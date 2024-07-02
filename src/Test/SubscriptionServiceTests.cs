using Domain.Models;
using Domain.Requests;
using FluentAssertions;
using Infra;
using Infra.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using Services;
using System.Net;

namespace Test
{
    public class SubscriptionServiceTests
    {
        private readonly SubscriptionService _subService;
        private readonly Mock<MyDBContext> _dbContextMock;
        private readonly Mock<ToDoListHttpClient> _clientMock;
        private readonly Mock<DbSet<Subscriptions>> _mockSubscriptionsDbSet;
        private readonly List<Subscriptions> _subscriptionsList;

        public SubscriptionServiceTests()
        {
            _subscriptionsList = new()
            {
                new Subscriptions { Id = 1, MainTaskIdTopic = 15, SubTaskIdSubscriber = 78 },
                new Subscriptions { Id = 2, MainTaskIdTopic = 21, SubTaskIdSubscriber = 99 },
                new Subscriptions { Id = 3, MainTaskIdTopic = 7, SubTaskIdSubscriber = 4 },
                new Subscriptions { Id = 4, MainTaskIdTopic = 55, SubTaskIdSubscriber = 98 },
                new Subscriptions { Id = 5, MainTaskIdTopic = 82, SubTaskIdSubscriber = 37 },
                new Subscriptions { Id = 6, MainTaskIdTopic = 2, SubTaskIdSubscriber = 26 }
            };

            var options = new DbContextOptionsBuilder<MyDBContext>()
                              .UseInMemoryDatabase(databaseName: "Mock_DB")
                              .Options;

            _dbContextMock = new Mock<MyDBContext>(options);

            _mockSubscriptionsDbSet = _subscriptionsList.AsQueryable().BuildMockDbSet();
            _dbContextMock.Setup(db => db.Subscriptions).Returns(_mockSubscriptionsDbSet.Object);

            var _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock.Setup(f => f.CreateClient("toDoClient"))
                                    .Returns(new HttpClient
                                    {
                                        BaseAddress = new Uri("https://testurl.com/"),
                                        Timeout = TimeSpan.FromSeconds(60)
                                    });

            var inMemorySettings = new Dictionary<string, string>
            {
                {"ToDoListApi:BaseUrl", "https://testurl.com/"}
            };
            IConfiguration configuration = new ConfigurationBuilder()
                                           .AddInMemoryCollection(inMemorySettings!)
                                           .Build();

            _clientMock = new Mock<ToDoListHttpClient>(_httpClientFactoryMock.Object, configuration);
            _clientMock.Setup(c => c.SetUrlWebhook(1)).ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK });

            _subService = new(_dbContextMock.Object, _clientMock.Object);
        }

        [InlineData(21, 25)]
        [InlineData(1, int.MaxValue)]
        [InlineData(int.MaxValue, 1)]
        [InlineData(9999, 7777)]
        [Theory]
        public async Task Create_ReturnsNewSubscription_WhenRequestIsGood(int mainTaskId, int subTaskId)
        {
            // Arrange
            SubscriptionsRequest subRequest = new()
            {
                MainTaskIdTopic = mainTaskId,
                SubTaskIdSubscriber = subTaskId
            };

            _mockSubscriptionsDbSet.Setup(m => m.AddAsync(It.IsAny<Subscriptions>(), It.IsAny<CancellationToken>()))
                                   .Callback<Subscriptions, CancellationToken>((sub, ct) =>
                                   {
                                       sub.Id = _subscriptionsList.Count() + 1;
                                       _subscriptionsList.Add(sub);
                                   })
                                   .ReturnsAsync((Subscriptions sub, CancellationToken ct) =>
                                   {
                                       var entry = _dbContextMock.Object.Entry(sub);
                                       return entry;
                                   });

            _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(1);

            // Act
            var result = await _subService.Create(subRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Subscriptions>();
            result.Id.Should().Be(_subscriptionsList.Count());
            result.SubTaskIdSubscriber.Should().Be(subRequest.SubTaskIdSubscriber);
            result.MainTaskIdTopic.Should().Be(subRequest.MainTaskIdTopic);
            _mockSubscriptionsDbSet.Verify(m => m.AddAsync(It.IsAny<Subscriptions>(), It.IsAny<CancellationToken>()), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [InlineData(15)]
        [InlineData(21)]
        [InlineData(7)]
        [InlineData(55)]
        [InlineData(82)]
        [InlineData(2)]
        [Theory]
        public async Task GetSubscriptionByMainTaskId_ReturnsSubscriptions_WhenValidId(int mainTaskId)
        {
            // Arrange            
            var expectedSubscriptions = _subscriptionsList.Where(s => s.MainTaskIdTopic == mainTaskId).ToList();

            // Act
            var result = await _subService.GetSubscriptionByMainTaskId(mainTaskId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Contain(s => s.MainTaskIdTopic == mainTaskId);
            result.Should().BeOfType<List<Subscriptions>>();
            result.Should().BeEquivalentTo(expectedSubscriptions);
        }

        [InlineData(78)]
        [InlineData(99)]
        [InlineData(4)]
        [InlineData(98)]
        [InlineData(37)]
        [InlineData(26)]
        [Theory]
        public async Task GetSubscriptionBySubTaskId_ReturnsSubscription_WhenValidId(int subTaskId)
        {
            // Arrange            
            var expectedSubscriptions = _subscriptionsList.Where(s => s.SubTaskIdSubscriber == subTaskId).FirstOrDefault();

            // Act
            var result = await _subService.GetSubscriptionBySubTaskId(subTaskId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Subscriptions>();
            result.Should().BeEquivalentTo(expectedSubscriptions);
        }
    }
}
