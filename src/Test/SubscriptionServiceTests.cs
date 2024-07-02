using Domain.Models;
using Domain.Requests;
using FluentAssertions;
using Infra;
using Infra.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
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
        private readonly Mock<DbSet<Subscriptions>> mockSubscriptionsDbSet;

        public SubscriptionServiceTests()
        {
            var options = new DbContextOptionsBuilder<MyDBContext>()
                              .UseInMemoryDatabase(databaseName: "Mock_DB")
                              .Options;

            _dbContextMock = new Mock<MyDBContext>(options);

            var subscriptions = new List<Subscriptions>
            {
                new Subscriptions { Id = 1, MainTaskIdTopic = 15, SubTaskIdSubscriber = 78 },
                new Subscriptions { Id = 2, MainTaskIdTopic = 21, SubTaskIdSubscriber = 99 },
                new Subscriptions { Id = 3, MainTaskIdTopic = 7, SubTaskIdSubscriber = 4 },
                new Subscriptions { Id = 4, MainTaskIdTopic = 55, SubTaskIdSubscriber = 98 },
                new Subscriptions { Id = 5, MainTaskIdTopic = 82, SubTaskIdSubscriber = 37 },
                new Subscriptions { Id = 6, MainTaskIdTopic = 2, SubTaskIdSubscriber = 26 }
            }.AsQueryable();

            mockSubscriptionsDbSet = new();
            mockSubscriptionsDbSet.As<IQueryable<Subscriptions>>().Setup(m => m.Provider).Returns(subscriptions.Provider);
            mockSubscriptionsDbSet.As<IQueryable<Subscriptions>>().Setup(m => m.Expression).Returns(subscriptions.Expression);
            mockSubscriptionsDbSet.As<IQueryable<Subscriptions>>().Setup(m => m.ElementType).Returns(subscriptions.ElementType);
            mockSubscriptionsDbSet.As<IQueryable<Subscriptions>>().Setup(m => m.GetEnumerator()).Returns(subscriptions.GetEnumerator());

            _dbContextMock.Setup(db => db.Subscriptions).Returns(mockSubscriptionsDbSet.Object);

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

        [Fact]
        public async void Create_ReturnsNewSubscription_WhenRequestIsGood()
        {
            // Arrange
            SubscriptionsRequest subRequest = new()
            {
                MainTaskIdTopic = 95,
                SubTaskIdSubscriber = 115
            };

            Subscriptions expectedNewSub = new()
            {
                MainTaskIdTopic = subRequest.MainTaskIdTopic,
                SubTaskIdSubscriber = subRequest.SubTaskIdSubscriber
            };

            var entry = new EntityEntry<Subscriptions>(new InternalEntityEntry(stateManager: default, entityType: typeof(Subscriptions), entity: expectedNewSub));

            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);
            _dbContextMock.Setup(db => db.Subscriptions.AddAsync(It.IsAny<Subscriptions>(), It.IsAny<CancellationToken>()))
                .Returns(entry);






            // Act
            var result = await _subService.Create(subRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Subscriptions>();
            result.Id.Should().BeGreaterThan(0);
            result.SubTaskIdSubscriber.Should().Be(expectedNewSub.SubTaskIdSubscriber);
            result.MainTaskIdTopic.Should().Be(expectedNewSub.MainTaskIdTopic);
        }
    }
}
