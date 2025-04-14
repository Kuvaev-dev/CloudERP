using API.Controllers.Stock;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Controllers.API.Stock
{
    [TestFixture]
    public class StockApiControllerTests
    {
        private Mock<IStockRepository> _stockRepositoryMock;
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private Mock<IProductQualityService> _productQualityServiceMock;
        private StockApiController _controller;
        private Domain.Models.Stock _testStock;
        private List<Domain.Models.Stock> _testStocks;
        private List<ProductQuality> _testProductQualities;

        [SetUp]
        public void SetUp()
        {
            _stockRepositoryMock = new Mock<IStockRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _productQualityServiceMock = new Mock<IProductQualityService>();
            _controller = new StockApiController(
                _stockRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _productQualityServiceMock.Object);

            _testStock = new Domain.Models.Stock
            {
                ProductID = 1,
                CategoryID = 1,
                CategoryName = "Electronics",
                ProductName = "Laptop",
                Quantity = 100,
                SaleUnitPrice = 1000.0,
                CurrentPurchaseUnitPrice = 800.0,
                ExpiryDate = new DateTime(2026, 1, 1),
                Manufacture = new DateTime(2024, 1, 1),
                StockTreshHoldQuantity = 20,
                Description = "High-end laptop",
                UserID = 1,
                UserName = "test_user",
                BranchID = 1,
                BranchName = "Main Branch",
                CompanyID = 1,
                CompanyName = "Test Company",
                IsActive = true
            };

            _testStocks = new List<Domain.Models.Stock>
            {
                _testStock,
                new Domain.Models.Stock
                {
                    ProductID = 2,
                    CategoryID = 2,
                    CategoryName = "Clothing",
                    ProductName = "T-Shirt",
                    Quantity = 200,
                    SaleUnitPrice = 20.0,
                    CurrentPurchaseUnitPrice = 15.0,
                    ExpiryDate = new DateTime(2026, 6, 1),
                    Manufacture = new DateTime(2024, 2, 1),
                    StockTreshHoldQuantity = 50,
                    Description = "Cotton T-Shirt",
                    UserID = 1,
                    UserName = "test_user",
                    BranchID = 1,
                    BranchName = "Main Branch",
                    CompanyID = 1,
                    CompanyName = "Test Company",
                    IsActive = true
                }
            };

            _testProductQualities = new List<ProductQuality>
            {
                new ProductQuality
                {
                    ProductID = 1,
                    ProductName = "Laptop",
                    StockTreshHoldQuantity = 100,
                    ExpiryDate = new DateTime(2026, 1, 1),
                    Manufacture = new DateTime(2024, 1, 1)
                },
                new ProductQuality
                {
                    ProductID = 2,
                    ProductName = "T-Shirt",
                    StockTreshHoldQuantity = 200,
                    ExpiryDate = new DateTime(2026, 6, 1),
                    Manufacture = new DateTime(2024, 1, 1)
                }
            };

            // Налаштування ModelState для контролера
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenStockRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new StockApiController(null, _categoryRepositoryMock.Object, _productQualityServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenCategoryRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new StockApiController(_stockRepositoryMock.Object, null, _productQualityServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenProductQualityServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new StockApiController(_stockRepositoryMock.Object, _categoryRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithStocks_WhenStocksExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _stockRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                .ReturnsAsync(_testStocks);

            // Act
            var result = await _controller.GetAll(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testStocks);
            _stockRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoStocksExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _stockRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                .ReturnsAsync(new List<Domain.Models.Stock>());

            // Act
            var result = await _controller.GetAll(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.As<IEnumerable<Domain.Models.Stock>>().Should().BeEmpty();
            _stockRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _stockRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAll(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _stockRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithStock_WhenStockExists()
        {
            // Arrange
            int id = 1;
            _stockRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                .ReturnsAsync(_testStock);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testStock);
            _stockRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenStockDoesNotExist()
        {
            // Arrange
            int id = 999;
            _stockRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                .ReturnsAsync((Domain.Models.Stock)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _stockRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Retrieval error";
            _stockRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _stockRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtRoute_WhenModelIsValidAndStockDoesNotExist()
        {
            // Arrange
            _stockRepositoryMock.Setup(r => r.GetByProductNameAsync(_testStock.CompanyID, _testStock.BranchID, _testStock.ProductName))
                                .ReturnsAsync((Domain.Models.Stock)null);
            _stockRepositoryMock.Setup(r => r.AddAsync(_testStock))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(_testStock);

            // Assert
            var createdResult = result.Result as CreatedAtRouteResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.RouteName.Should().Be("GetById");
            createdResult.RouteValues["id"].Should().Be(_testStock.ProductID);
            createdResult.Value.Should().BeEquivalentTo(_testStock);
            _stockRepositoryMock.Verify(r => r.GetByProductNameAsync(_testStock.CompanyID, _testStock.BranchID, _testStock.ProductName), Times.Once());
            _stockRepositoryMock.Verify(r => r.AddAsync(_testStock), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("ProductName", "Product Name is required.");
            var invalidStock = new Domain.Models.Stock { ProductID = 1 }; // ProductName is null

            // Act
            var result = await _controller.Create(invalidStock);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Invalid data.");
            _stockRepositoryMock.Verify(r => r.GetByProductNameAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
            _stockRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Stock>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnConflict_WhenStockAlreadyExists()
        {
            // Arrange
            _stockRepositoryMock.Setup(r => r.GetByProductNameAsync(_testStock.CompanyID, _testStock.BranchID, _testStock.ProductName))
                                .ReturnsAsync(_testStock);

            // Act
            var result = await _controller.Create(_testStock);

            // Assert
            var conflictResult = result.Result as ConflictResult;
            conflictResult.Should().NotBeNull();
            conflictResult.StatusCode.Should().Be(409);
            _stockRepositoryMock.Verify(r => r.GetByProductNameAsync(_testStock.CompanyID, _testStock.BranchID, _testStock.ProductName), Times.Once());
            _stockRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Stock>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _stockRepositoryMock.Setup(r => r.GetByProductNameAsync(_testStock.CompanyID, _testStock.BranchID, _testStock.ProductName))
                                .ReturnsAsync((Domain.Models.Stock)null);
            _stockRepositoryMock.Setup(r => r.AddAsync(_testStock))
                                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Create(_testStock);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Contain(exceptionMessage);
            _stockRepositoryMock.Verify(r => r.GetByProductNameAsync(_testStock.CompanyID, _testStock.BranchID, _testStock.ProductName), Times.Once());
            _stockRepositoryMock.Verify(r => r.AddAsync(_testStock), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenStockIsValid()
        {
            // Arrange
            int id = 1;
            _stockRepositoryMock.Setup(r => r.UpdateAsync(_testStock))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, _testStock);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testStock);
            _stockRepositoryMock.Verify(r => r.UpdateAsync(_testStock), Times.Once());
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
            _stockRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Stock>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            int id = 999;
            _testStock.ProductID = 1;

            // Act
            var result = await _controller.Update(id, _testStock);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _stockRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Stock>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Update error";
            _stockRepositoryMock.Setup(r => r.UpdateAsync(_testStock))
                                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Update(id, _testStock);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _stockRepositoryMock.Verify(r => r.UpdateAsync(_testStock), Times.Once());
        }

        [Test]
        public async Task GetProductQuality_ShouldReturnOkWithQualities_WhenQualitiesExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _productQualityServiceMock.Setup(s => s.GetAllProductsQualityAsync(branchId, companyId))
                                      .ReturnsAsync(_testProductQualities);

            // Act
            var result = await _controller.GetProductQuality(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testProductQualities);
            _productQualityServiceMock.Verify(s => s.GetAllProductsQualityAsync(branchId, companyId), Times.Once());
        }

        [Test]
        public async Task GetProductQuality_ShouldReturnOkWithEmptyList_WhenNoQualitiesExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _productQualityServiceMock.Setup(s => s.GetAllProductsQualityAsync(branchId, companyId))
                                      .ReturnsAsync(new List<ProductQuality>());

            // Act
            var result = await _controller.GetProductQuality(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.As<IEnumerable<ProductQuality>>().Should().BeEmpty();
            _productQualityServiceMock.Verify(s => s.GetAllProductsQualityAsync(branchId, companyId), Times.Once());
        }

        [Test]
        public async Task GetProductQuality_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Quality retrieval error";
            _productQualityServiceMock.Setup(s => s.GetAllProductsQualityAsync(branchId, companyId))
                                      .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetProductQuality(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _productQualityServiceMock.Verify(s => s.GetAllProductsQualityAsync(branchId, companyId), Times.Once());
        }
    }
}