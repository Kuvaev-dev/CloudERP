using API.Controllers.Company;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Company
{
    [TestFixture]
    public class CompanyApiControllerTests
    {
        private Mock<ICompanyRepository> _companyRepositoryMock;
        private Mock<IFileService> _fileServiceMock;
        private Mock<IFileAdapterFactory> _fileAdapterFactoryMock;
        private CompanyApiController _controller;
        private List<Domain.Models.Company> _testCompanies;
        private Domain.Models.Company _testCompany;
        private const string DEFAULT_COMPANY_LOGO_PATH = "~/CompanyLogo/erp-logo.png";

        [SetUp]
        public void SetUp()
        {
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _fileAdapterFactoryMock = new Mock<IFileAdapterFactory>();
            _controller = new CompanyApiController(
                _companyRepositoryMock.Object,
                _fileServiceMock.Object,
                _fileAdapterFactoryMock.Object);
            _testCompanies = new List<Domain.Models.Company>
            {
                new Domain.Models.Company
                {
                    CompanyID = 1,
                    Name = "Company One",
                    Logo = DEFAULT_COMPANY_LOGO_PATH,
                    Description = "First company description"
                },
                new Domain.Models.Company
                {
                    CompanyID = 2,
                    Name = "Company Two",
                    Logo = "/logos/company2.png",
                    Description = "Second company description"
                }
            };
            _testCompany = new Domain.Models.Company
            {
                CompanyID = 3,
                Name = "Company Three",
                Logo = null,
                Description = "Third company description"
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenCompanyRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CompanyApiController(null, _fileServiceMock.Object, _fileAdapterFactoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFileServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CompanyApiController(_companyRepositoryMock.Object, null, _fileAdapterFactoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFileAdapterFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CompanyApiController(_companyRepositoryMock.Object, _fileServiceMock.Object, null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithCompanies_WhenRepositorySucceeds()
        {
            _companyRepositoryMock.Setup(r => r.GetAllAsync())
                                  .ReturnsAsync(_testCompanies);
            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testCompanies);
            _companyRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _companyRepositoryMock.Setup(r => r.GetAllAsync())
                                  .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetAll();
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _companyRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithCompany_WhenIdExists()
        {
            var id = 1;
            _companyRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                  .ReturnsAsync(_testCompanies[0]);
            var result = await _controller.GetById(id);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testCompanies[0]);
            _companyRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var id = 999;
            _companyRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                  .ReturnsAsync((Domain.Models.Company)null);
            var result = await _controller.GetById(id);
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model cannot be null.");
            _companyRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = 1;
            var exceptionMessage = "Database error";
            _companyRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                  .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetById(id);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _companyRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            _companyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Models.Company>()))
                                  .Returns(Task.CompletedTask);
            var result = await _controller.Create(_testCompany);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testCompany.CompanyID);
            var returnedCompany = createdResult.Value as Domain.Models.Company;
            returnedCompany.Should().BeEquivalentTo(_testCompany, options => options.Excluding(c => c.Logo));
            returnedCompany.Logo.Should().Be(DEFAULT_COMPANY_LOGO_PATH);
            _companyRepositoryMock.Verify(r => r.AddAsync(It.Is<Domain.Models.Company>(c => c.Logo == DEFAULT_COMPANY_LOGO_PATH)), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Create(null);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Company>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _companyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Models.Company>()))
                                  .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Create(_testCompany);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Company>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            _companyRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Models.Company>()))
                                  .Returns(Task.CompletedTask);
            var result = await _controller.Update(_testCompany);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var returnedCompany = okResult.Value as Domain.Models.Company;
            returnedCompany.Should().BeEquivalentTo(_testCompany, options => options.Excluding(c => c.Logo));
            returnedCompany.Logo.Should().Be(DEFAULT_COMPANY_LOGO_PATH);
            _companyRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Domain.Models.Company>(c => c.Logo == DEFAULT_COMPANY_LOGO_PATH)), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Update(null);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _companyRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Company>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _companyRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Models.Company>()))
                                  .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Update(_testCompany);
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _companyRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Company>()), Times.Once());
        }

        [Test]
        public async Task Create_ShouldNotOverrideExistingLogo_WhenLogoIsProvided()
        {
            var companyWithLogo = new Domain.Models.Company
            {
                CompanyID = 4,
                Name = "Company Four",
                Logo = "/logos/company4.png",
                Description = "Fourth company description"
            };
            _companyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Models.Company>()))
                                  .Returns(Task.CompletedTask);
            var result = await _controller.Create(companyWithLogo);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            var returnedCompany = createdResult.Value as Domain.Models.Company;
            returnedCompany.Logo.Should().Be("/logos/company4.png");
            _companyRepositoryMock.Verify(r => r.AddAsync(It.Is<Domain.Models.Company>(c => c.Logo == "/logos/company4.png")), Times.Once());
        }

        [Test]
        public async Task Update_ShouldNotOverrideExistingLogo_WhenLogoIsProvided()
        {
            var companyWithLogo = new Domain.Models.Company
            {
                CompanyID = 4,
                Name = "Company Four",
                Logo = "/logos/company4.png",
                Description = "Fourth company description"
            };
            _companyRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Models.Company>()))
                                  .Returns(Task.CompletedTask);
            var result = await _controller.Update(companyWithLogo);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var returnedCompany = okResult.Value as Domain.Models.Company;
            returnedCompany.Logo.Should().Be("/logos/company4.png");
            _companyRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Domain.Models.Company>(c => c.Logo == "/logos/company4.png")), Times.Once());
        }
    }
}