using API.Controllers.Utilities;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Controllers.API.Utilities
{
    [TestFixture]
    public class SupportApiControllerTests
    {
        private Mock<ISupportTicketRepository> _supportTicketRepositoryMock;
        private SupportApiController _controller;
        private SupportTicket _testTicket;
        private List<SupportTicket> _testTickets;

        [SetUp]
        public void SetUp()
        {
            _supportTicketRepositoryMock = new Mock<ISupportTicketRepository>();
            _controller = new SupportApiController(_supportTicketRepositoryMock.Object);

            _testTicket = new SupportTicket
            {
                TicketID = 1,
                Subject = "Test Issue",
                Name = "John Doe",
                Email = "john.doe@example.com",
                Message = "This is a test message",
                DateCreated = new DateTime(2024, 1, 1),
                AdminResponse = null,
                RespondedBy = null,
                ResponseDate = null,
                IsResolved = false,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };

            _testTickets = new List<SupportTicket>
            {
                _testTicket,
                new SupportTicket
                {
                    TicketID = 2,
                    Subject = "Another Issue",
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Message = "This is another test message",
                    DateCreated = new DateTime(2024, 1, 2),
                    AdminResponse = null,
                    RespondedBy = null,
                    ResponseDate = null,
                    IsResolved = false,
                    CompanyID = 1,
                    BranchID = 1,
                    UserID = 2
                }
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SupportApiController(null));
        }

        [Test]
        public async Task GetUserTickets_ShouldReturnOkWithTickets_WhenTicketsExist()
        {
            // Arrange
            int userId = 1;
            _supportTicketRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                                       .ReturnsAsync(_testTickets);

            // Act
            var result = await _controller.GetUserTickets(userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testTickets);
            _supportTicketRepositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once());
        }

        [Test]
        public async Task GetUserTickets_ShouldReturnNotFound_WhenNoTicketsExist()
        {
            // Arrange
            int userId = 999;
            _supportTicketRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                                       .ReturnsAsync((IEnumerable<SupportTicket>)null);

            // Act
            var result = await _controller.GetUserTickets(userId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _supportTicketRepositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once());
        }

        [Test]
        public async Task GetUserTickets_ShouldReturnNotFound_WhenEmptyTicketsList()
        {
            // Arrange
            int userId = 999;
            _supportTicketRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                                       .ReturnsAsync(new List<SupportTicket>());

            // Act
            var result = await _controller.GetUserTickets(userId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _supportTicketRepositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once());
        }

        [Test]
        public async Task SubmitTicket_ShouldReturnOkWithMessage_WhenTicketIsValid()
        {
            // Arrange
            _supportTicketRepositoryMock.Setup(r => r.AddAsync(_testTicket))
                                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SubmitTicket(_testTicket);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(new { message = "Заявка успешно отправлена" });
            _supportTicketRepositoryMock.Verify(r => r.AddAsync(_testTicket), Times.Once());
        }

        [Test]
        public async Task SubmitTicket_ShouldReturnOk_WhenTicketIsNull()
        {
            // Arrange
            _supportTicketRepositoryMock.Setup(r => r.AddAsync(null))
                                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SubmitTicket(null);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(new { message = "Заявка успешно отправлена" });
            _supportTicketRepositoryMock.Verify(r => r.AddAsync(null), Times.Once());
        }

        [Test]
        public async Task SubmitTicket_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _supportTicketRepositoryMock.Setup(r => r.AddAsync(_testTicket))
                                       .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.SubmitTicket(_testTicket);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supportTicketRepositoryMock.Verify(r => r.AddAsync(_testTicket), Times.Once());
        }

        [Test]
        public async Task GetAdminList_ShouldReturnOkWithTickets_WhenTicketsExist()
        {
            // Arrange
            _supportTicketRepositoryMock.Setup(r => r.GetAllAsync())
                                       .ReturnsAsync(_testTickets);

            // Act
            var result = await _controller.GetAdminList();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testTickets);
            _supportTicketRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAdminList_ShouldReturnNotFound_WhenNoTicketsExist()
        {
            // Arrange
            _supportTicketRepositoryMock.Setup(r => r.GetAllAsync())
                                       .ReturnsAsync((IEnumerable<SupportTicket>)null);

            // Act
            var result = await _controller.GetAdminList();

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _supportTicketRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAdminList_ShouldReturnNotFound_WhenEmptyTicketsList()
        {
            // Arrange
            _supportTicketRepositoryMock.Setup(r => r.GetAllAsync())
                                       .ReturnsAsync(new List<SupportTicket>());

            // Act
            var result = await _controller.GetAdminList();

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _supportTicketRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task ResolveTicket_ShouldReturnOkWithMessage_WhenTicketExists()
        {
            // Arrange
            int ticketId = 1;
            string responseMessage = "Issue resolved";
            _supportTicketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId))
                                       .ReturnsAsync(_testTicket);
            _supportTicketRepositoryMock.Setup(r => r.UpdateAsync(It.Is<SupportTicket>(t =>
                t.TicketID == ticketId &&
                t.AdminResponse == responseMessage &&
                t.RespondedBy == "Admin" &&
                t.IsResolved == true)))
                                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ResolveTicket(ticketId, responseMessage);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(new { message = "Тикет успешно обработан" });
            _supportTicketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once());
            _supportTicketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SupportTicket>()), Times.Once());
        }

        [Test]
        public async Task ResolveTicket_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            int ticketId = 999;
            string responseMessage = "Issue resolved";
            _supportTicketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId))
                                       .ReturnsAsync((SupportTicket)null);

            // Act
            var result = await _controller.ResolveTicket(ticketId, responseMessage);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _supportTicketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once());
            _supportTicketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SupportTicket>()), Times.Never());
        }

        [Test]
        public async Task ResolveTicket_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int ticketId = 1;
            string responseMessage = "Issue resolved";
            var exceptionMessage = "Database error";
            _supportTicketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId))
                                       .ReturnsAsync(_testTicket);
            _supportTicketRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SupportTicket>()))
                                       .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ResolveTicket(ticketId, responseMessage);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supportTicketRepositoryMock.Verify(r => r.GetByIdAsync(ticketId), Times.Once());
            _supportTicketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SupportTicket>()), Times.Once());
        }
    }
}