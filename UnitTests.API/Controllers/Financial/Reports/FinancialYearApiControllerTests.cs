using API.Controllers.Financial.Reports;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Financial.Reports
{
    [TestFixture]
    public class FinancialYearApiControllerTests
    {
        private Mock<IFinancialYearRepository> _financialYearRepositoryMock;
        private FinancialYearApiController _controller;
        private List<FinancialYear> _testFinancialYears;
        private FinancialYear _testFinancialYear;

        [SetUp]
        public void SetUp()
        {
            _financialYearRepositoryMock = new Mock<IFinancialYearRepository>();
            _controller = new FinancialYearApiController(_financialYearRepositoryMock.Object);

            _testFinancialYears = new List<FinancialYear>
            {
                new FinancialYear
                {
                    FinancialYearID = 1,
                    FinancialYearName = "2024",
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2024, 12, 31),
                    IsActive = true,
                    UserID = 1,
                    UserName = "user1"
                },
                new FinancialYear
                {
                    FinancialYearID = 2,
                    FinancialYearName = "2025",
                    StartDate = new DateTime(2025, 1, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    IsActive = false,
                    UserID = 2,
                    UserName = "user2"
                }
            };

            _testFinancialYear = new FinancialYear
            {
                FinancialYearID = 3,
                FinancialYearName = "2026",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 12, 31),
                IsActive = true,
                UserID = 3,
                UserName = "user3"
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new FinancialYearApiController(null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithFinancialYears_WhenRepositorySucceeds()
        {
            // Arrange
            _financialYearRepositoryMock.Setup(r => r.GetAllAsync())
                                        .ReturnsAsync(_testFinancialYears);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testFinancialYears);
            _financialYearRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _financialYearRepositoryMock.Setup(r => r.GetAllAsync())
                                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithFinancialYear_WhenIdExists()
        {
            // Arrange
            var id = 1;
            _financialYearRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                        .ReturnsAsync(_testFinancialYears[0]);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testFinancialYears[0]);
            _financialYearRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            var id = 999;
            _financialYearRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                        .ReturnsAsync((FinancialYear)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _financialYearRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = 1;
            var exceptionMessage = "Database error";
            _financialYearRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            // Arrange
            _financialYearRepositoryMock.Setup(r => r.AddAsync(It.IsAny<FinancialYear>()))
                                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(_testFinancialYear);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testFinancialYear.FinancialYearID);
            createdResult.Value.Should().BeEquivalentTo(_testFinancialYear);
            _financialYearRepositoryMock.Verify(r => r.AddAsync(It.Is<FinancialYear>(f => f.FinancialYearID == _testFinancialYear.FinancialYearID)), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Act
            var result = await _controller.Create(null);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model not found.");
            _financialYearRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FinancialYear>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _financialYearRepositoryMock.Setup(r => r.AddAsync(It.IsAny<FinancialYear>()))
                                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Create(_testFinancialYear);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FinancialYear>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValidAndIdsMatch()
        {
            // Arrange
            var id = _testFinancialYear.FinancialYearID;
            _financialYearRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<FinancialYear>()))
                                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, _testFinancialYear);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testFinancialYear);
            _financialYearRepositoryMock.Verify(r => r.UpdateAsync(It.Is<FinancialYear>(f => f.FinancialYearID == _testFinancialYear.FinancialYearID)), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Act
            var result = await _controller.Update(1, null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _financialYearRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<FinancialYear>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var id = 999;

            // Act
            var result = await _controller.Update(id, _testFinancialYear);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _financialYearRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<FinancialYear>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = _testFinancialYear.FinancialYearID;
            var exceptionMessage = "Database error";
            _financialYearRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<FinancialYear>()))
                                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Update(id, _testFinancialYear);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<FinancialYear>()), Times.Once());
        }
    }
}