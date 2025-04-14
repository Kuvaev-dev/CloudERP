using API.Controllers.User.Users;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utils.Helpers;

namespace UnitTests.Controllers.API.User.Users
{
    [TestFixture]
    public class UserApiControllerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<PasswordHelper> _passwordHelperMock;
        private UserApiController _controller;
        private Domain.Models.User _testUser;
        private List<Domain.Models.User> _testUsers;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHelperMock = new Mock<PasswordHelper>();
            _controller = new UserApiController(_userRepositoryMock.Object, _passwordHelperMock.Object);

            _testUser = new Domain.Models.User
            {
                UserID = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                ContactNo = "1234567890",
                UserName = "johndoe",
                Password = "hashedpassword",
                Salt = "somesalt",
                UserTypeID = 1,
                IsActive = true,
                UserTypeName = "Admin",
                BranchName = "Main Branch"
            };

            _testUsers = new List<Domain.Models.User>
            {
                _testUser,
                new Domain.Models.User
                {
                    UserID = 2,
                    FullName = "Jane Smith",
                    Email = "jane.smith@example.com",
                    ContactNo = "0987654321",
                    UserName = "janesmith",
                    Password = "hashedpassword2",
                    Salt = "anothersalt",
                    UserTypeID = 2,
                    IsActive = true,
                    UserTypeName = "Employee",
                    BranchName = "Main Branch"
                }
            };

