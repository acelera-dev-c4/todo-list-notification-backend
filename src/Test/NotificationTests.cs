using Api.Controllers;
using Domain.Models;
using Domain.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services;

namespace Tests
{
    public class NotificationTests
    {
        private readonly NotificationController _controller;
        private readonly Mock<INotificationService> _mockContext;

        public NotificationTests()
        {
            _mockContext = new Mock<INotificationService>();
            _controller = new NotificationController(_mockContext.Object);
        }

        [Fact]
        public async Task Post_CreatesNewNotificaion_Success()
        {
            // Arrange
            var notificationRequest = new NotificationRequest
            {
                SubscriptionId = 1,
                Message = "Tarefa Compras no supermercado Completa!",
                Readed = false,
                UserId = 2
            };
            var notification = new Notifications
            {
                Id = 1,
                SubscriptionId = 1,
                Message = "Tarefa Compras no supermercado Completa!",
                Readed = false,
                UserId = 2
            };

            _mockContext.Setup(n => n.Create(notificationRequest)).ReturnsAsync(notification);

            // Act
            var result = await _controller.Create(notificationRequest) as OkObjectResult;
            var item = result?.Value as Notifications;

            // Assert
            result!.StatusCode.Should().Be(200);
            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(notification);
        }

        [Fact]
        public async Task Get_ReturnsAllOrderedByDescendingId_Success()
        {
            // Arrange 

            var notifications = new List<Notifications>
            {
                new Notifications {Id = 1, SubscriptionId = 1, Message = "Tarefa Compras no supermercado Completa!", Readed = false, UserId = 2},
                new Notifications {Id = 2, SubscriptionId = 2, Message = "Tarefa Preparação para apresentação Completa!", Readed = false, UserId = 3},
                new Notifications {Id = 3, SubscriptionId = 3, Message = "Tarefa Atualização de relatórios Completa!", Readed = true, UserId = 1},
            };

            _mockContext.Setup(n => n.List()).ReturnsAsync(notifications);

            // Act

            var result = await _controller.Get();

            // Assert

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var returnedNotifications = okResult!.Value as List<Notifications>;
            returnedNotifications.Should().BeEquivalentTo(notifications);
        }

        [Fact]
        public async Task GetByUserId_ReturnsAll_NotificationsForSpecifiedUser()
        {
            // Arrange
            int userId = 2;
            var notifications = new List<Notifications>
            {
                new Notifications {Id = 1, SubscriptionId = 1, Message = "Tarefa Compras no supermercado Completa!", Readed = false, UserId = 2},
                new Notifications {Id = 4, SubscriptionId = 4, Message = "Tarefa Limpeza do escritório Completa!", Readed = false, UserId = 2},
            };

            _mockContext.Setup(n => n.GetByUserId(userId)).ReturnsAsync(notifications);

            // Act
            var result = await _controller.GetByUserId(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var returnedNotifications = okResult!.Value as List<Notifications>;
            returnedNotifications.Should().BeEquivalentTo(notifications);
        }

    }
}