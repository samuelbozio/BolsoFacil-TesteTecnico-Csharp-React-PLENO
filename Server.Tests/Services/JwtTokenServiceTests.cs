using Xunit;
using Moq;
using Server.Services;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Server.Tests.Services
{
    public class JwtTokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<JwtTokenService>> _mockLogger;
        private readonly JwtTokenService _jwtTokenService;

        public JwtTokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<JwtTokenService>>();

            // Configurar valores padrão
            _mockConfiguration.Setup(x => x["Jwt:SecretKey"])
                .Returns("your-secret-key-must-be-at-least-32-characters-long-for-256-bit-hs256!");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"])
                .Returns("HouseholdExpensesAPI");
            _mockConfiguration.Setup(x => x["Jwt:Audience"])
                .Returns("HouseholdExpensesAPI");
            _mockConfiguration.Setup(x => x["Jwt:ExpireMinutes"])
                .Returns("60");

            _jwtTokenService = new JwtTokenService(_mockConfiguration.Object, _mockLogger.Object);
        }

        [Fact]
        public void GenerateToken_WithValidData_ReturnsValidToken()
        {
            // Arrange
            int userId = 1;
            string username = "testuser";
            string email = "test@example.com";

            // Act
            var token = _jwtTokenService.GenerateToken(userId, username, email);

            // Assert
            Assert.NotEmpty(token);
            Assert.True(token.Split('.').Length == 3); // JWT tem 3 partes
        }

        [Fact]
        public void GenerateToken_WithoutSecretKey_ThrowsException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["Jwt:SecretKey"])
                .Returns((string)null);

            var service = new JwtTokenService(_mockConfiguration.Object, _mockLogger.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                service.GenerateToken(1, "user", "email@test.com"));
        }

        [Fact]
        public void ValidateToken_WithValidToken_ReturnsTrue()
        {
            // Arrange
            var token = _jwtTokenService.GenerateToken(1, "testuser", "test@example.com");

            // Act
            var isValid = _jwtTokenService.ValidateToken(token);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ReturnsFalse()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var isValid = _jwtTokenService.ValidateToken(invalidToken);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateToken_WithoutSecretKey_ReturnsFalse()
        {
            // Arrange
            var token = _jwtTokenService.GenerateToken(1, "testuser", "test@example.com");
            _mockConfiguration.Setup(x => x["Jwt:SecretKey"])
                .Returns((string)null);

            var service = new JwtTokenService(_mockConfiguration.Object, _mockLogger.Object);

            // Act
            var isValid = service.ValidateToken(token);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void GetClaimsFromToken_WithValidToken_ReturnsClaims()
        {
            // Arrange
            var token = _jwtTokenService.GenerateToken(1, "testuser", "test@example.com");

            // Act
            var claims = _jwtTokenService.GetClaimsFromToken(token);

            // Assert
            Assert.NotNull(claims);
            var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            Assert.NotNull(userIdClaim);
            Assert.Equal("1", userIdClaim.Value);
        }

        [Fact]
        public void GenerateToken_TokenExpiration_RespectsConfiguration()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["Jwt:ExpireMinutes"])
                .Returns("1"); // 1 minuto apenas para teste

            var service = new JwtTokenService(_mockConfiguration.Object, _mockLogger.Object);

            // Act
            var token = service.GenerateToken(1, "user", "email@test.com");

            // Assert
            Assert.True(service.ValidateToken(token)); // Deve ser válido imediatamente
        }
    }
}