            // Налаштування ModelState для контролера
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenUserRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserApiController(null, _passwordHelperMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenPasswordHelperIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserApiController(_userRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithUsers_WhenUsersExist()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetAllAsync())
                               .ReturnsAsync(_testUsers);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUsers);
            _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoUsersExist()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetAllAsync())
                               .ReturnsAsync(new List<Domain.Models.User>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.As<IEnumerable<Domain.Models.User>>().Should().BeEmpty();
            _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _userRepositoryMock.Setup(r => r.GetAllAsync())
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetByBranch_ShouldReturnOkWithUsers_WhenUsersExist()
        {
            // Arrange
            int companyId = 1, branchTypeId = 1, branchId = 1;
            _userRepositoryMock.Setup(r => r.GetByBranchAsync(companyId, branchTypeId, branchId))
                               .ReturnsAsync(_testUsers);

            // Act
            var result = await _controller.GetByBranch(companyId, branchTypeId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUsers);
            _userRepositoryMock.Verify(r => r.GetByBranchAsync(companyId, branchTypeId, branchId), Times.Once());
        }

        [Test]
        public async Task GetByBranch_ShouldReturnOkWithEmptyList_WhenNoUsersExist()
        {
            // Arrange
            int companyId = 1, branchTypeId = 1, branchId = 1;
            _userRepositoryMock.Setup(r => r.GetByBranchAsync(companyId, branchTypeId, branchId))
                               .ReturnsAsync(new List<Domain.Models.User>());

            // Act
            var result = await _controller.GetByBranch(companyId, branchTypeId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.As<IEnumerable<Domain.Models.User>>().Should().BeEmpty();
            _userRepositoryMock.Verify(r => r.GetByBranchAsync(companyId, branchTypeId, branchId), Times.Once());
        }

        [Test]
        public async Task GetByBranch_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchTypeId = 1, branchId = 1;
            var exceptionMessage = "Branch retrieval error";
            _userRepositoryMock.Setup(r => r.GetByBranchAsync(companyId, branchTypeId, branchId))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetByBranch(companyId, branchTypeId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userRepositoryMock.Verify(r => r.GetByBranchAsync(companyId, branchTypeId, branchId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithUser_WhenUserExists()
        {
            // Arrange
            int id = 1;
            _userRepositoryMock.Setup(r => r.GetByIdAsync(id))
                               .ReturnsAsync(_testUser);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUser);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int id = 999;
            _userRepositoryMock.Setup(r => r.GetByIdAsync(id))
                               .ReturnsAsync((Domain.Models.User)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _userRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Retrieval error";
            _userRepositoryMock.Setup(r => r.GetByIdAsync(id))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenUserIsValid()
        {
            // Arrange
            var plainPassword = "password123";
            var hashedPassword = "hashedpassword";
            var salt = "somesalt";
            var userToCreate = new Domain.Models.User
            {
                UserID = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                ContactNo = "1234567890",
                UserName = "johndoe",
                Password = plainPassword,
                UserTypeID = 1,
                IsActive = true
            };
            _passwordHelperMock.Setup(p => PasswordHelper.HashPassword(plainPassword, out salt))
                               .Returns(hashedPassword);
            _userRepositoryMock.Setup(r => r.AddAsync(It.Is<Domain.Models.User>(u => u.Password == hashedPassword && u.Salt == salt)))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(userToCreate);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(userToCreate.UserID);
            createdResult.Value.Should().BeEquivalentTo(userToCreate, options => options.Excluding(u => u.Password).Excluding(u => u.Salt));
            _passwordHelperMock.Verify(p => PasswordHelper.HashPassword(plainPassword, out salt), Times.Once());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.User>()), Times.Once());
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
            _passwordHelperMock.Verify(p => PasswordHelper.HashPassword(It.IsAny<string>(), out It.Ref<string>.IsAny), Times.Never());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.User>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var plainPassword = "password123";
            var hashedPassword = "hashedpassword";
            var salt = "somesalt";
            var userToCreate = new Domain.Models.User
            {
                UserID = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                ContactNo = "1234567890",
                UserName = "johndoe",
                Password = plainPassword,
                UserTypeID = 1,
                IsActive = true
            };
            var exceptionMessage = "Database error";
            _passwordHelperMock.Setup(p => PasswordHelper.HashPassword(plainPassword, out salt))
                               .Returns(hashedPassword);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Models.User>()))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Create(userToCreate);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _passwordHelperMock.Verify(p => PasswordHelper.HashPassword(plainPassword, out salt), Times.Once());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.User>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenUserIsValidWithNewPassword()
        {
            // Arrange
            int id = 1;
            var plainPassword = "newpassword123";
            var hashedPassword = "newhashedpassword";
            var salt = "newsalt";
            var userToUpdate = new Domain.Models.User
            {
                UserID = id,
                FullName = "John Doe Updated",
                Email = "john.doe@example.com",
                ContactNo = "1234567890",
                UserName = "johndoe",
                Password = plainPassword,
                UserTypeID = 1,
                IsActive = true
            };
            _passwordHelperMock.Setup(p => PasswordHelper.HashPassword(plainPassword, out salt))
                               .Returns(hashedPassword);
            _userRepositoryMock.Setup(r => r.UpdateAsync(It.Is<Domain.Models.User>(u => u.Password == hashedPassword && u.Salt == salt)))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, userToUpdate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(userToUpdate, options => options.Excluding(u => u.Password).Excluding(u => u.Salt));
            _passwordHelperMock.Verify(p => PasswordHelper.HashPassword(plainPassword, out salt), Times.Once());
            _userRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Never());
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.User>()), Times.Once());
            _passwordHelperMock.Verify(p => PasswordHelper.HashPassword(It.IsAny<string>(), out It.Ref<string>.IsAny), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenUserIsValidWithoutPasswordChange()
        {
            // Arrange
            int id = 1;
            var existingUser = new Domain.Models.User
            {
                UserID = id,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                ContactNo = "1234567890",
                UserName = "johndoe",
                Password = "existinghashedpassword",
                Salt = "existingsalt",
                UserTypeID = 1,
                IsActive = true
            };
            var userToUpdate = new Domain.Models.User
            {
                UserID = id,
                FullName = "John Doe Updated",
                Email = "john.doe@example.com",
                ContactNo = "1234567890",
                UserName = "johndoe",
                Password = null, // No password change
                UserTypeID = 1,
                IsActive = true
            };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(id))
                               .ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.UpdateAsync(It.Is<Domain.Models.User>(u => u.Password == existingUser.Password && u.Salt == existingUser.Salt)))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, userToUpdate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(userToUpdate, options => options.Excluding(u => u.Password).Excluding(u => u.Salt));
            _passwordHelperMock.Verify(p => PasswordHelper.HashPassword(It.IsAny<string>(), out It.Ref<string>.IsAny), Times.Never());
            _userRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.User>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _controller.Update(id, null);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _passwordHelperMock.Verify(p => PasswordHelper.HashPassword(It.IsAny<string>(), out It.Ref<string>.IsAny), Times.Never());
            _userRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never());
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.User>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var userToUpdate = new Domain.Models.User
            {
                UserID = id,
                FullName = "John Doe Updated",
                Email = "john.doe@example.com",
                ContactNo = "1234567890",
                UserName = "johndoe",
                Password = null,
                UserTypeID = 1,
                IsActive = true
            };
            var exceptionMessage = "Update error";
            _userRepositoryMock.Setup(r => r.GetByIdAsync(id))
                               .ReturnsAsync(_testUser);
            _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Models.User>()))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Update(id, userToUpdate);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.User>()), Times.Once());
        }
    }
}