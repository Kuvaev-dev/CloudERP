using API.Controllers.Inventory;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Controllers.API.Stock
{
    [TestFixture]
    public class CategoryApiControllerTests
    {
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private CategoryApiController _controller;
        private Category _testCategory;
        private List<Category> _testCategories;

        [SetUp]
        public void SetUp()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _controller = new CategoryApiController(_categoryRepositoryMock.Object);

            _testCategory = new Category
            {
                CategoryID = 1,
                CategoryName = "Electronics",
                BranchID = 1,
                BranchName = "Main Branch",
                CompanyID = 1,
                CompanyName = "Test Company",
                UserID = 1,
                UserName = "test_user"
            };

            _testCategories = new List<Category>
            {
                _testCategory,
                new Category
                {
                    CategoryID = 2,
                    CategoryName = "Clothing",
                    BranchID = 1,
                    BranchName = "Main Branch",
                    CompanyID = 1,
                    CompanyName = "Test Company",
                    UserID = 1,
                    UserName = "test_user"
                }
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CategoryApiController(null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithCategories_WhenCategoriesExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _categoryRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                   .ReturnsAsync(_testCategories);

            // Act
            var result = await _controller.GetAll(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testCategories);
            _categoryRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _categoryRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                   .ReturnsAsync(new List<Category>());

            // Act
            var result = await _controller.GetAll(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.As<IEnumerable<Category>>().Should().BeEmpty();
            _categoryRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithCategory_WhenCategoryExists()
        {
            // Arrange
            int id = 1;
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync(_testCategory);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testCategory);
            _categoryRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            int id = 999;
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync((Category)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _categoryRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenCategoryIsValid()
        {
            // Arrange
            _categoryRepositoryMock.Setup(r => r.AddAsync(_testCategory))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(_testCategory);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testCategory.CategoryID);
            createdResult.Value.Should().BeEquivalentTo(_testCategory);
            _categoryRepositoryMock.Verify(r => r.AddAsync(_testCategory), Times.Once());
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
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _categoryRepositoryMock.Setup(r => r.AddAsync(_testCategory))
                                   .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Create(_testCategory);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _categoryRepositoryMock.Verify(r => r.AddAsync(_testCategory), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenCategoryIsValid()
        {
            // Arrange
            int id = 1;
            _categoryRepositoryMock.Setup(r => r.UpdateAsync(_testCategory))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, _testCategory);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testCategory);
            _categoryRepositoryMock.Verify(r => r.UpdateAsync(_testCategory), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _controller.Update(id, null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _categoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            int id = 999;
            _testCategory.CategoryID = 1;

            // Act
            var result = await _controller.Update(id, _testCategory);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _categoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Update error";
            _categoryRepositoryMock.Setup(r => r.UpdateAsync(_testCategory))
                                   .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Update(id, _testCategory);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _categoryRepositoryMock.Verify(r => r.UpdateAsync(_testCategory), Times.Once());
        }
    }
}