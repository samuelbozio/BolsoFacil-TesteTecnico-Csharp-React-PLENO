using Xunit;
using Moq;
using Server.Controllers;
using Server.DTOs;
using Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Server.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IJwtTokenService> _mockJwtTokenService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockJwtTokenService = new Mock<IJwtTokenService>();
            _mockLogger = new Mock<ILogger<AuthController>>();

            _authController = new AuthController(_mockJwtTokenService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "usuario1", Password = "senha123" };
            var expectedToken = "valid.jwt.token";

            _mockJwtTokenService.Setup(x => x.GenerateToken(
                It.IsAny<int>(),
                loginDto.Username,
                It.IsAny<string>()))
                .Returns(expectedToken);

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDTO>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(expectedToken, response.Token);
            Assert.NotNull(response.User);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "invaliduser", Password = "invalidpass" };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDTO>(unauthorizedResult.Value);
            Assert.False(response.Success);
            Assert.Contains("inválidas", response.Message);
        }

        [Fact]
        public void Login_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var result = _authController.Login(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDTO>(badRequest.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public void Login_WithEmptyUsername_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "", Password = "senha123" };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDTO>(badRequest.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public void Login_WithEmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "usuario1", Password = "" };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDTO>(badRequest.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public void Login_WithCorrectAdminCredentials_ReturnsTokenForAdmin()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "admin", Password = "admin123" };
            var expectedToken = "admin.jwt.token";

            _mockJwtTokenService.Setup(x => x.GenerateToken(
                It.IsAny<int>(),
                "admin",
                It.IsAny<string>()))
                .Returns(expectedToken);

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDTO>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("admin", response.User.Username);
        }
    }
}
